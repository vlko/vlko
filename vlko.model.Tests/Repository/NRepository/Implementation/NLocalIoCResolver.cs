using System;
using vlko.core.Repository;
using vlko.core.Repository.RepositoryAction;
using vlko.model.Implementation.NH.Repository;
using vlko.model.Implementation.NH.Repository.RepositoryAction;

namespace vlko.model.Tests.Repository.NRepository.Implementation
{
    public class NLocalFactoryResolver : IRepositoryFactoryResolver
    {

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
            if ((type == typeof(ISaveAction<NTestObject>))
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
