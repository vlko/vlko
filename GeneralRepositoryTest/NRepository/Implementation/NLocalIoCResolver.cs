using System;
using GenericRepository;
using GenericRepository.RepositoryAction;
using vlko.core.ActiveRecord;
using vlko.core.ActiveRecord.RepositoryAction;

namespace GeneralRepositoryTest.NRepository
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

        public BaseRepository<T> GetRepository<T>() where T : class
        {
            if (typeof(T) == typeof(NTestObject))
            {
                return new Repository<T>();
            }

            return null; 
        }
    }
}
