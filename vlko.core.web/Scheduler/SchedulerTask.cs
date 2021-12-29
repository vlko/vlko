using NLog;
using System;
using System.Threading.Tasks;

namespace vlko.core.web.Scheduler
{
    public abstract class SchedulerTask
    {

        public DateTime NextRun { get; private set; }
        public bool Running { get; private set; }
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
            NextRun = DateTime.UtcNow.AddMinutes(startImmediately ? 0 : callIntervalInMinutes);
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
            Logger.Trace("Starting scheduler job: " + GetType().Name);
            Running = true;
            try
            {
                DoJob();
            }
            catch (Exception e)
            {
                Logger.Error(e, "Scheduler task failed: " + e.Message);
            }
            Running = false;
            NextRun = DateTime.UtcNow.AddMinutes(CallIntervalInMinutes);
        }

        /// <summary>
        /// Does the job.
        /// </summary>
        protected abstract void DoJob();
    }
}
