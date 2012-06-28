using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Directory = Lucene.Net.Store.Directory;

namespace vlko.BlogModule.Search
{
	public class SearchProvider : ISearchProvider
	{
		private Directory _directory;
		private Analyzer _analyzer;
		private IndexWriter _writer;
		private int _currentWriters;
	    private string _pathToIndex;

	    /// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="indexFolder"></param>
		public void Initialize(string indexFolder)
		{
	        _pathToIndex = Path.Combine(indexFolder, "Index");
            _directory = FSDirectory.GetDirectory(_pathToIndex);
			_analyzer = new StandardAnalyzer();
		}

        /// <summary>
        /// Deletes the index.
        /// </summary>
        public void DeleteIndex()
        {
            _directory.Close();
            System.IO.Directory.Delete(_pathToIndex, true);
        }

		/// <summary>
		/// Gets the index writer.
		/// </summary>
		/// <returns>Index writer.</returns>
		public IndexWriter OpenIndexWriter()
		{
			lock (this)
			{
				if (_currentWriters == 0)
				{
					_writer = new IndexWriter(_directory, _analyzer);
				}
				++_currentWriters;
			}
			return _writer;
		}

		/// <summary>
		/// Gets the index reader.
		/// </summary>
		/// <returns>Index reader.</returns>
		public void CloseIndexWriter(IndexWriter writer)
		{
			lock (this)
			{
				--_currentWriters;
				if (_currentWriters == 0)
				{
					writer.Commit();
					writer.Close();
				}
			}
		}

		/// <summary>
		/// Gets the index searcher.
		/// </summary>
		/// <returns>Index searcher.</returns>
		public IndexSearcher GetIndexSearcher()
		{
			return new IndexSearcher(_directory);
		}

		/// <summary>
		/// Gets the query parser.
		/// </summary>
		/// <param name="fields">The fields.</param>
		/// <returns>Query parser.</returns>
		public MultiFieldQueryParser GetQueryParser(string[] fields)
		{
			return new MultiFieldQueryParser(fields, _analyzer);
		}

		/// <summary>
		/// Optimizes the index.
		/// </summary>
		public void OptimizeIndex()
		{
			var indexWriter = OpenIndexWriter();
			indexWriter.Optimize();
			indexWriter.Close(true);
		}
	}
}