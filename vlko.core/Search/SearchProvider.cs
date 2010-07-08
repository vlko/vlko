using System;
using System.IO;
using System.Threading;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Directory = Lucene.Net.Store.Directory;

namespace vlko.core.Search
{
	public class SearchProvider : ISearchProvider
	{
		private Directory _directory;
		private Analyzer _analyzer;
		private IndexWriter _writer;
		private int _currentWriters;

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="indexFolder"></param>
		public void Initialize(string indexFolder)
		{
			var pathToIndex = Path.Combine(indexFolder, "Index");
			_directory = FSDirectory.GetDirectory(pathToIndex);
			_analyzer = new StandardAnalyzer();
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