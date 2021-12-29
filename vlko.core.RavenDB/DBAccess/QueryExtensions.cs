using Raven.Client.Documents.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using vlko.core.RavenDB.DBAccess;
using vlko.core.Tools;

namespace Raven.Client.Documents.Linq
{
    public static class QueryExtensions
    {
        /// <summary>
        /// Create query to query by with projection result only from index fields.
        /// </summary>
        /// <typeparam name="TProjection">The type of the projection.</typeparam>
        public static IQueryable<TProjection> ProjectFromIndexFields<TProjection>(this IQueryable query) where TProjection : class
        {
            return query.ProjectInto<TProjection>()
#if FAILPROJECTION
                .Customize(x => x.Projection(ProjectionBehavior.FromIndexOrThrow));
#else
                .Customize(x => x.Projection(ProjectionBehavior.FromIndex));
#endif
        }


        public static IQueryable<TResult> TransformWith<TFrom, TTransformer, TResult>(this IQueryable<TFrom> query, IDictionary<string, string> transformParameters = null)
             where TFrom : class
        {
            var transformer = (AbstractTransformer<TFrom, TResult>)InstanceCreator.Create(typeof(TTransformer));
            return GenerateTransformQuery(query, transformer, transformParameters);
        }
        internal static IQueryable<TResult> GenerateTransformQuery<TFrom, TResult>(IQueryable<TFrom> query, AbstractTransformer<TFrom, TResult> transformer, IDictionary<string, string> transformParameters)
            where TFrom : class
        {
            IQueryable<TResult> projectQuery;
            if (transformParameters != null)
            {
                if (transformer.TransformResultWithParameters == null)
                {
                    throw new RavenDBAccessException($"Transformer '{transformer.GetType().Name}' has paremeters and not implemented 'TransformResultWitParameters'!");
                }
                if (transformer.PrepareDefaultParameters == null)
                {
                    throw new RavenDBAccessException($"Transformer '{transformer.GetType().Name}' has paremeters and not implemented 'PrepareDefaultParameters'! Use this function to prepare default values for all parameters");
                }
                transformer.PrepareDefaultParameters(transformParameters);
                projectQuery = transformer.TransformResultWithParameters(query, transformParameters);
            }
            else
            {
                if (transformer.TransformResult == null)
                {
                    throw new RavenDBAccessException($"Transformer '{transformer.GetType().Name}' has not implemented 'TransformResult'!");
                }
                projectQuery = transformer.TransformResult(query);
            }

            return projectQuery;
        }

        public static string AddWhereConditionToQuery(string query, string rawContiton)
        {
            // regex for query pattern
            var match = Regex.Match(
                query,
                @"^(from\s+index\s+[\'A-z0-9]+\s*(?:as\s+[A-z0-9]+\s*)?)(where\s*(.*?)\s*)?((?:\sorder\s|\slimit\s|\sselect\s).*)$",
                RegexOptions.Singleline);
            if (!match.Success)
            {
                throw new RavenDBAccessException($"Not able to parse query for '{query}'!");
            }
            var whereCondition = " where " + (match.Groups[2].Success ? $"({rawContiton}) AND ({match.Groups[3].Value}) " : rawContiton + " "); 
            // use id() to not create auto index
            return match.Groups[1].Value + whereCondition + match.Groups[4].Value;
        }
    }
}
