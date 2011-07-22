using System;
using NHibernate;
using vlko.core.NH.Repository;
using vlko.core.NH.Repository.RepositoryAction;
using vlko.core.Repository;
using vlko.core.Repository.RepositoryAction;
using ITransaction = vlko.core.Repository.ITransaction;

namespace vlko.BlogModule.Tests.Repository.NRepository.Implementation
{
    public class NLocalFactoryResolver : IRepositoryFactoryResolver
    {
    	private readonly ISessionFactory _sessionFactory;

    	public NLocalFactoryResolver(ISessionFactory sessionFactory)
		{
			_sessionFactory = sessionFactory;
		}

    	public T ResolveAction<T>() where T : class, IAction
        {
            Type type = typeof (T);
            if (type == typeof(NFilterLinqQueryAction))
            {
                return new NFilterLinqQueryAction() as T;
            }
            if (type == typeof(NFilterCriterion))
            {
                return new NFilterCriterion() as T;
            }
            if ((type == typeof(IUpdateAction<NTestObject>))
                || (type == typeof(ICreateAction<NTestObject>))
                || (type == typeof(IFindByPkAction<NTestObject>))
                || (type == typeof(IDeleteAction<NTestObject>)))
            {
                return new CRUDActions<NTestObject>() as T;
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
