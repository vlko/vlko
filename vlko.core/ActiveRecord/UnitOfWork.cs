using Castle.ActiveRecord;
using GenericRepository;

namespace vlko.core.ActiveRecord
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


