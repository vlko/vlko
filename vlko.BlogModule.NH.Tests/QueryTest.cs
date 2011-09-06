using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using vlko.core.Repository;
using vlko.core.Repository.Exceptions;

namespace vlko.BlogModule.NH.Tests
{
    [TestClass]
    public class QueryTest
    {
        private MockRepository _mocker;
        private IRepositoryFactoryResolver _factoryResolver;

        [TestInitialize]
        public void InitializeTreadManager()
        {
            _mocker = new MockRepository();
            _factoryResolver = _mocker.StrictMock<IRepositoryFactoryResolver>();
            RepositoryFactory.IntitializeWith(_factoryResolver);
        }

		[TestCleanup]
		public void CleanupThreadManager()
		{
			RepositoryFactory.IntitializeWith(null);
		}

        [TestMethod]
        public void Test_get_query()
        {
			var query = _mocker.StrictMock<IAction<object>>();
            var repository = _mocker.PartialMock<BaseRepository<object>>();

            using (_mocker.Record())
            {
				Expect.Call(_factoryResolver.ResolveAction<IAction<object>>())
					.Do((Func<IAction<object>>)delegate
				{
                        return query;
                    });
                Expect.Call(query.Initialized).Return(false);
                Expect.Call(delegate { query.Initialize(null); })
                    .Constraints(Is.NotNull());
            }

            using (_mocker.Playback())
            {
				var resultQuery = repository.GetAction<IAction<object>>();
                Assert.AreEqual(query, resultQuery);
            }
        }


        [TestMethod]
        public void Test_get_interface_query()
        {
            var query = _mocker.StrictMock<ITestQueryAction>();
            var repository = _mocker.PartialMock<BaseRepository<object>>();

            using (_mocker.Record())
            {
                Expect.Call(_factoryResolver.ResolveAction<ITestQueryAction>())
                    .Do((Func<ITestQueryAction>)delegate{
                        return query;
                    });
                Expect.Call(query.Initialized).Return(false);
                Expect.Call(delegate { query.Initialize(null); })
                    .Constraints(Is.NotNull());
            }

            using (_mocker.Playback())
            {
				var resultQuery = repository.GetAction<ITestQueryAction>();
                Assert.AreEqual(query, resultQuery);
            }
        }
        
        [TestMethod]
        public void Test_get_interface_implemented_query()
        {
            var query = new TestQueryAction();
            var repository = _mocker.PartialMock<BaseRepository<object>>();

            using (_mocker.Record())
            {
                Expect.Call(_factoryResolver.ResolveAction<ITestQueryAction>())
                    .Do((Func<ITestQueryAction>)delegate{
                        return query;
                    });
            }

            using (_mocker.Playback())
            {
				var resultQuery = repository.GetAction<ITestQueryAction>();
                Assert.AreEqual(query, resultQuery);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ActionNotRegisteredException))]
        public void Test_get_query_with_not_registered_exception()
        {
			var query = _mocker.StrictMock<IAction<object>>();
            var repository = _mocker.PartialMock<BaseRepository<object>>();
            
            using (_mocker.Record())
            {
                Expect.Call(_factoryResolver.ResolveAction<ITestQueryAction>())
                    .Do((Func<ITestQueryAction>)delegate{
                        return null;
                    });
            }

            using (_mocker.Playback())
            {
				var resultQuery = repository.GetAction<ITestQueryAction>();
            }
        }


        [TestMethod]
        [ExpectedException(typeof(ActionNotRegisteredException))]
        public void Test_get_query_with_RepositoryIoC_not_initialized_exception()
        {
            RepositoryFactory.IntitializeWith(null);
			var query = _mocker.StrictMock<IAction<object>>();
            var repository = _mocker.StrictMock<BaseRepository<object>>();

            using (_mocker.Record())
            {
                Expect.Call(_factoryResolver.ResolveAction<ITestQueryAction>())
                    .Do((Func<ITestQueryAction>)delegate
                {
                    return null;
                });
            }

            using (_mocker.Playback())
            {
				var resultQuery = repository.GetAction<ITestQueryAction>();
            }

        }

    }

	public interface ITestQueryAction : IAction<object>
    {
    }

    public class TestQueryAction : ITestQueryAction
    {
        public IQueryResult<object> Result()
        {
            throw new NotImplementedException();
        }

        public bool Initialized
        {
            get { return false; }
        }

        public void Initialize(IInitializeContext<object> intializeContext)
        {
            Assert.IsNotNull(intializeContext.Repository);
        }

    	public void Initialize()
    	{
    		
    	}
    }
}
