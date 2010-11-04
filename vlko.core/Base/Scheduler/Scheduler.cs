using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using NLog;

namespace vlko.core.Base.Scheduler
{
	public class Scheduler
	{
		private const string CacheIdent = "Unique??SchedulerCacheIdent";
		private readonly SchedulerTask[] _tasks;
		private readonly int _internalCheckInterval;
		private readonly Cache _cache;
		private Logger _logger;
		public DateTime LastRun { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Scheduler"/> class.
		/// </summary>
		/// <param name="tasks">The tasks.</param>
		/// <param name="internalCheckInterval">The internal check interval in minutes.</param>
		public Scheduler(SchedulerTask[] tasks, int internalCheckInterval = 2)
		{
			_tasks = tasks;
			_internalCheckInterval = internalCheckInterval;
			_cache = HttpRuntime.Cache;
			_logger = LogManager.GetLogger("SchedulerTask");
			_logger.Trace("Scheduler created.");
		}

		/// <summary>
		/// Starts this instance.
		/// </summary>
		public void Start()
		{
			InvokeTasks();
			Schedule();
		}

		/// <summary>
		/// Starts the scheduler.
		/// </summary>
		private void Schedule()
		{
			if (_cache[CacheIdent] == null)
			{
				_cache.Add(CacheIdent, this, null,
					 DateTime.Now.AddMinutes(_internalCheckInterval), Cache.NoSlidingExpiration,
					 CacheItemPriority.NotRemovable, OnCacheCallback);
			}
		}

		/// <summary>
		/// Invokes the tasks.
		/// </summary>
		public void InvokeTasks()
		{
			_logger.Trace("Scheduler invoke tasks.");
			foreach (var schedulerTask in _tasks)
			{
				if (schedulerTask.NextRun <= DateTime.Now)
				{
					schedulerTask.Execute();
				}
			}
		}

		/// <summary>
		/// Called when [cache callback].
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <param name="reason">The reason.</param>
		private static void OnCacheCallback(string key, object value, CacheItemRemovedReason reason)
		{
			Scheduler scheduler = (Scheduler)value;
			if (scheduler.LastRun < DateTime.Now)
			{
				scheduler.InvokeTasks();
				scheduler.LastRun = DateTime.Now;
			}
			scheduler.Schedule();
		}
	}
}
