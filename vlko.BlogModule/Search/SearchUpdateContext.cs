using System;
using Lucene.Net.Index;
using vlko.core.Repository;

namespace vlko.BlogModule.Search
{
	public class SearchUpdateContext : ITransactionContext
	{
		private readonly ISearchProvider _searchProvider;
		private IndexWriter _indexWriter = null;
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchUpdateContext"/> class.
		/// </summary>
		/// <param name="searchProvider">The search provider.</param>
		public SearchUpdateContext(ISearchProvider searchProvider)
		{
			_searchProvider = searchProvider;
		}

		/// <summary>
		/// Gets the index writer.
		/// </summary>
		/// <value>The index writer.</value>
		public IndexWriter IndexWriter
		{
			get
			{
				if (_indexWriter == null)
				{
					_indexWriter = _searchProvider.OpenIndexWriter();
				}
				return _indexWriter;
			}
		}

		/// <summary>
		/// Commits this instance.
		/// </summary>
		public void Commit()
		{

		}

		/// <summary>
		/// Rollbacks this instance.
		/// </summary>
		public void Rollback()
		{

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
		/// <see cref="SearchUpdateContext"/> is reclaimed by garbage collection.
		/// </summary>
		~SearchUpdateContext()
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
				if (_indexWriter != null)
				{
					_searchProvider.CloseIndexWriter(_indexWriter);
				}
			}   
		}
	}
}
