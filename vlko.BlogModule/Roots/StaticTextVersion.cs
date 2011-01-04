using System;

namespace vlko.BlogModule.Roots
{
    public class StaticTextVersion
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public virtual int Version { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public virtual string Text { get; set; }

        /// <summary>
        /// Gets or sets the creator.
        /// </summary>
        /// <value>The creator.</value>
        public virtual User CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>The created date.</value>
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the static text.
        /// </summary>
        /// <value>The static text.</value>
        public virtual StaticText StaticText { get; set; }
    }
}


