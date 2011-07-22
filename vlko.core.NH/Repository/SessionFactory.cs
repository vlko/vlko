using System;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web;
using NHibernate;
using NHibernate.Linq;
using vlko.core.Repository.Exceptions;

namespace vlko.core.NH.Repository
{
	/// <summary>
	/// Session factory inspired by Scopes in Castle.ActiveRecord.
	/// </summary>
	public static class SessionFactory
	{
		private class StackInfo
		{
			public ISession Session { get; set; }
			public UnitOfWork TopUnitOfWork { get; set; }
			public Transaction TopTransaction { get; set; }
		}

		/// <summary>
		/// Gets or sets the session factory instance.
		/// </summary>
		/// <value>
		/// The session factory instance.
		/// </value>
		public static ISessionFactory SessionFactoryInstance { get; set; }

		const string StackIdent = "SessionFactory.CurrentStack";

		[ThreadStatic]
		static StackInfo _stack;
		
		/// <summary>
		/// Gets the current session.
		/// </summary>
		/// <value>The current session.</value>
		public static ISession Current
		{
			get
			{
				var currentSession = GetRegisteredSession();
				if (currentSession == null)
				{
					throw new SessionException("There is no active session!");
				}
				return currentSession;
			}
		}

		/// <summary>
		/// Gets the current stack.
		/// </summary>
		/// <value>The current stack.</value>
		private static StackInfo CurrentStack
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			get
			{
				HttpContext current = HttpContext.Current;

				if (current == null)
				{
					return _stack ?? (_stack = new StackInfo());
				}

				StackInfo contextstack = (StackInfo)current.Items[StackIdent];

				if (contextstack == null)
				{
					contextstack = new StackInfo();

					current.Items[StackIdent] = contextstack;
				}

				return contextstack;
			}
		}

		/// <summary>
		/// Registers the unit of work.
		/// </summary>
		/// <param name="unitOfWork">The unit of work.</param>
		internal static void RegisterUnitOfWork(UnitOfWork unitOfWork)
		{
			var stack = CurrentStack;
			// if no session yet, start new
			if (stack.Session == null)
			{
				stack.TopUnitOfWork = unitOfWork;
				stack.Session = SessionFactoryInstance.OpenSession();
				stack.Session.FlushMode = FlushMode.Commit;
			}
		}

		/// <summary>
		/// Unregisters the unit of work.
		/// </summary>
		/// <param name="unitOfWork">The unit of work.</param>
		internal static void UnregisterUnitOfWork(UnitOfWork unitOfWork)
		{
			var stack = CurrentStack;
			if (stack.TopUnitOfWork == unitOfWork)
			{
				if (stack.TopTransaction != null)
				{
					throw new SessionException("Transaction is open! Try commit or rollback before ");
				}
				ClearStackInfo(stack);
			}
		}

		/// <summary>
		/// Registers the transaction.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		internal static void RegisterTransaction(Transaction transaction)
		{
			var stack = CurrentStack;
			// if no session yet, start new
			if (stack.Session == null)
			{
				stack.Session = SessionFactoryInstance.OpenSession();
				stack.Session.FlushMode = FlushMode.Commit;
			}
			// and if no transaction yet, then start new one
			if (stack.TopTransaction == null)
			{
				transaction.LockObject = stack.Session.Connection;
				// lock transaction concurrency
				Monitor.Enter(transaction.LockObject);

				stack.TopTransaction = transaction;
				stack.Session.BeginTransaction(IsolationLevel.Unspecified);
			}
		}

		/// <summary>
		/// Unregisters the transaction.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		internal static void UnregisterTransaction(Transaction transaction)
		{
			var stack = CurrentStack;
			if (stack.TopTransaction == transaction)
			{
				bool transactionStillActive = stack.Session.Transaction.IsActive;

				// if we have still open transaction then call rollback
				if (transactionStillActive)
				{
					RollbackTransaction(transaction);
				}

				// unlock transaction concurrency
				Monitor.Exit(transaction.LockObject);


				// throw exception if transaction was still active and no exception raised, then throw my own exception
				if (transactionStillActive && Marshal.GetExceptionCode() == 0)
				{
					throw new SessionException("Transaction is open! Try commit or rollback before ");
				}

				stack.TopTransaction = null;

				// if transaction is also top session then free session
				if (stack.TopUnitOfWork == null)
				{
					ClearStackInfo(stack);
				}
			}
		}


