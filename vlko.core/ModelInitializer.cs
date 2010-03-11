using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using vlko.core.Models;

namespace vlko.core
{
    public static class ModelInitializer
    {
        /// <summary>
        /// Lists the of model types.
        /// </summary>
        /// <returns>List of model types.</returns>
        public static Type[] ListOfModelTypes()
        {
            return new[]
                       {
                           typeof(Content),
                           typeof(User),
                           typeof(Comment),
                           typeof(CommentVersion),
                           typeof(StaticText),
                           typeof(StaticTextVersion),
                           typeof(RssFeed),
                           typeof(RssItem)
                       };
        }
    }
}
