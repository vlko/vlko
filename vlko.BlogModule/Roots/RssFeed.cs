using System;
using System.Collections.Generic;

namespace vlko.BlogModule.Roots
{
	public class RssFeed
	{
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		public virtual Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public virtual string Name { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>The URL.</value>
		public virtual string Url { get; set; }

		/// <summary>
		/// Gets or sets the author regex.
		/// </summary>
		/// <value>The author regex.</value>
		public virtual string AuthorRegex { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [get direct content].
		/// </summary>
		/// <value><c>true</c> if [get direct content]; otherwise, <c>false</c>.</value>
		public virtual bool GetDirectContent { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [display full content].
		/// </summary>
		/// <value><c>true</c> if [display full content]; otherwise, <c>false</c>.</value>
		public virtual bool DisplayFullContent { get; set; }

		/// <summary>
		/// Gets or sets the content parse regex.
		/// </summary>
		/// <value>The content parse regex.</value>
		public virtual string ContentParseRegex { get; set; }

		/// <summary>
		/// Gets or sets the RSS items.
		/// </summary>
		/// <value>The RSS items.</value>
		public virtual IList<RssItem> RssItems { get; set; }

	}
}


