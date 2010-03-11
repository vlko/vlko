using GenericRepository;
using vlko.core.ActiveRecord;

namespace GeneralRepositoryTest.NRepository
{
    public class NLocalIoCResolver : IRepositoryIoCResolver
    {

        public T ResolveQuery<T>() where T : class
        {
            if (typeof(T) == typeof(NFilterLinqQuery))
            {
                return new NFilterLinqQuery() as T;
            }
            if (typeof(T) == typeof(NFilterCriterion))
            {
                return new NFilterCriterion() as T;
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
