using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericRepository.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using GenericRepository;
using Rhino.Mocks.Constraints;

namespace GeneralRepositoryTest
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
            var query = _mocker.StrictMock<IQueryAction<object>>();
            var repository = _mocker.PartialMock<BaseRepository<object>>();

            using (_mocker.Record())
            {
                Expect.Call(_factoryResolver.ResolveAction<IQueryAction<object>>())
                    .Do((Func<IQueryAction<object>>)delegate{
                        return query;
                    });
                Expect.Call(query.Initialized).Return(false);
                Expect.Call(delegate { query.Initialize(null); })
                    .Constraints(Is.NotNull());
            }

            using (_mocker.Playback())
            {
                var resultQuery = repository.GetQuery<IQueryAction<object>>();
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
                var resultQuery = repository.GetQuery<ITestQueryAction>();
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
                var resultQuery = repository.GetQuery<ITestQueryAction>();
                Assert.AreEqual(query, resultQuery);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(QueryNotRegisteredException))]
        public void Test_get_query_with_not_registered_exception()
        {
            var query = _mocker.StrictMock<IQueryAction<object>>();
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
                var resultQuery = repository.GetQuery<ITestQueryAction>();
            }
        }


        [TestMethod]
        [ExpectedException(typeof(RepositoryFactoryNotInitializeException))]
        public void Test_get_query_with_RepositoryIoC_not_initialized_exception()
        {
            RepositoryFactory.IntitializeWith(null);
            var query = _mocker.StrictMock<IQueryAction<object>>();
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
                var resultQuery = repository.GetQuery<ITestQueryAction>();
            }

        }

    }

    public interface ITestQueryAction : IQueryAction<object>
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

        public void Initialize(InitializeContext<object> intializeContext)
        {
            Assert.IsNotNull(intializeContext.BaseRepository);
        }

    	public void Initialize()
    	{
    		
    	}
    }
}