		/// <summary>
		/// Rollbacks the transaction.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		internal static void RollbackTransaction(Transaction transaction)
		{
			var stack = CurrentStack;
			stack.Session.Transaction.Rollback();
		}

		/// <summary>
		/// Commits the transaction.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		internal static void CommitTransaction(Transaction transaction)
		{
			var stack = CurrentStack;
			if (stack.TopTransaction == transaction)
			{
				if (stack.Session.Transaction.WasRolledBack)
				{
					throw new SessionException("Nested transaction called Rollback and thus we are not able to commit!");
				}
				stack.Session.Transaction.Commit();
			}
			else
			{
				stack.Session.Flush();
			}
			
		}

		/// <summary>
		/// Clears the stack info.
		/// </summary>
		/// <param name="stack">The stack.</param>
		private static void ClearStackInfo(StackInfo stack)
		{
			stack.Session.Close();
			stack.Session.Dispose();
			stack.TopUnitOfWork = null;
			stack.TopUnitOfWork = null;
			stack.Session = null;
		}

		/// <summary>
		/// Gets the registered session.
		/// </summary>
		/// <returns>Current session.</returns>
		internal static ISession GetRegisteredSession()
		{
			return CurrentStack.Session;
		}

		/// <summary>
		/// Gets a value indicating whether this instance has initialized scope.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has initialized scope; otherwise, <c>false</c>.
		/// </value>
		internal static bool HasInitializedScope
		{
			get { return GetRegisteredSession() != null; }
		}

	}

	public static class SessionFactory<T> where T : class
	{
		/// <summary>
		/// Gets the current.
		/// </summary>
		/// <value>The current.</value>
		public static ISession Current
		{
			get { return SessionFactory.Current; }
		}

		/// <summary>
		/// Creates the specified instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		public static void Create(T instance)
		{
			Current.Save(instance);
		}

		/// <summary>
		/// Updates the specified instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		public static void Update(T instance)
		{
			Current.Update(instance);
		}

		/// <summary>
		/// Creates the or update.
		/// </summary>
		/// <param name="instance">The instance.</param>
		public static void CreateOrUpdate(T instance)
		{
			Current.SaveOrUpdate(instance);
		}

		/// <summary>
		/// Deletes the specified instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		public static void Delete(T instance)
		{
			Current.Delete(instance);
		}



		/// <summary>
		/// Finds the by primary key.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Object with specified key or exception if not found.</returns>
		public static T FindByPrimaryKey(object id)
		{
			return FindByPrimaryKey(id, true);
		}

		/// <summary>
		/// Finds the by primary key.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="throwOnNotFound">if set to <c>true</c> [throw on not found].</param>
		/// <returns>Object with specified key</returns>
		public static T FindByPrimaryKey(object id, bool throwOnNotFound)
		{
			T loaded = Current.Get(typeof(T), id) as T;
			if (throwOnNotFound && loaded == null)
			{
				throw new NotFoundException(typeof(T), id, string.Empty);
			}
			return loaded;
		}

		/// <summary>
		/// Finds all.
		/// </summary>
		/// <returns>All items for this type.</returns>
		public static T[] FindAll()
		{
			return Queryable.ToArray();
		}

		/// <summary>
		/// Counts items for this type.
		/// </summary>
		/// <returns>Count of items for this type.</returns>
		public static int Count()
		{
			return Queryable.Count();
		}

		/// <summary>
		/// Gets the queryable interface for linq use.
		/// </summary>
		/// <value>The queryable interface for linq use.</value>
		public static IQueryable<T> Queryable
		{
			get
			{
				return Current.Query<T>();
			}
		}

		/// <summary>
		/// Gets the query over for criterion use.
		/// </summary>
		/// <value>The query over for criterion use.</value>
		public static IQueryOver<T,T> QueryOver
		{
			get
			{
				return Current.QueryOver<T>();
			}
		}
	}
}
