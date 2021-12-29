using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vlko.core.DBAccess.Querying;
using vlko.core.Tools;

namespace vlko.core.RavenDB.DBAccess.Querying
{
    public class QueryDef<T> : QueryDef<T, T>, IQueryDef<T> where T : class
    {
        public QueryDef(IQueryable<T> query, RavenSession session) : base(query, session)
        {
        }

        public IQueryDef<T> AdditonalWhere(Func<IQueryable<T>, IQueryable<T>> whereConditionProcessor)
        {
            return new QueryDef<T>(whereConditionProcessor(_query), Session);
        }
    }

    public class QueryDef<T, TResult> : IQueryDef<T, TResult> where T : class where TResult : class
    {
        protected readonly IRavenQueryable<T> _query;
        private readonly bool _projectOnlyFromFields;
        private readonly Type _transformer;
        private readonly IDictionary<string, string> _transformParameters;
        public RavenSession Session { get; private set; }


        internal QueryDef(IQueryable<T> query, RavenSession session, Type transformer = null, IDictionary<string, string> transformParameters = null, bool projectOnlyFromFields = false)
        {
            Session = session;
            _query = (IRavenQueryable<T>)query;
            _transformer = transformer;
            _transformParameters = transformParameters;
            _projectOnlyFromFields = projectOnlyFromFields;
        }

        /// <summary>
        /// Load partial data projected to TResult. from query.
        /// </summary>
        /// <typeparam name="TResult">Number of items to skip</typeparam>
        /// <param name="skip">Starting index.</param>
        /// <param name="take">Number of items to take.</param>
        /// <param name="sortDefinition">Sort definition</param>
        /// <param name="sort">Current sort ident.</param>
        /// <returns>Partial data projected to TResult.</returns>
        public PartialResult<TResult> LoadPartial(int skip, int take, SortDefinition<T> sortDefinition = null, string sort = null)
        {
            IQueryable<T> query = _query.Statistics(out QueryStatistics statistics).Skip(skip).Take(take);
            if (sortDefinition != null)
            {
                if (!string.IsNullOrEmpty(sort) && sortDefinition.ContainsKey(sort))
                {
                    query = sortDefinition[sort](query);
                }
                else if (sortDefinition.DefaultSort != null)
                {
                    query = sortDefinition.DefaultSort(query);
                }
            }
            IQueryable<TResult> projectQuery;
            if (typeof(T) != typeof(TResult))
            {
                if (_transformer != null)
                {
                    var transformer = (AbstractTransformer<T, TResult>)InstanceCreator.Create(_transformer);
                    projectQuery = QueryExtensions.GenerateTransformQuery(query, transformer, _transformParameters);
                }
                else
                {
                    if (_projectOnlyFromFields)
                    {
                        projectQuery = query.ProjectInto<TResult>()
#if FAILPROJECTION
                            .Customize(x => x.Projection(ProjectionBehavior.FromIndexOrThrow));
#else
                            .Customize(x => x.Projection(ProjectionBehavior.FromIndex));
#endif
                    }
                    else
                    {
                        projectQuery = query.ProjectInto<TResult>();
                    }
                }
            }
            else
            {
                projectQuery = query.OfType<TResult>();
            }
            var data = projectQuery.ToArray();
            var totalCount = statistics.TotalResults;

            return new PartialResult<TResult>(data, totalCount);
        }

        /// <summary>
        /// Project using TProjection type.
        /// </summary>
        /// <typeparam name="TProjection">Projection type.</typeparam>
        /// <returns>Query def with TProjection result.</returns>
        public IQueryDef<T, TProjection> Project<TProjection>() where TProjection : class
        {
            return new QueryDef<T, TProjection>(_query, Session);
        }

        /// <summary>
        /// Project only index fields using TProjection type.
        /// </summary>
        /// <typeparam name="TProjection">Projection type.</typeparam>
        /// <returns>Query def with TProjection result.</returns>
        public IQueryDef<T, TProjection> ProjectFromIndexFields<TProjection>() where TProjection : class
        {
            return new QueryDef<T, TProjection>(_query, Session, projectOnlyFromFields: true);
        }

        public IQueryDef<T, TTransform> Transform<TTransform, TTransformer>(IDictionary<string, string> transformParameters = null) where TTransform : class where TTransformer : class
        {
            return new QueryDef<T, TTransform>(_query, Session, typeof(TTransformer), transformParameters);
        }

        /// <summary>
        /// Get count of items in this query.
        /// </summary>
        /// <returns>Count of items in this query.</returns>
        public int Count()
        {
            _query.Statistics(out QueryStatistics statistics).Take(0).ToArray();
            return statistics.TotalResults;
        }

        public IEnumerable<TResult> Stream(Func<IQueryable<T>, IQueryable<T>> prepareQuery = null, int? count = null)
        {
            int position = 0;
            IQueryable<T> query = prepareQuery != null ? prepareQuery(_query) : _query;
            IQueryable<TResult> projectQuery;
            if (typeof(T) != typeof(TResult))
            {
                if (_transformer != null)
                {
                    var transformer = (AbstractTransformer<T, TResult>)InstanceCreator.Create(_transformer);
                    projectQuery = QueryExtensions.GenerateTransformQuery(query, transformer, _transformParameters);
                }
                else
                {
                    if (_projectOnlyFromFields)
                    {
                        projectQuery = query.ProjectInto<TResult>()
#if FAILPROJECTION
                            .Customize(x => x.Projection(ProjectionBehavior.FromIndexOrThrow));
#else
                            .Customize(x => x.Projection(ProjectionBehavior.FromIndex));
#endif
                    }
                    else
                    {
                        projectQuery = query.ProjectInto<TResult>();
                    }
                }
            }
            else
            {
                projectQuery = query.OfType<TResult>();
            }
            using var stream = Session.Advanced.Stream(projectQuery);
            while (stream.MoveNext())
            {
                ++position;
                if (count == null || position <= count.Value)
                {
                    yield return stream.Current.Document;
                }
                else
                {
                    yield break;
                }
            }
        }

        public List<TResult> LoadAllData()
        {
            var result = new List<TResult>();
            foreach (var item in Stream())
            {
                result.Add(item);
            }
            return result;
        }

        public Task<PartialResult<TResult>> LoadPartialAsync(int skip, int take, SortDefinition<T> sortDefinition = null, string sort = null)
        {
            throw new NotImplementedException("Use RavenAsyncSession.GetQueryDef for async calls");
        }

        public IAsyncEnumerable<TResult> StreamAsync(Func<IQueryable<T>, IQueryable<T>> prepareQuery = null, int? count = null)
        {
            throw new NotImplementedException("Use RavenAsyncSession.GetQueryDef for async calls");
        }

        public Task<int> CountAsync()
        {
            throw new NotImplementedException("Use RavenAsyncSession.GetQueryDef for async calls");
        }


        public Task<List<TResult>> LoadAllDataAsync()
        {
            throw new NotImplementedException();
        }
    }
}
