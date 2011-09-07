using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using vlko.core.Repository;
using vlko.core.Repository.Exceptions;

namespace vlko.BlogModule.RavenDB.Tests
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

        [TestMethod]
        public void Test_get_query()
        {
			var query = _mocker.StrictMock<ICommandGroup<object>>();
            var repository = _mocker.PartialMock<BaseRepository<object>>();

            using (_mocker.Record())
            {
				Expect.Call(_factoryResolver.ResolveCommand<ICommandGroup<object>>())
					.Do((Func<ICommandGroup<object>>)delegate
				{
                        return query;
                    });
                Expect.Call(query.Initialized).Return(false);
                Expect.Call(delegate { query.Initialize(null); })
                    .Constraints(Is.NotNull());
            }

            using (_mocker.Playback())
            {
				var resultQuery = repository.GetCommand<ICommandGroup<object>>();
                Assert.AreEqual(query, resultQuery);
            }
        }


        [TestMethod]
        public void Test_get_interface_query()
        {
            var query = _mocker.StrictMock<ITestQuery>();
            var repository = _mocker.PartialMock<BaseRepository<object>>();

            using (_mocker.Record())
            {
                Expect.Call(_factoryResolver.ResolveCommand<ITestQuery>())
                    .Do((Func<ITestQuery>)delegate{
                        return query;
                    });
                Expect.Call(query.Initialized).Return(false);
                Expect.Call(delegate { query.Initialize(null); })
                    .Constraints(Is.NotNull());
            }

            using (_mocker.Playback())
            {
				var resultQuery = repository.GetCommand<ITestQuery>();
                Assert.AreEqual(query, resultQuery);
            }
        }
        
        [TestMethod]
        public void Test_get_interface_implemented_query()
        {
            var query = new TestQuery();
            var repository = _mocker.PartialMock<BaseRepository<object>>();

            using (_mocker.Record())
            {
                Expect.Call(_factoryResolver.ResolveCommand<ITestQuery>())
                    .Do((Func<ITestQuery>)delegate{
                        return query;
                    });
            }

            using (_mocker.Playback())
            {
				var resultQuery = repository.GetCommand<ITestQuery>();
                Assert.AreEqual(query, resultQuery);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ActionNotRegisteredException))]
        public void Test_get_query_with_not_registered_exception()
        {
			var query = _mocker.StrictMock<ICommandGroup<object>>();
            var repository = _mocker.PartialMock<BaseRepository<object>>();
            
            using (_mocker.Record())
            {
                Expect.Call(_factoryResolver.ResolveCommand<ITestQuery>())
                    .Do((Func<ITestQuery>)delegate{
                        return null;
                    });
            }

            using (_mocker.Playback())
            {
				var resultQuery = repository.GetCommand<ITestQuery>();
            }
        }


        [TestMethod]
        [ExpectedException(typeof(ActionNotRegisteredException))]
        public void Test_get_query_with_RepositoryIoC_not_initialized_exception()
        {
            RepositoryFactory.IntitializeWith(null);
			var query = _mocker.StrictMock<ICommandGroup<object>>();
            var repository = _mocker.StrictMock<BaseRepository<object>>();

            using (_mocker.Record())
            {
                Expect.Call(_factoryResolver.ResolveCommand<ITestQuery>())
                    .Do((Func<ITestQuery>)delegate
                {
                    return null;
                });
            }

            using (_mocker.Playback())
            {
				var resultQuery = repository.GetCommand<ITestQuery>();
            }

        }

    }

	public interface ITestQuery : ICommandGroup<object>
    {
    }

    public class TestQuery : ITestQuery
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
