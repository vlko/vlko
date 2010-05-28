using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;

namespace vlko.core.Tools
{
    public class InstanceCreator<T> where T : new()
    {
        private static readonly Func<T> LambdaConstruction = Expression.Lambda<Func<T>>(
           Expression.New(typeof(T)), null).Compile();

        public static T Create()
        {
            return LambdaConstruction();
        }
    }

    public class InstanceCreator
    {
        private static readonly Dictionary<Type, Func<object>> ConstructorCache = new Dictionary<Type, Func<object>>();

        private static ReaderWriterLockSlim CacheLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Creates the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Instance of object.</returns>
        public static object Create(Type type)
        {
            Func<object> lambdaConstructor = null;

            CacheLock.EnterReadLock();
            if (ConstructorCache.ContainsKey(type))
            {
                lambdaConstructor = ConstructorCache[type];
            }
            CacheLock.ExitReadLock();
            
            if (lambdaConstructor == null)
            {
                CacheLock.EnterWriteLock();
                if (!ConstructorCache.ContainsKey(type))
                {
                    ConstructorCache[type] = Expression.Lambda<Func<object>>(
                        Expression.New(type), null).Compile();
                }
                lambdaConstructor = ConstructorCache[type];
                CacheLock.ExitWriteLock();
            }

            return lambdaConstructor();
        }
    }
}
