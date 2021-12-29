using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using vlko.core.InversionOfControl;
using vlko.core.web.Scheduler;
using vlko.core.web.Services;

namespace vlko.core.web.Scheduler
{
    public class KeepAliveTask : SchedulerTask
    {
        private static HttpClient _requestClient = new HttpClient();
        private Lazy<string> _aliveUrl;
        /// <summary>
        /// Initializes a new instance of the <see cref="KeepAliveTask"/> class.
        /// </summary>
        /// <param name="callIntervalInMinutes">The call interval in minutes.</param>
        /// <param name="startImmediately">if set to <c>true</c> [start immediately].</param>
        public KeepAliveTask(int callIntervalInMinutes, bool startImmediately)
            : base(callIntervalInMinutes, startImmediately)
        {
            _aliveUrl = new Lazy<string>(GetAliveUrl);
        }

        /// <summary>
        /// Gets the alive URL.
        /// </summary>
        /// <returns>Url to keep app upp and running</returns>
        private static string GetAliveUrl()
        {
            return IoC.Scope.Resolve<IAppInfoService>().RootUrl;
        }


        /// <summary>
        /// Does the job.
        /// </summary>
        protected override void DoJob()
        {
            try
            {
                var aliveRequestContent = _requestClient.GetStringAsync(_aliveUrl.Value).Result;
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to do keep alive request to {_aliveUrl.Value}", e);
            }
            Logger.Debug("Alive request to '{0}' completed succesfully", _aliveUrl.Value);
        }
    }
}
