using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace vlko.core.Base
{
	public static class LockExtensions
	{
		/// <summary>
		/// Create write lock.
		/// </summary>
		/// <param name="lockSlim">The lock slim.</param>
		/// <returns>Write lock disposable helper.</returns>
		public static WriteLockHelper WriteLock(this ReaderWriterLockSlim lockSlim)
		{
			return new WriteLockHelper(lockSlim);
		}

		/// <summary>
		/// Create read lock.
		/// </summary>
		/// <param name="lockSlim">The lock slim.</param>
		/// <returns>Read lock disposable helper.</returns>
		public static ReadLockHelper ReadLock(this ReaderWriterLockSlim lockSlim)
		{
			return new ReadLockHelper(lockSlim);
		}
		/// <summary>
		/// Write lock helper.
		/// </summary>
		public class WriteLockHelper : IDisposable
		{
			private ReaderWriterLockSlim _lockSlim;

			/// <summary>
			/// Initializes a new instance of the <see cref="WriteLockHelper"/> class.
			/// </summary>
			/// <param name="lockSlim">The lock slim.</param>
			internal WriteLockHelper(ReaderWriterLockSlim lockSlim)
			{
				_lockSlim = lockSlim;
				_lockSlim.EnterWriteLock();
			}

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			public void Dispose()
			{
				_lockSlim.ExitWriteLock();
			}
		}
		/// <summary>
		/// Read lock helper
		/// </summary>
		public class ReadLockHelper : IDisposable
		{
			private ReaderWriterLockSlim _lockSlim;

			/// <summary>
			/// Initializes a new instance of the <see cref="ReadLockHelper"/> class.
			/// </summary>
			/// <param name="lockSlim">The lock slim.</param>
			internal ReadLockHelper(ReaderWriterLockSlim lockSlim)
			{
				_lockSlim = lockSlim;
				_lockSlim.EnterReadLock();
			}

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			public void Dispose()
			{
				_lockSlim.ExitReadLock();
			}
		}
	}
}
