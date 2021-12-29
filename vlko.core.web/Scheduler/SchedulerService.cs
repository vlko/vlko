using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace vlko.core.web.Scheduler
{
    public class SchedulerService : IHostedService
    {
        private readonly SchedulerTask[] _tasks;
        private readonly int _internalCheckInterval;
        private Timer _timer;
        private Logger _logger;
        public DateTime LastRun { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerService"/> class.
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        /// <param name="internalCheckInterval">The internal check interval (in seconds).</param>
        public SchedulerService(SchedulerTask[] tasks, int internalCheckInterval = 120)
        {
            _tasks = tasks;
            _internalCheckInterval = internalCheckInterval;
            _logger = LogManager.GetLogger("SchedulerTask");
            _logger.Trace("Scheduler created.");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Timed Background Service is starting.");

            _timer = new Timer(ExecuteTasks, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(_internalCheckInterval));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Executes all tasks.
        /// </summary>
        public void ExecuteTasks(object state)
        {
            // we trust to this and so we can skip logging
            // _logger.Trace("Scheduler invoke tasks.");
            foreach (var schedulerTask in _tasks)
            {
                if (!schedulerTask.Running && schedulerTask.NextRun <= DateTime.UtcNow)
                {
                    schedulerTask.Execute();
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
