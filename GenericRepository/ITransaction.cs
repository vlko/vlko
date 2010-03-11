using System;

namespace GenericRepository
{
    public interface ITransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
