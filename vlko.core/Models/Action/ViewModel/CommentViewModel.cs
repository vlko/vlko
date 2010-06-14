using System;

namespace vlko.core.Models.Action.ViewModel
{
	public class CommentViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CommentViewModel"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="name">The name.</param>
		/// <param name="createdDate">The created date.</param>
		/// <param name="text">The text.</param>
		/// <param name="version">The version.</param>
		/// <param name="owner">The owner.</param>
		/// <param name="anonymousName">Name of the anonymous.</param>
		/// <param name="clientIp">The client ip.</param>
		/// <param name="level">The level.</param>
		public CommentViewModel(Guid id, string name, DateTime createdDate, string text, int version, User owner, string anonymousName, string clientIp, int level)
		{
			Id = id;
			Name = name;
			CreatedDate = createdDate;
			Text = text;
			Version = version;
			Owner = owner;
			AnonymousName = anonymousName;
			ClientIp = clientIp;
			Level = level;
		}

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