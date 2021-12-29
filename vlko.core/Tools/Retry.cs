using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace vlko.core.Tools
{
    public static class Retry
    {
        public static void Do(
            Action action,
            TimeSpan retryInterval,
            int retryCount = 3)
        {
            Do<object>(() =>
            {
                action();
                return null;
            }, retryInterval, retryCount);
        }

        public static void DoOnException(
            Func<Exception, bool> continueCondition,
            Action action,
            TimeSpan retryInterval,
            int retryCount = 3)
        {
            Do<object>(() =>
            {
                action();
                return Task.FromResult<object>(null);
            }, retryInterval, retryCount, continueCondition);
        }

        public static T Do<T>(
            Func<T> action,
            TimeSpan retryInterval,
            int retryCount = 3, Func<Exception, bool> continueCondition = null)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    if (retry > 0)
                    {
                        Thread.Sleep(retryInterval);
                    }
                    return action();
                }
                catch (Exception ex)
                {
                    if (continueCondition == null || continueCondition(ex))
                    {
                        exceptions.Add(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            throw new AggregateException(exceptions);
        }

        public static async Task DoOnExceptionAsync(
            Func<Exception, bool> continueCondition,
            Func<Task> action,
            TimeSpan retryInterval,
            int retryCount = 3)
        {
            await DoAsync<object>(async () =>
            {
                await action();
                return Task.FromResult<object>(null);
            }, retryInterval, retryCount, continueCondition);
        }

        public static async Task<T> DoAsync<T>(
            Func<Task<T>> action,
            TimeSpan retryInterval,
            int retryCount = 3, Func<Exception, bool> continueCondition = null)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    if (retry > 0)
                    {
                        Thread.Sleep(retryInterval);
                    }
                    return await action();
                }
                catch (Exception ex)
                {
                    if (continueCondition == null || continueCondition(ex))
                    {
                        exceptions.Add(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            throw new AggregateException(exceptions);
        }
    }
}
