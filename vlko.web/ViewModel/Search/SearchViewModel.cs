using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using vlko.core.Components;

namespace vlko.web.ViewModel.Search
{
	public class SearchViewModel
	{
		/// <summary>
		/// Gets or sets the search query.
		/// </summary>
		/// <value>The search query.</value>
		public string Query { get; set; }

		/// <summary>
		/// Gets or sets the search results.
		/// </summary>
		/// <value>The search results.</value>
		public PagedModel<Object> SearchResults { get; set; }
	}
}