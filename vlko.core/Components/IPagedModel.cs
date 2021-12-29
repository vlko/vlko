namespace vlko.core.Components
{
    public interface IPagedModel
    {
        /// <summary>
        /// Gets or sets the page items.
        /// </summary>
        /// <value>The page items.</value>
        int PageSize { get; set; }

        /// <summary>
        /// Gets the allowed page sizes.
        /// </summary>
        /// <value>The allowed page sizes.</value>
        int[] AllowedPageSizes { get; }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>The current page.</value>
        int CurrentPage { get; set; }

        /// <summary>
        /// Gets the total count.
        /// </summary>
        /// <value>The total count.</value>
        int Count { get; }

        /// <summary>
        /// Gets the pages number.
        /// </summary>
        /// <value>The pages number.</value>
        int PagesNumber { get; }
    }
}