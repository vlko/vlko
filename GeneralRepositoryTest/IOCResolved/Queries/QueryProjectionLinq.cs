using System.Linq;
using GeneralRepositoryTest.IOCResolved.Model;
using GenericRepository;
using vlko.core.ActiveRecord;

namespace GeneralRepositoryTest.IOCResolved.Queries
{
    public class QueryProjectionLinq : BaseLinqQuery<Room>, IQueryProjection
    {
        public GenericRepository.IQueryResult<RoomWithHotelProjection> DoProjection()
        {
            return
                new QueryLinqResult<RoomWithHotelProjection>(
                    Queryable.Select(
                        room => new RoomWithHotelProjection {HotelName = room.Hotel.Name, RoomName = room.Name}));
        }

    }
}
