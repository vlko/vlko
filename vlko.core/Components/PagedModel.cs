using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vlko.core.Components;
using vlko.core.DBAccess;
using vlko.core.DBAccess.Querying;

namespace vlko.core.Components
{
    public class PagedModel<T> : IEnumerable<T>, IPagedModel where T : class
    {
        public class LocalOverride
        {
            public LocalOverride(IPagedModel sourcePageModel, IEnumerable<T> overridedData)
            {
                SourcePageModel = sourcePageModel;
                OverridedData = overridedData;
            }
            public IPagedModel SourcePageModel { get; private set; }
            public IEnumerable<T> OverridedData { get; private set; }
        }
        public static int DefaultPageItems = 15;

        private IEnumerable<T> _currentData = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedModel&lt;T&gt;"/> class.
        /// </summary>
        public PagedModel()
        {
            PageSize = DefaultPageItems;
            AllowedPageSizes = new[] { DefaultPageItems };
            CurrentPage = 1;
            _currentData = new T[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedModel&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        public PagedModel(int currentPage) : base()
        {
            CurrentPage = currentPage;
        }

        /// <summary>
        /// Sets the size of the page.
        /// </summary>
        /// <param name="pageSizes">The page sizes.</param>
	    public void SetPageSizes(params int[] pageSizes)
        {
            AllowedPageSizes = pageSizes;
            if (!AllowedPageSizes.Contains(PageSize))
            {
                PageSize = AllowedPageSizes[0];
            }
        }

        /// <summary>
        /// Gets or sets the allowed page sizes.
        /// </summary>
        /// <value>The allowed page sizes.</value>
        public int[] AllowedPageSizes { get; private set; }
        /// <summary>
        /// Gets or sets the page items.
        /// </summary>
        /// <value>The page items.</value>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>The current page.</value>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets the total count.
        /// </summary>
        /// <value>The total count.</value>
        public int Count { get; private set; }

        /// <summary>
        /// Gets the pages number.
        /// </summary>
        /// <value>The pages number.</value>
        public int PagesNumber { get; protected set; }

        public virtual PagedModel<T> LoadData(IEnumerable<T> queryResult)
        {
            CalculatePages(queryResult.Count());

            _currentData = queryResult.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

            return this;
        }

        public void RestrictPageNumber(int maximumPageNumber)
        {
            if (Count > maximumPageNumber * PageSize)
            {
                Count = maximumPageNumber * PageSize;
            }
            if (PagesNumber > maximumPageNumber)
            {
                PagesNumber = maximumPageNumber;
            }
        }
        /// <summary>
		/// Loads the data.
		/// </summary>
		/// <typeparam name="TSource">Source data type</typeparam>
        /// <param name="localOverride">Use local override for functionality loading from other index.</param>
		/// <returns>This instance.</returns>
		public PagedModel<T> LoadData<TSource>(IQueryDef<TSource, T> queryDefinition, SortDefinition<TSource> sortDefinition = null, string sort = null, Func<IEnumerable<T>, LocalOverride> localOverride = null)
            where TSource : class
        {
            var partialResult = PrepareResult(queryDefinition, sortDefinition, sort);
            Count = partialResult.TotalCount;
            // local override for data loaded by ident from other index
            if (localOverride != null)
            {
                var result = localOverride(partialResult.PageData);
                _currentData = result.OverridedData;
                CurrentPage = result.SourcePageModel.CurrentPage;
                CalculatePages(result.SourcePageModel.Count);
            }
            else
            {
                _currentData = partialResult.PageData;
            }

            return this;
        }

        /// <summary>
		/// Loads the data.
		/// </summary>
		/// <typeparam name="TSource">Source data type</typeparam>
        /// <param name="localOverride">Use local override for functionality loading from other index.</param>
		/// <returns>This instance.</returns>
		public async Task<PagedModel<T>> LoadDataAsync<TSource>(IQueryDef<TSource, T> queryDefinition, SortDefinition<TSource> sortDefinition = null, string sort = null, Func<IEnumerable<T>, LocalOverride> localOverride = null)
            where TSource : class
        {
            var partialResult = await PrepareResultAsync(queryDefinition, sortDefinition, sort);
            Count = partialResult.TotalCount;
            // local override for data loaded by ident from other index
            if (localOverride != null)
            {
                var result = localOverride(partialResult.PageData);
                _currentData = result.OverridedData;
                CurrentPage = result.SourcePageModel.CurrentPage;
                CalculatePages(result.SourcePageModel.Count);
            }
            else
            {
                _currentData = partialResult.PageData;
            }

            return this;
        }

        /// <summary>
        /// Use this function to get data with same paging applied as with page model.
        /// </summary>
        /// <typeparam name="TSource">Source data type</typeparam>
        /// <typeparam name="TData">Result data type.</typeparam>
        /// <returns>Data with applied paging.</returns>
        public virtual PartialResult<TData> PrepareResult<TSource, TData>(IQueryDef<TSource, TData> queryDefinition, SortDefinition<TSource> sortDefinition = null, string sort = null)
            where TSource : class
            where TData : class
        {
            if (!AllowedPageSizes.Contains(PageSize))
            {
                PageSize = AllowedPageSizes[0];
            }

            // check lo ranges
            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }

            bool repeatQueryBecauseOfRange;
            PartialResult<TData> partialResult;
            if (CurrentPage > 10000)
            {
                var count = queryDefinition.Count();
                PagesNumber = count / PageSize + (count % PageSize > 0 ? 1 : 0);
                if (CurrentPage > PagesNumber)
                {
                    CurrentPage = PagesNumber;
                }
            }
            do
            {
                repeatQueryBecauseOfRange = false;

                partialResult = queryDefinition.LoadPartial((CurrentPage - 1) * PageSize, PageSize, sortDefinition, sort);

                var count = partialResult.TotalCount;
                PagesNumber = count / PageSize + (count % PageSize > 0 ? 1 : 0);

                // check hi and lo ranges
                if (CurrentPage > PagesNumber)
                {
                    CurrentPage = PagesNumber;
                    repeatQueryBecauseOfRange = true;
                }

            }
            while (repeatQueryBecauseOfRange);
            return partialResult;
        }

        /// <summary>
        /// Use this function to get data with same paging applied as with page model.
        /// </summary>
        /// <typeparam name="TSource">Source data type</typeparam>
        /// <typeparam name="TData">Result data type.</typeparam>
        /// <returns>Data with applied paging.</returns>
        public virtual async Task<PartialResult<TData>> PrepareResultAsync<TSource, TData>(IQueryDef<TSource, TData> queryDefinition, SortDefinition<TSource> sortDefinition = null, string sort = null)
            where TSource : class
            where TData : class
        {
            if (!AllowedPageSizes.Contains(PageSize))
            {
                PageSize = AllowedPageSizes[0];
            }

            // check lo ranges
            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }

            bool repeatQueryBecauseOfRange;
            PartialResult<TData> partialResult;
            if (CurrentPage > 10000)
            {
                var count = await queryDefinition.CountAsync();
                PagesNumber = count / PageSize + (count % PageSize > 0 ? 1 : 0);
                if (CurrentPage > PagesNumber)
                {
                    CurrentPage = PagesNumber;
                }
            }
            do
            {
                repeatQueryBecauseOfRange = false;

                partialResult = await queryDefinition.LoadPartialAsync((CurrentPage - 1) * PageSize, PageSize, sortDefinition, sort);

                var count = partialResult.TotalCount;
                PagesNumber = count / PageSize + (count % PageSize > 0 ? 1 : 0);

                // check hi and lo ranges
                if (count > 0 && CurrentPage > PagesNumber)
                {
                    CurrentPage = PagesNumber;
                    repeatQueryBecauseOfRange = true;
                }

            }
            while (repeatQueryBecauseOfRange);
            return partialResult;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Enumerator for current data.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _currentData.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Calculates the pagination.
        /// </summary>
        /// <param name="count">Total items count</param>
        /// <returns>This instance.</returns>
        protected PagedModel<T> CalculatePages(int count)
        {
            if (!AllowedPageSizes.Contains(PageSize))
            {
                PageSize = AllowedPageSizes[0];
            }

            Count = count;
            PagesNumber = Count / PageSize + (Count % PageSize > 0 ? 1 : 0);

            // check hi and lo ranges
            if (CurrentPage > PagesNumber)
            {
                CurrentPage = PagesNumber;
            }
            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }

            return this;
        }

    }
}
