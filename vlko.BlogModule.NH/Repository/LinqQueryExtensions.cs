using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;

namespace vlko.BlogModule.NH.Repository
{
	public static class LinqQueryExtensions
	{
		public static IQueryable<T> Cacheable<T>(this IQueryable<T> query)
		{
			return LinqExtensionMethods.Cacheable(query);
		}

		public static IEnumerable<T> ToFuture<T>(this IQueryable<T> query)
		{
			return LinqExtensionMethods.ToFuture(query);
		}

		public static IFutureValue<T> ToFutureValue<T>(this IQueryable<T> query)
		{
			return LinqExtensionMethods.ToFutureValue(query);
		}
	}
}