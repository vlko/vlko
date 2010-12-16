using System;
using System.Collections.Generic;

namespace vlko.model.Roots
{
    public class StaticText : Content
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticText"/> class.
        /// </summary>
        public StaticText()
        {
            ContentType = ContentType.StaticText;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the friendly URL.
        /// </summary>
        /// <value>The friendly URL.</value>
        public virtual string FriendlyUrl { get; set; }

        /// <summary>
        /// Gets or sets the actual version.
        /// </summary>
        /// <value>The actual version.</value>
        public virtual int ActualVersion { get; set; }

        /// <summary>
        /// Gets or sets all static text versions.
        /// </summary>
        /// <value>All static text versions.</value>
        public virtual IList<StaticTextVersion> StaticTextVersions { get; set; }
    }
}


