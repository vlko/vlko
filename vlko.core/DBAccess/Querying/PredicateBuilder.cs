using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace System.Linq
{
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> NullExpression<T>() { return null; }
        public static Expression<Func<T, bool>> True<T>() { return f => true; }
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                            Expression<Func<T, bool>> expr2)
        {
            if (expr1 != null)
            {
                if (expr2 != null)
                {
                    return Expression.Lambda<Func<T, bool>>
                        (Expression.OrElse(expr1.Body, expr2.Body), expr1.Parameters);
                }
                return expr1;
            }
            return expr2;
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                             Expression<Func<T, bool>> expr2)
        {
            if (expr1 != null)
            {
                if (expr2 != null)
                {
                    return Expression.Lambda<Func<T, bool>>
                        (Expression.AndAlso(expr1.Body, expr2.Body), expr1.Parameters);
                }
                return expr1;
            }
            return expr2;
        }
    }
}
