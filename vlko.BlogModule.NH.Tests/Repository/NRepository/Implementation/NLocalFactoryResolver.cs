using System;
using NHibernate;
using vlko.core.NH.Repository;
using vlko.core.NH.Repository.RepositoryAction;
using vlko.core.Repository;
using vlko.core.Repository.RepositoryAction;
using ITransaction = vlko.core.Repository.ITransaction;

namespace vlko.BlogModule.NH.Tests.Repository.NRepository.Implementation
{
    public class NLocalFactoryResolver : IRepositoryFactoryResolver
    {
    	private readonly ISessionFactory _sessionFactory;

    	public NLocalFactoryResolver(ISessionFactory sessionFactory)
		{
			_sessionFactory = sessionFactory;
		}

    	public T ResolveCommand<T>() where T : class, ICommandGroup
        {
            Type type = typeof (T);
            if (type == typeof(NFilterLinqQuery))
            {
                return new NFilterLinqQuery() as T;
            }
            if (type == typeof(NFilterCriterion))
            {
                return new NFilterCriterion() as T;
            }
            if ((type == typeof(IUpdateCommand<NTestObject>))
                || (type == typeof(ICreateCommand<NTestObject>))
                || (type == typeof(IFindByPkCommand<NTestObject>))
                || (type == typeof(IDeleteCommand<NTestObject>)))
            {
                return new CrudCommands<NTestObject>() as T;
            }
            return null;
        }

        public IUnitOfWork GetUnitOfWork()
        {
			return new UnitOfWork();
        }

        public ITransaction GetTransaction()
        {
			return new Transaction();
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            if (typeof(T) == typeof(NTestObject))
            {
                return new Repository<T>();
            }

            return null; 
        }
    }
}
