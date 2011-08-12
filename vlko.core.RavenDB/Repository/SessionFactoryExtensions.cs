using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Raven.Client.Document;
using vlko.core.Repository.Exceptions;

namespace vlko.core.RavenDB.Repository
{
	public static class SessionFactoryExtensions
	{
		/// <summary>
		/// Add contition to convert contains to or query the contains is implemented as or.
		/// </summary>
		/// <typeparam name="T">The type of queryable.</typeparam>
		/// <typeparam name="TArray">The type of the array.</typeparam>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="queryable">The queryable.</param>
		/// <param name="items">The items.</param>
		/// <param name="reduceFunction">The reduce function.</param>
		/// <param name="condition">The condition.</param>
		/// <returns>Queryable.</returns>
		public static IQueryable<T> WhereContainsAsOr<T, TArray, TKey>(this IQueryable<T> queryable, IEnumerable<TArray> items, Func<TArray, TKey> reduceFunction, Func<TKey, Expression<Func<T, bool>>> condition)
		{
			var reducedItems = items.Select(reduceFunction).ToArray();
			// continue only if any items in reduce
			if (reducedItems.Length > 0)
			{
				var partialLambdas = reducedItems.Select(condition);

				Expression current = null;
				ParameterExpression parameter = null;
				// merge as or
				foreach (var item in partialLambdas)
				{
					if (item != null)
					{
						if (current == null)
						{
							current = item.Body;
							parameter = item.Parameters[0];
						}
						else
						{
							current = Expression.OrElse(current, item.Body);
						}
					}
				}
				// if no partial lambda, then use always false expression
				if (current == null)
				{
					return null;
				}
				return queryable.Where(Expression.Lambda<Func<T, bool>>(current, parameter));
			}
			// add impossible condition as this is contains condition and we have nothing to contains 
			return queryable.Where(item => 1 == 0);
		}

		/// <summary>
		/// Loads the specified id.
		/// </summary>
		/// <typeparam name="T">Type of result.</typeparam>
		/// <param name="loader">The loader.</param>
		/// <param name="id">The id.</param>
		/// <returns>
		/// Object with specified key or exception if not found.
		/// </returns>
		public static T Load<T>(this ILoaderWithInclude<T> loader, object id)
		{
			return Load(loader, id, true);
		}

		/// <summary>
		/// Loads the specified id.
		/// </summary>
		/// <typeparam name="T">Type of result.</typeparam>
		/// <param name="loader">The loader.</param>
		/// <param name="id">The id.</param>
		/// <param name="throwOnNotFound">if set to <c>true</c> [throw on not found].</param>
		/// <returns>Object with specified key</returns>
		public static T Load<T>(this ILoaderWithInclude<T> loader, object id, bool throwOnNotFound)
		{
			T loaded = loader.Load<T>(string.Format(CultureInfo.InvariantCulture, "{0}", id));
			if (throwOnNotFound && loaded == null)
			{
				throw new NotFoundException(typeof(T), id, string.Empty);
			}
			return loaded;
		}

		/// <summary>
		/// Loads the specified ids.
		/// </summary>
		/// <typeparam name="T">Type of result.</typeparam>
		/// <param name="loader">The loader.</param>
		/// <param name="ids">The ids.</param>
		/// <returns>Array of object for specified ids.</returns>
		public static T[] Load<T>(this ILoaderWithInclude<T> loader, params object[] ids)
		{
			return Load(loader, ids.AsEnumerable());
		}

		/// <summary>
		/// Loads the specified ids.
		/// </summary>
		/// <typeparam name="T">Type of result.</typeparam>
		/// <param name="loader">The loader.</param>
		/// <param name="ids">The ids.</param>
		/// <returns>Array of object for specified ids.</returns>
		public static T[] Load<T>(this ILoaderWithInclude<T> loader, IEnumerable<object> ids)
		{
			var idents = ids.Select(id => string.Format(CultureInfo.InvariantCulture, "{0}", id));
			return loader.Load(idents);
		}
	}
}