﻿using System;
using System.ComponentModel.Composition;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using vlko.core.Repository;

namespace vlko.BlogModule.Search
{
	[Export(typeof(SearchContext))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class SearchContext : IUnitOfWorkContext
	{

		private readonly ISearchProvider _searchProvider;
		private IndexSearcher _indexSearcher = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchContext"/> class.
		/// </summary>
		/// <param name="searchProvider">The search provider.</param>
		[ImportingConstructor]
		public SearchContext(ISearchProvider searchProvider)
		{
			_searchProvider = searchProvider;
		}

		/// <summary>
		/// Gets the index searcher.
		/// </summary>
		/// <value>The index searcher.</value>
		public IndexSearcher IndexSearcher
		{
			get
			{
				if (_indexSearcher == null)
				{
					_indexSearcher = _searchProvider.GetIndexSearcher();
				}
				return _indexSearcher;
			}
		}

		/// <summary>
		/// Gets the query parser.
		/// </summary>
		/// <param name="fields">The fields.</param>
		/// <returns>Query parser.</returns>
		public MultiFieldQueryParser GetQueryParser(string[] fields)
		{
			return _searchProvider.GetQueryParser(fields);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="SearchContext"/> is reclaimed by garbage collection.
		/// </summary>
		~SearchContext()
		{
			Dispose(false);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_indexSearcher != null)
				{
					_indexSearcher.Close();
				}
			}
		}
	}
}
