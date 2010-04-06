using GenericRepository;
using Castle.ActiveRecord;
using NotFoundException = GenericRepository.Exceptions.NotFoundException;

namespace vlko.core.ActiveRecord
{
    public class Repository<T> : BaseRepository<T> where T : class
    {
       
    }
}


