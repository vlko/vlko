using System.Collections;
using System.Collections.Generic;
using vlko.model.Repository;

namespace vlko.core.Components
{
	public class PagedModel<T> : IEnumerable<T>, IPagedModel where T : class
	{
		public static int DefaultPageItems = 15;

		private T[] _currentData = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="PagedModel&lt;T&gt;"/> class.
		/// </summary>
		public PagedModel()
		{
			PageItems = DefaultPageItems;
			CurrentPage = 1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PagedModel&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="currentPage">The current page.</param>
		public PagedModel(int currentPage)
		{
			CurrentPage = currentPage; 
		}

		/// <summary>
		/// Gets or sets the page items.
		/// </summary>
		/// <value>The page items.</value>
		public int PageItems { get; set; }

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
		public int PagesNumber { get; private set; }

		/// <summary>
		/// Loads the data.
		/// </summary>
		/// <param name="queryResult">The query result.</param>
		/// <returns>This instance.</returns>
		public PagedModel<T> LoadData(IQueryResult<T> queryResult)
		{
			Count = queryResult.Count();

			PagesNumber = Count / PageItems + (Count % PageItems > 0 ? 1 : 0);

			// check hi and lo ranges
			if (CurrentPage > PagesNumber)
			{
				CurrentPage = PagesNumber;
			}
			if (CurrentPage < 1)
			{
				CurrentPage = 1;
			}


			_currentData = queryResult.ToPage(CurrentPage - 1, PageItems);

			return this;
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>Enumerator for current data.</returns>
		public IEnumerator<T> GetEnumerator()
		{
			return ((IEnumerable<T>) _currentData).GetEnumerator();
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

	}
}
