using System.Web.Mvc;
using vlko.BlogModule.Base.Scheduler;
using vlko.core.Base.Scheduler;
using vlko.core.Components;

namespace vlko.BlogModule
{
	public static class ApplicationInit
	{

		/// <summary>
		/// Do init part in one step and right order.
		/// </summary>
		public static void FullInit()
		{
			RegisterBinders();
			InitializeScheduler();
		}

		/// <summary>
		/// Registers the binders.
		/// </summary>
		public static void RegisterBinders()
		{
			ModelBinders.Binders.DefaultBinder = new ExtendedModelBinder();
		}

		/// <summary>
		/// Initializes the scheduler.
		/// </summary>
		public static void InitializeScheduler()
		{
			var scheduler = new Scheduler(new SchedulerTask[]
			                              	{
			                              		new KeepAliveTask(5, true),
												new UpdateTwitterTask(10, true),
												new UpdateRssFeedsTask(10, true),
			                              	}, 20);
			scheduler.Start();
		}
	}
}
