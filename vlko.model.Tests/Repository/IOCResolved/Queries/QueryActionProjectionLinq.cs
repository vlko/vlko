using System.Linq;
using vlko.core.Repository;
using vlko.model.Implementation.NH.Repository;
using vlko.model.Tests.Repository.IOCResolved.Model;

namespace vlko.model.Tests.Repository.IOCResolved.Queries
{
    public class QueryActionProjectionLinq : BaseLinqQueryAction<Room>, IQueryActionProjection
    {
        public IQueryResult<RoomWithHotelProjection> DoProjection()
        {
            return
                new QueryLinqResult<RoomWithHotelProjection>(
                    Queryable.Select(
                        room => new RoomWithHotelProjection {HotelName = room.Hotel.Name, RoomName = room.Name}));
        }

    }
}
