using System.ComponentModel.Composition;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;

namespace vlko.BlogModule.Search
{
	[InheritedExport]
	public interface ISearchProvider
	{
		/// <summary>
		/// Initializes this instance.
		/// </summary>
		void Initialize(string indexFolder);

		/// <summary>
		/// Gets the index writer.
		/// </summary>
		/// <returns>Index writer.</returns>
		IndexWriter OpenIndexWriter();

		/// <summary>
		/// Closes the index writer.
		/// </summary>
		/// <param name="writer">The writer.</param>
		void CloseIndexWriter(IndexWriter writer);

		/// <summary>
		/// Gets the index searcher.
		/// </summary>
		/// <returns>Index searcher.</returns>
		IndexSearcher GetIndexSearcher();

		/// <summary>
		/// Gets the query parser.
		/// </summary>
		/// <param name="fields">The fields.</param>
		/// <returns>Query parser.</returns>
		MultiFieldQueryParser GetQueryParser(string[] fields);

		/// <summary>
		/// Optimizes the index.
		/// </summary>
		void OptimizeIndex();
	}
}
