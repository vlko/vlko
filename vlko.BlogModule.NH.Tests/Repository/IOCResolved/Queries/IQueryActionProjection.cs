using vlko.BlogModule.NH.Tests.Repository.IOCResolved.Model;
using vlko.core.Repository;

namespace vlko.BlogModule.NH.Tests.Repository.IOCResolved.Queries
{
	public interface IQueryActionProjection : IAction<Room>
    {

        /// <summary>
        /// Does the projection.
        /// </summary>
        /// <returns>Room with hotel projection.</returns>
        IQueryResult<RoomWithHotelProjection> DoProjection();
    }
}
