using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using Raven.Client.Connection;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Client.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Raven.Client;
using vlko.core.Repository;
using vlko.core.Repository.Exceptions;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;

namespace vlko.core.RavenDB.Repository
{
	/// <summary>
	/// Session factory inspired by Scopes in Castle.ActiveRecord.
	/// </summary>
	public static class SessionFactory
	{
		private class StackInfo
		{
			public enum TransactionStateEnum
			{
				NoActiveTransaction,
				Active,
				Commit,
				Rollback
			}
			public DocumentSession Session { get; set; }
			public IDisposable Transaction { get; set; }
			public UnitOfWork TopUnitOfWork { get; set; }
			public Transaction TopTransaction { get; set; }
			public TransactionStateEnum TransactionState { get; set; }
		}

		/// <summary>
		/// Gets or sets the document store instance.
		/// </summary>
		/// <value>
		/// The document store instance.
		/// </value>
		public static IDocumentStore DocumentStoreInstance { get; set; }

		const string StackIdent = "SessionFactory.CurrentStack";

		[ThreadStatic]
		static StackInfo _stack;

		/// <summary>
		/// Gets the current session.
		/// </summary>
		/// <value>The current session.</value>
		public static IDocumentSession Current
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
		/// Waits for stale indexes.
		/// !NOTE! Use only if really necessary as can have great impact on performance !NOTE!
		/// </summary>
		public static void WaitForStaleIndexes()
		{
			IUnitOfWork staleIndexUnitOfWork = null;
			// create session if necessary
			if (!HasInitializedScope)
			{
				staleIndexUnitOfWork = RepositoryFactory.StartUnitOfWork();
			}
			// wait for all index
			foreach (var index in Current.Advanced.DatabaseCommands.GetIndexNames(0, int.MaxValue))
			{
				var indexName = ((JValue) index).Value as string;
				SessionFactory.Current.Advanced.LuceneQuery<object>(indexName)
					.WaitForNonStaleResults(TimeSpan.MaxValue).FirstOrDefault();
			}
			// close session if necessary
			if (staleIndexUnitOfWork != null)
			{
				staleIndexUnitOfWork.Dispose();
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
				stack.Session = (DocumentSession)DocumentStoreInstance.OpenSession();
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
				stack.Session = (DocumentSession)DocumentStoreInstance.OpenSession();
			}
			// and if no transaction yet, then start new one
			if (stack.TopTransaction == null)
			{
				stack.TopTransaction = transaction;
				stack.Transaction = RavenTransactionAccessor.StartTransaction();
				stack.TransactionState = StackInfo.TransactionStateEnum.Active;
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
				bool transactionStillActive = stack.TransactionState == StackInfo.TransactionStateEnum.Active;

				// if we have still open transaction then call rollback
				if (transactionStillActive)
				{
					RollbackTransaction(transaction);
				}

				// throw exception if transaction was still active and no exception raised, then throw my own exception
				if (transactionStillActive && Marshal.GetExceptionCode() == 0)
				{
					throw new SessionException("Transaction is open! Try commit or rollback before ");
				}

				stack.Transaction.Dispose();
				stack.TransactionState = StackInfo.TransactionStateEnum.NoActiveTransaction;
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
			stack.TransactionState = StackInfo.TransactionStateEnum.Rollback;
			stack.Session.Rollback(RavenTransactionAccessor.GetTransactionInformation().Id);
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
				if (stack.TransactionState == StackInfo.TransactionStateEnum.Rollback)
				{
					throw new SessionException("Nested transaction called Rollback and thus we are not able to commit!");
				}
				stack.Session.SaveChanges();
				stack.TransactionState = StackInfo.TransactionStateEnum.Commit;
				stack.Session.Commit(RavenTransactionAccessor.GetTransactionInformation().Id);
			}
			else
			{
				stack.Session.SaveChanges();
			}

		}

		/// <summary>
		/// Clears the stack info.
		/// </summary>
		/// <param name="stack">The stack.</param>
		private static void ClearStackInfo(StackInfo stack)
		{
			stack.Session.Dispose();
			stack.TopUnitOfWork = null;
			stack.TopUnitOfWork = null;
			stack.Session = null;
		}

