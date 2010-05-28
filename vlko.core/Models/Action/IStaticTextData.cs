using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericRepository;
using vlko.core.Models.Action.ViewModel;

namespace vlko.core.Models.Action
{
    public interface IStaticTextData : IAction<StaticText>
    {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>Query result.</returns>
        IQueryResult<StaticTextViewModel> GetAll();

        /// <summary>
        /// Gets the deleted.
        /// </summary>
        /// <returns>Query result.</returns>
        IQueryResult<StaticTextViewModel> GetDeleted();

        /// <summary>
        /// Gets the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Data model for specific id.</returns>
        StaticTextViewModel Get(Guid id);

        /// <summary>
        /// Gets the specified nice URL.
        /// </summary>
        /// <param name="friendlyUrl">The friendly URL.</param>
        /// <returns>Data model for</returns>
        StaticTextViewModel Get(string friendlyUrl);
    }
}
