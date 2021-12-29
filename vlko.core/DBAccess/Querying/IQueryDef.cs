using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using vlko.core.DBAccess;

namespace vlko.core.DBAccess.Querying
{
    public interface IQueryDef<T> : IQueryDef<T, T> where T : class
    {
        IQueryDef<T> AdditonalWhere(Func<IQueryable<T>, IQueryable<T>> whereConditionProcessor);
    }
    public interface IQueryDef<T, TResult> where T : class where TResult : class
    {
        /// <summary>
        /// Load partial data projected to TResult. from query.
        /// </summary>
        /// <typeparam name="TResult">Number of items to skip</typeparam>
        /// <param name="skip">Starting index.</param>
        /// <param name="take">Number of items to take.</param>
        /// <param name="sortDefinition">Sort definition</param>
        /// <param name="sort">Current sort ident.</param>
        /// <returns>Partial data projected to TResult.</returns>
        PartialResult<TResult> LoadPartial(int skip, int take, SortDefinition<T> sortDefinition = null, string sort = null);
        /// <summary>
        /// Load partial data projected to TResult. from query.
        /// </summary>
        /// <typeparam name="TResult">Number of items to skip</typeparam>
        /// <param name="skip">Starting index.</param>
        /// <param name="take">Number of items to take.</param>
        /// <param name="sortDefinition">Sort definition</param>
        /// <param name="sort">Current sort ident.</param>
        /// <returns>Partial data projected to TResult.</returns>
        Task<PartialResult<TResult>> LoadPartialAsync(int skip, int take, SortDefinition<T> sortDefinition = null, string sort = null);

        /// <summary>
        /// Project using TProjection type.
        /// </summary>
        /// <typeparam name="TProjection">Projection type.</typeparam>
        /// <returns>Query def with TProjection result.</returns>
        IQueryDef<T, TProjection> Project<TProjection>() where TProjection : class;
        IQueryDef<T, TProjection> ProjectFromIndexFields<TProjection>() where TProjection : class;

        IQueryDef<T, TTransform> Transform<TTransform, TTransformer>(IDictionary<string, string> transformParameters = null) where TTransform : class where TTransformer : class;

        IEnumerable<TResult> Stream(Func<IQueryable<T>, IQueryable<T>> prepareQuery, int? count = null);
        IAsyncEnumerable<TResult> StreamAsync(Func<IQueryable<T>, IQueryable<T>> prepareQuery = null, int? count = null);

        List<TResult> LoadAllData();
        Task<List<TResult>> LoadAllDataAsync();
        /// <summary>
        /// Get count of items in this query.
        /// </summary>
        /// <returns>Count of items in this query.</returns>
        int Count();
        /// <summary>
        /// Get count of items in this query.
        /// </summary>
        /// <returns>Count of items in this query.</returns>
        Task<int> CountAsync();
    }




}
