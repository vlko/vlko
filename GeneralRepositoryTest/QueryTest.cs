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
        private IRepositoryIoCResolver _ioCResolver;

        [TestInitialize]
        public void InitializeTreadManager()
        {
            _mocker = new MockRepository();
            _ioCResolver = _mocker.StrictMock<IRepositoryIoCResolver>();
            RepositoryIoC.IntitializeWith(_ioCResolver);
        }

        [TestMethod]
        public void Test_get_query()
        {
            var query = _mocker.StrictMock<IQuery<object>>();
            var repository = _mocker.PartialMock<BaseRepository<object>>();

            using (_mocker.Record())
            {
                Expect.Call(_ioCResolver.ResolveQuery<IQuery<object>>())
                    .Do((Func<IQuery<object>>)delegate{
                        return query;
                    });
                Expect.Call(delegate { query.Initialize(null); })
                    .Constraints(Is.NotNull());
            }

            using (_mocker.Playback())
            {
                var resultQuery = repository.GetQuery<IQuery<object>>();
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
                Expect.Call(_ioCResolver.ResolveQuery<ITestQuery>())
                    .Do((Func<ITestQuery>)delegate{
                        return query;
                    });
                Expect.Call(delegate { query.Initialize(null); })
                    .Constraints(Is.NotNull());
            }

            using (_mocker.Playback())
            {
                var resultQuery = repository.GetQuery<ITestQuery>();
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
                Expect.Call(_ioCResolver.ResolveQuery<ITestQuery>())
                    .Do((Func<ITestQuery>)delegate{
                        return query;
                    });
            }

            using (_mocker.Playback())
            {
                var resultQuery = repository.GetQuery<ITestQuery>();
                Assert.AreEqual(query, resultQuery);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(QueryNotRegisteredException))]
        public void Test_get_query_with_not_registered_exception()
        {
            var query = _mocker.StrictMock<IQuery<object>>();
            var repository = _mocker.PartialMock<BaseRepository<object>>();
            
            using (_mocker.Record())
            {
                Expect.Call(_ioCResolver.ResolveQuery<ITestQuery>())
                    .Do((Func<ITestQuery>)delegate{
                        return null;
                    });
            }

            using (_mocker.Playback())
            {
                var resultQuery = repository.GetQuery<ITestQuery>();
            }
        }


        [TestMethod]
        [ExpectedException(typeof(RepositoryIoCNotInitializeException))]
        public void Test_get_query_with_RepositoryIoC_not_initialized_exception()
        {
            RepositoryIoC.IntitializeWith(null);
            var query = _mocker.StrictMock<IQuery<object>>();
            var repository = _mocker.StrictMock<BaseRepository<object>>();

            using (_mocker.Record())
            {
                Expect.Call(_ioCResolver.ResolveQuery<ITestQuery>())
                    .Do((Func<ITestQuery>)delegate
                {
                    return null;
                });
            }

            using (_mocker.Playback())
            {
                var resultQuery = repository.GetQuery<ITestQuery>();
            }

        }

    }

    public interface ITestQuery : IQuery<object>
    {
    }

    public class TestQuery : ITestQuery
    {
        public IQueryResult<object> Result()
        {
            throw new NotImplementedException();
        }

        public void Initialize(QueryInitializeContext<object> intializeContext)
        {
            Assert.IsNotNull(intializeContext.BaseRepository);
        }
    }
}
