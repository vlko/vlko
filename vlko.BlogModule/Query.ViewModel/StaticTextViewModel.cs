using System;
using System.ComponentModel.DataAnnotations;
using vlko.BlogModule.Roots;
using vlko.core.Roots;

namespace vlko.BlogModule.Action.ViewModel
{
	public class StaticTextViewModel
	{

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		[Key]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the friendly URL.
		/// </summary>
		/// <value>The friendly URL.</value>
		public string FriendlyUrl { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the creator.
		/// </summary>
		/// <value>The creator.</value>
		public User Creator { get; set; }

		/// <summary>
		/// Gets or sets the current date.
		/// </summary>
		/// <value>The current date.</value>
		public DateTime ChangeDate { get; set; }

		/// <summary>
		/// Gets or sets the publish date.
		/// </summary>
		/// <value>The publish date.</value>
		public DateTime PublishDate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [allow comments].
		/// </summary>
		/// <value><c>true</c> if [allow comments]; otherwise, <c>false</c>.</value>
		public bool AllowComments { get; set; }

		/// <summary>
		/// Gets or sets the comment counts.
		/// </summary>
		/// <value>The comment counts.</value>
		public int CommentCounts { get; set; }
	}
}