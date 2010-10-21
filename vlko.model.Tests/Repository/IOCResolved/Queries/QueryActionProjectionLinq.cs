using System.Linq;
using vlko.model.Repository;
using vlko.model.Repository.NH;
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
