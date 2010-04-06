using System;

namespace vlko.core.Models.Action.ActionModel
{
    public class StaticTextActionModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticTextActionModel"/> class.
        /// </summary>
        public StaticTextActionModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticTextActionModel"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="title">The title.</param>
        /// <param name="text">The text.</param>
        /// <param name="creator">The creator.</param>
        /// <param name="changeDate">The change date.</param>
        /// <param name="publishDate">The publish date.</param>
        /// <param name="allowComments">if set to <c>true</c> [allow comments].</param>
        public StaticTextActionModel(Guid id, string title, string text, User creator, DateTime changeDate, DateTime publishDate, bool allowComments)
        {
            Id = id;
            Title = title;
            Text = text;
            Creator = creator;
            ChangeDate = changeDate;
            PublishDate = publishDate;
            AllowComments = allowComments;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public Guid Id { get; set; }

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
    }
}
