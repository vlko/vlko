using System.ComponentModel.Composition;
using System.Data;
using NHibernate.Connection;

namespace vlko.BlogModule.NH.Testing
{


	/// <summary>
	/// ConnectionProvider for Sqlite in memory tests, that suppresses closing
	/// the connection to keep the data until the test is finished.
	/// </summary>
	[Export("test")]
	public class InMemoryConnectionProvider : DriverConnectionProvider
	{
		/// <summary>
		/// The connection to the database
		/// </summary>
		public static IDbConnection Connection = null;

		/// <summary>
		/// Called by the framework.
		/// </summary>
		/// <returns>A connection to the database</returns>
		public override IDbConnection GetConnection()
		{
			if (Connection == null)
				Connection = base.GetConnection();

			return Connection;
		}

		/// <summary>
		/// No-Op.
		/// </summary>
		/// <param name="conn">The connection to close.</param>
		public override void CloseConnection(IDbConnection conn) { }

		/// <summary>
		/// Closes the connection after the tests.
		/// </summary>
		public static void Restart()
		{
			if (Connection != null)
			{
				Connection.Close();
				Connection = null;
			}
		}
	}
}
