using System;
using vlko.BlogModule.Roots;

namespace vlko.BlogModule.Action.ViewModel
{
	public class CommentViewModel
	{

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the created date.
		/// </summary>
		/// <value>The created date.</value>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		public string Text { get; set; }

		/// <summary>
		/// Gets or sets the version.
		/// </summary>
		/// <value>The version.</value>
		public int Version { get; set;}

		/// <summary>
		/// Gets a value indicating whether this <see cref="CommentViewModel"/> is changed.
		/// </summary>
		/// <value><c>true</c> if changed; otherwise, <c>false</c>.</value>
		public bool Changed { 
			get
			{
				return Version != 0;	
			} 
		}

		/// <summary>
		/// Gets or sets the owner.
		/// </summary>
		/// <value>The owner.</value>
		public User Owner { get; set; }

		/// <summary>
		/// Gets or sets the name of the anonymous user.
		/// </summary>
		/// <value>The name of the anonymous user.</value>
		public string AnonymousName { get; set; }

		/// <summary>
		/// Gets or sets the client ip.
		/// </summary>
		/// <value>The client ip.</value>
		public string ClientIp { get; set; }

		/// <summary>
		/// Gets or sets the level.
		/// </summary>
		/// <value>The level.</value>
		public int Level { get; set; }
	}
}