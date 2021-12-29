using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using vlko.core.DBAccess.Querying;
using vlko.core.Tools;

namespace vlko.core.RavenDB.DBAccess
{
    public static class SortHelper
    {
        public static SortDefinition<T> AddSortScore<T, TType>(this SortDefinition<T> sortDefinition, string ident, Expression<Func<T, TType>> sortFunc) where T : class
        {
            var field = Helpers.GetPropertyInfo(sortFunc).Name;
            sortDefinition.AddSort(ident, orderQuery => orderQuery.OrderByScore().ThenBy(sortFunc), query => query.OrderByScore().ThenByDescending(sortFunc));
            return sortDefinition;
        }

        public static SortDefinition<T> AddSortNullable<T, TType>(this SortDefinition<T> sortDefinition, string ident, Expression<Func<T, TType>> sortFunc) where T : class
        {
            var field = Helpers.GetPropertyInfo(sortFunc).Name;
            sortDefinition.AddSort(ident,
                NullableAscCustomizer<T, TType>(field, orderQuery => orderQuery.OrderBy(sortFunc)),
                NullableAscCustomizer<T, TType>(field, orderQuery => orderQuery.OrderByDescending(sortFunc)));
            return sortDefinition;
        }

        public static SortDefinition<T> AddSortComputedNullable<T, TType>(this SortDefinition<T> sortDefinition, string ident, Expression<Func<T, TType>> sortFunc) where T : class
        {
            var field = Helpers.GetPropertyInfo(sortFunc).Name;
            sortDefinition.AddSort(ident,
                NullableAscCustomizer<T, TType>(field, orderQuery => orderQuery.OrderBy(sortFunc)),
                NullableAscCustomizer<T, TType>(field, orderQuery => orderQuery.OrderByDescending(sortFunc)));
            return sortDefinition;
        }
        public static SortDefinition<T> AddSortDateTimeNullable<T, TType>(this SortDefinition<T> sortDefinition, string ident, Expression<Func<T, TType>> sortFunc) where T : class
        {
            var field = Helpers.GetPropertyInfo(sortFunc).Name;
            sortDefinition.AddSort(ident, 
                orderQuery => orderQuery.OrderBy(sortFunc),
                NullableAscCustomizer<T, TType>(field, orderQuery => orderQuery.OrderByDescending(sortFunc)));
            return sortDefinition;
        }

        public static IQueryable<T> OrderByDescendingNullable<T, TType>(this IQueryable<T> source, Expression<Func<T, TType>> sortFunc) where T : class
        {
            var field = Helpers.GetPropertyInfo(sortFunc).Name;
            return NullableAscCustomizer<T, TType>(field, orderQuery => orderQuery.OrderByDescending(sortFunc))(source);
        }

        public static SortDefinition<T> AddSortScoreNullable<T, TType>(this SortDefinition<T> sortDefinition, string ident, Expression<Func<T, TType>> sortFunc) where T : class
        {
            var field = Helpers.GetPropertyInfo(sortFunc).Name;
            sortDefinition.AddSort(ident, 
                NullableAscCustomizer<T, TType>(field, orderQuery => orderQuery.OrderByScore().ThenBy(sortFunc)), 
                NullableAscCustomizer<T, TType>(field, orderQuery => orderQuery.OrderByScore().ThenByDescending(sortFunc)));
            return sortDefinition;
        }

        public static Func<IQueryable<T>, IQueryable<T>> NullableAscCustomizer<T, TType>(string field, Func<IQueryable<T>, IQueryable<T>> order) where T : class
        {
            return source =>
            {
                return order(((IRavenQueryable<T>)source).Customize(c => c.BeforeQueryExecuted(indexQuery =>
                {
                    // regex for query pattern
                    var match = Regex.Match(
                        indexQuery.Query,
                        @"^(.*?from\s+index\s+[\'A-z0-9]+\s*(?:as\s[\'A-z0-9]+\s*)?)(where\s*(.*?)\s*)?(order\sby\s*(.*?)\s*)?((?:\slimit\s|\sselect\s).*)$",
                        RegexOptions.Singleline);
                    if (!match.Success)
                    {
                        throw new RavenDBAccessException($"Not able to parse query for '{indexQuery.Query}'!");
                    }
                    if (!match.Groups[4].Success)
                    {
                        throw new RavenDBAccessException($"order by not included in query '{indexQuery.Query}'!");
                    }
                    // compute where condition
                    var priorityQuery = string.Format("boost({0} != null, 100) or {0} = null", field);
                    if (typeof(TType) == typeof(decimal) || typeof(TType) == typeof(decimal?) || typeof(TType) == typeof(double) || typeof(TType) == typeof(double?))
                    {
                        priorityQuery = string.Format("boost({0} between -100000000000 and 100000000000, 100) or {0} = null", field);
                    }
                    else if (typeof(TType) == typeof(float) || typeof(TType) == typeof(float?))
                    {
                        priorityQuery = string.Format("boost({0} between -100000000000 and 100000000000, 100) or {0} = null", field);
                    }
                    else if (typeof(TType) == typeof(int) || typeof(TType) == typeof(int?))
                    {
                        priorityQuery = string.Format("boost({0} between -2147483648 and 2147483647, 100) or {0} = null", field);
                    }
                    else if (typeof(TType) == typeof(long) || typeof(TType) == typeof(long?))
                    {
                        priorityQuery = string.Format("boost({0} between -100000000000 and 100000000000, 100) or {0} = null", field);
                    }
                    else if (typeof(TType) == typeof(DateTime) || typeof(TType) == typeof(DateTime?))
                    {
                        priorityQuery = string.Format("boost({0} != null, 100) or {0} = null", field);
                    }
                    var whereCondition = " where " + (match.Groups[2].Success ? $"boost({match.Groups[3].Value}, 0) AND ({priorityQuery})" : priorityQuery + " ");

                    // compute order condition
                    var orderClause = " order by " + (!match.Groups[5].Value.Contains("score()") ? "score(), " : null) + match.Groups[5].Value;
                    // use id() to not create auto index
                    indexQuery.Query = match.Groups[1].Value + whereCondition + orderClause + match.Groups[6].Value;
                })));
            };
        }
    }
}
