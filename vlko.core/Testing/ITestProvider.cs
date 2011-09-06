using System.Collections.Generic;
using System.Linq;

namespace vlko.core.Testing
{
	public interface ITestProvider
	{
		void SetUp();
		void TearDown();

		void WaitForIndexing();

		void Create<T>(T model) where T : class;
		T GetById<T>(object id) where T : class;
		IEnumerable<T> FindAll<T>() where T : class;
		int Count<T>() where T : class;
		IQueryable<T> AsQueryable<T>() where T : class;
	}
}
