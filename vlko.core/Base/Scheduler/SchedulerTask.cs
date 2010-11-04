using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using NLog;

namespace vlko.core.Base.Scheduler
{
	public abstract class SchedulerTask
	{
		
		public DateTime NextRun { get; private set; }
		public int CallIntervalInMinutes { get; private set; }

		private readonly Logger _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="SchedulerTask"/> class.
		/// </summary>
		/// <param name="callIntervalInMinutes">The call interval in minutes.</param>
		/// <param name="startImmediately">if set to <c>true</c> [start immediately].</param>
		public SchedulerTask(int callIntervalInMinutes, bool startImmediately = true)
		{
			CallIntervalInMinutes = callIntervalInMinutes;
			NextRun = DateTime.Now.AddMinutes(startImmediately ? 0 : callIntervalInMinutes);
			_logger = LogManager.GetLogger("SchedulerTask");
		}

		/// <summary>
		/// Gets the logger.
		/// </summary>
		/// <value>The logger.</value>
		protected virtual Logger Logger
		{
			get { return _logger; }
		}

		/// <summary>
		/// Executes this instance.
		/// </summary>
		internal void Execute()
		{
			Logger.Trace("Starting scheduler job: " + this.GetType().Name);
			try
			{
				DoJob();
			}
			catch (Exception e)
			{
				Logger.ErrorException("Scheduler task failed: " + e.Message, e);
			}
			NextRun = DateTime.Now.AddMinutes(CallIntervalInMinutes);
		}

		/// <summary>
		/// Does the job.
		/// </summary>
		protected abstract void DoJob();
	}
}
