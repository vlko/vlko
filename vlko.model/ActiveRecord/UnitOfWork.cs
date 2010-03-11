using Castle.ActiveRecord;
using GenericRepository;

namespace vlko.model.ActiveRecord
{
    /// <summary>
    /// Session implementation for Active record.
    /// </summary>
    public class UnitOfWork : SessionScope, IUnitOfWork
    {
        public UnitOfWork()
            : base(FlushAction.Never)
        {
        }
    }
}


