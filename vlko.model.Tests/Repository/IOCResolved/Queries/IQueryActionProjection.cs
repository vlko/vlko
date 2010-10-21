using vlko.model.Repository;
using vlko.model.Tests.Repository.IOCResolved.Model;

namespace vlko.model.Tests.Repository.IOCResolved.Queries
{
    public interface IQueryActionProjection : IQueryAction<Room>
    {

        /// <summary>
        /// Does the projection.
        /// </summary>
        /// <returns>Room with hotel projection.</returns>
        IQueryResult<RoomWithHotelProjection> DoProjection();
    }
}