		/// <summary>
		/// Gets the registered session.
		/// </summary>
		/// <returns>Current session.</returns>
		internal static IDocumentSession GetRegisteredSession()
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
		private static string ConvertToStringId(object id)
		{
			if (id is string)
			{
				return (string)id;
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}", GetTypeIdent(), id);
		}
		/// <summary>
		/// Gets the current.
		/// </summary>
		/// <value>The current.</value>
		public static IDocumentSession Current
		{
			get { return SessionFactory.Current; }
		}

		/// <summary>
		/// Existses the specified id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		public static bool Exists(object id)
		{
			return Current.Advanced.DatabaseCommands.Get(ConvertToStringId(id)) != null;
		}

		/// <summary>
		/// Creates the specified instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		public static void Store(T instance)
		{
			Current.Store(instance);
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
		/// Loads the specified id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Object with specified key or exception if not found.</returns>
		public static T Load(object id)
		{
			return Load(id, true);
		}

		/// <summary>
		/// Loads the specified id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="throwOnNotFound">if set to <c>true</c> [throw on not found].</param>
		/// <returns>Object with specified key</returns>
		public static T Load(object id, bool throwOnNotFound)
		{
			T loaded = Current.Load<T>(ConvertToStringId(id));
			if (throwOnNotFound && loaded == null)
			{
				throw new NotFoundException(typeof(T), id, string.Empty);
			}
			return loaded;
		}

		/// <summary>
		/// Loads the specified ids.
		/// </summary>
		/// <param name="ids">The ids.</param>
		/// <returns>Array of object for specified ids.</returns>
		public static T[] LoadMore<TKey>(IEnumerable<TKey> ids)
		{
			var idents = ids.Select(id => ConvertToStringId(id));
			return Current.Load<T>(idents);
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
		public static IRavenQueryable<T> Queryable
		{
			get
			{
				return Current.Query<T>();
			}
		}

		/// <summary>
		/// Queryable interface for specified index.
		/// </summary>
		/// <typeparam name="TIndexCreator">The type of the index creator.</typeparam>
		/// <returns>The queryable interface for linq use.</returns>
		public static IRavenQueryable<T> IndexQuery<TIndexCreator>() where TIndexCreator : AbstractIndexCreationTask, new()
		{
				return Current.Query<T, TIndexCreator>();
		}

		/// <summary>
		/// Queryable interface for specified index.
		/// </summary>
		/// <typeparam name="TIndexCreator">The type of the index creator.</typeparam>
		/// <typeparam name="TReduce">The type of the reduce result.</typeparam>
		/// <returns>The queryable interface for linq use.</returns>
		public static IRavenQueryable<TReduce> IndexQuery<TIndexCreator, TReduce>() where TIndexCreator : AbstractIndexCreationTask<T, TReduce>, new()
		{
			return Current.Query<TReduce, TIndexCreator>();
		}

		/// <summary>
		/// Queryable interface for specified index ident.
		/// </summary>
		/// <param name="indexIdent">The index ident.</param>
		/// <returns>The queryable interface for linq use.</returns>
		public static IRavenQueryable<T> IndexQuery(string indexIdent)
		{
		    return Current.Query<T>(indexIdent);
		}

		/// <summary>
		/// Loader helper to load class with include.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>Loader helper to load class with include.</returns>
		public static ILoaderWithInclude<object> Include(string path)
		{
			return Current.Include(path);
		}

		/// <summary>
		/// Loader helper to load class with include.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>Loader helper to load class with include.</returns>
		public static ILoaderWithInclude<T> Include(Expression<Func<T, object>> path)
		{
			return Current.Include(path);
		}

		/// <summary>Generates the id.</summary>
		/// <param name="entity">The entity.</param>
		/// <returns>Generate id based on </returns>
		public static string GenerateId(T entity)
		{
			return Current.Advanced.Conventions.GenerateDocumentKey(entity).ToLower();
		}

		/// <summary>
		/// Generate type ident.
		/// </summary>
		/// <returns>Type ident for this session factory type.</returns>
		public static string GetTypeIdent()
		{
			return Current.Advanced.Conventions.GetTypeTagName(typeof (T)).ToLower() + "/";
		}
	}
}
