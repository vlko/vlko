﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using vlko.BlogModule.Commands;
using vlko.BlogModule.Commands.ComplexHelpers.Twitter;
using vlko.BlogModule.Roots;
using vlko.BlogModule.Search;
using vlko.core.Base.Scheduler;
using vlko.core.InversionOfControl;
using vlko.core.Repository;

namespace vlko.BlogModule.Base.Scheduler
{
	public class UpdateTwitterTask : SchedulerTask
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateTwitterTask"/> class.
		/// </summary>
		/// <param name="callIntervalInMinutes">The call interval in minutes.</param>
		/// <param name="startImmediately">if set to <c>true</c> [start immediately].</param>
		public UpdateTwitterTask(int callIntervalInMinutes, bool startImmediately)
			: base(callIntervalInMinutes, startImmediately)
		{
		}

		/// <summary>
		/// Does the job.
		/// </summary>
		protected override void DoJob()
		{
			// actions declaration
			var twitterConnection = RepositoryFactory.Command<ITwitterConnection>();
			var twitterData = RepositoryFactory.Command<ITwitterStatusCommands>();
			var searchAction = RepositoryFactory.Command<ISearchCommands>();

			// token declaration
			var oAuthToken = new OAuthToken
			                 	{
			                 		ConsumerKey = Settings.Twitter.ConsumerKey.Value,
			                 		ConsumerSecret = Settings.Twitter.ConsumerSecret.Value,
			                 		Token = Settings.Twitter.Token.Value,
			                 		TokenSecret = Settings.Twitter.TokenSecret.Value
			                 	};
			var twitterUser = Settings.Twitter.TwitterAccount.Value;

			// check if token is not valid log
			if (!twitterConnection.IsTokenValid(oAuthToken))
			{
				Logger.Fatal("Twitter OAuthToken is not valid go to /Admin/Twitter/Authorize to register new one!!");
				return;
			}


			bool proceedNextPage = false;
			int currentPage = 0;
			var statusToStore = new List<TwitterStatus>();
			do
			{

				var items = twitterConnection.GetStatusesForUser(oAuthToken, twitterUser, currentPage, 100);

				using (RepositoryFactory.StartUnitOfWork())
				{
					var storedItems = twitterData.GetByTwitterIds(items.Select(status => status.TwitterId));


					// add if item not yet stored
					foreach (var twitterStatus in items)
					{
						if (!storedItems.Any(item => item.TwitterId == twitterStatus.TwitterId)
						    && !statusToStore.Any(item => item.TwitterId == twitterStatus.TwitterId))
						{
							statusToStore.Add(twitterStatus);
						}
					}
					++currentPage;
					proceedNextPage = items.Length > 0 && storedItems.Length == 0;
				}

			} while (proceedNextPage);

			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				foreach (var twitterStatus in statusToStore.OrderByDescending(item => item.TwitterId))
				{
					twitterStatus.Modified = twitterStatus.CreatedDate;
					twitterStatus.CreatedBy = null;
					twitterStatus.PublishDate = twitterStatus.CreatedDate;
					twitterStatus.AreCommentAllowed = false;
					searchAction.IndexTwitterStatus(tran,
					                                twitterData.CreateStatus(twitterStatus));

				}
				tran.Commit();
			}
			Logger.Debug("There were '{0}' new statuses to store for {1}", statusToStore.Count, twitterUser);

		}
	}
}
