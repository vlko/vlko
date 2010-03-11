using System;

namespace GenericRepository
{
    /// <summary>
    /// ISession represents active connection to db/webservice/whatever.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
    }
}
