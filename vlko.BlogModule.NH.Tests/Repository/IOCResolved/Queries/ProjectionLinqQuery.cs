using System.Linq;
using vlko.BlogModule.NH.Tests.Repository.IOCResolved.Model;
using vlko.core.NH.Repository;
using vlko.core.Repository;

namespace vlko.BlogModule.NH.Tests.Repository.IOCResolved.Queries
{
    public class ProjectionLinqQuery : LinqQuery<Room>, IProjectionQuery
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
