using System;
using System.ComponentModel.DataAnnotations;
using vlko.core.ValidationAtribute;


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
        /// <param name="friendlyUrl">The friendly URL.</param>
        /// <param name="title">The title.</param>
        /// <param name="text">The text.</param>
        /// <param name="creator">The creator.</param>
        /// <param name="changeDate">The change date.</param>
        /// <param name="publishDate">The publish date.</param>
        /// <param name="allowComments">if set to <c>true</c> [allow comments].</param>
        public StaticTextActionModel(Guid id, string friendlyUrl, string title, string text, User creator, DateTime changeDate, DateTime publishDate, bool allowComments)
        {
            Id = id;
            FriendlyUrl = friendlyUrl;
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
        [Key]
        [Editable(false)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the friendly URL (if new then auto-generated).
        /// </summary>
        /// <value>The friendly URL.</value>
        [Display(ResourceType = typeof(ModelResources), Name = "FriendlyUrl")]
        [StringLength(80, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FriendlyURLError")]
        public string FriendlyUrl { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [Display(ResourceType = typeof(ModelResources), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TitleRequireError")]
        [StringLength(80, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TitleLengthError")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [Display(ResourceType = typeof(ModelResources), Name = "Text")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TextRequireError")]
        [DataType(DataType.Html)]
        [AntiXssHtmlText]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the creator.
        /// </summary>
        /// <value>The creator.</value>
        [Editable(false)]
        public User Creator { get; set; }

        /// <summary>
        /// Gets or sets the current date.
        /// </summary>
        /// <value>The current date.</value>
        [Editable(false)]
        [Display(ResourceType = typeof(ModelResources), Name = "ChangeDate")]
        public DateTime ChangeDate { get; set; }

        /// <summary>
        /// Gets or sets the publish date.
        /// </summary>
        /// <value>The publish date.</value>
        [Display(ResourceType = typeof(ModelResources), Name = "PublishDate")]
        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PublishDateRequireError")]
        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow comments].
        /// </summary>
        /// <value><c>true</c> if [allow comments]; otherwise, <c>false</c>.</value>
        [Display(ResourceType = typeof(ModelResources), Name = "AllowComments")]
        public bool AllowComments { get; set; }
    }
}
