using System;

namespace vlko.BlogModule.Action.ViewModel
{
	public class RssItemViewModelWithId : RssItemViewModel
	{
		/// <summary>
		/// Gets or sets the internal id.
		/// </summary>
		/// <value>The internal id.</value>
		public Guid Id { get; set; }
	}
}