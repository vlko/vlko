using System;
using System.ComponentModel.DataAnnotations;

namespace vlko.core.Models.Action.ViewModel
{
    public class StaticTextViewModel
    {
                /// <summary>
        /// Initializes a new instance of the <see cref="StaticTextViewModel"/> class.
        /// </summary>
        public StaticTextViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticTextViewModel"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="friendlyUrl">The friendly URL.</param>
        /// <param name="title">The title.</param>
        /// <param name="text">The text.</param>
        /// <param name="creator">The creator.</param>
        /// <param name="changeDate">The change date.</param>
        /// <param name="publishDate">The publish date.</param>
        /// <param name="allowComments">if set to <c>true</c> [allow comments].</param>
        /// <param name="commentCounts">The comment counts.</param>
        public StaticTextViewModel(Guid id, string friendlyUrl, string title, string text, User creator, DateTime changeDate, DateTime publishDate, bool allowComments, int commentCounts)
        {
            Id = id;
            FriendlyUrl = friendlyUrl;
            Title = title;
            Text = text;
            Creator = creator;
            ChangeDate = changeDate;
            PublishDate = publishDate;
            AllowComments = allowComments;
            CommentCounts = commentCounts;
        }

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
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; }

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