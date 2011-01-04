using System.Linq;
using vlko.BlogModule.NH.Repository;
using vlko.core.Repository;
using vlko.BlogModule.Tests.Repository.IOCResolved.Model;

namespace vlko.BlogModule.Tests.Repository.IOCResolved.Queries
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
