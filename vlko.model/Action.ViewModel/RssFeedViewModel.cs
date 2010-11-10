using System;
using System.ComponentModel.DataAnnotations;

namespace vlko.model.Action.ViewModel
{
	public class RssFeedViewModel
	{

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		[Key]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the friendly URL.
		/// </summary>
		/// <value>The friendly URL.</value>
		public string FeedUrl { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [display full content].
		/// </summary>
		/// <value><c>true</c> if [display full content]; otherwise, <c>false</c>.</value>
		public bool DisplayFullContent { get; set; }

		/// <summary>
		/// Gets or sets the feed item count.
		/// </summary>
		/// <value>The feed item count.</value>
		public int FeedItemCount { get; set; }
	}
}