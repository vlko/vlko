using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace vlko.core.DBAccess.Querying
{
    public class SortDefinition<T> : Dictionary<string, Func<IQueryable<T>, IQueryable<T>>> where T : class
    {
        public Func<IQueryable<T>, IQueryable<T>> DefaultSort { get; set; }

        /// <summary>
        /// Adds sort do sort definition.
        /// </summary>
        /// <typeparam name="TType">Sort type.</typeparam>
        /// <param name="ident">Sort ident.</param>
        /// <param name="sortFunc">Property sort resolve function.</param>
        /// <returns>This object for Fluent interface.</returns>
        public SortDefinition<T> AddSort<TType>(string ident, Expression<Func<T, TType>> sortFunc)
        {
            if (typeof(TType) == typeof(NoSortDefinition))
            {
                this[ident] = query => query;
                this[ident + "-desc"] = query => query;
            }
            else
            {
                this[ident] = query => query.OrderBy(sortFunc);
                this[ident + "-desc"] = query => query.OrderByDescending(sortFunc);
            }


            return this;
        }

        /// <summary>
        /// Adds sort do sort definition.
        /// </summary>
        /// <returns>This object for Fluent interface.</returns>
        public SortDefinition<T> AddSort(string ident, Func<IQueryable<T>, IQueryable<T>> ascSort, Func<IQueryable<T>, IQueryable<T>> descSort)
        {
            this[ident] = query => ascSort(query);
            this[ident + "-desc"] = query => descSort(query);

            return this;
        }

        /// <summary>
        /// Adds default sort function
        /// </summary>
        /// <param name="defaultSortFunction">Default sort function.</param>
        /// <returns>This object for Fluent interface.</returns>
        public SortDefinition<T> AddDefaultSort(Func<IQueryable<T>, IQueryable<T>> defaultSortFunction)
        {
            DefaultSort = defaultSortFunction;

            return this;
        }

        /// <summary>
        /// Creates copy without default sort function
        /// </summary>
        /// <returns>Copy of this instance without default sort.</returns>
        public SortDefinition<T> CreateCopy()
        {
            var result = new SortDefinition<T>();
            foreach (var pair in this)
            {
                result.Add(pair.Key, pair.Value);
            }
            return result;
        }
    }
}
