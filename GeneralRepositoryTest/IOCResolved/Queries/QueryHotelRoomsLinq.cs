using System.Linq;
using GeneralRepositoryTest.IOCResolved.Model;
using vlko.core.ActiveRecord;

namespace GeneralRepositoryTest.IOCResolved.Queries
{
    public class QueryHotelRoomsLinq : BaseLinqQuery<Room>, IQueryHotelRooms
    {
        /// <summary>
        /// Wheres the name of the hotel.
        /// </summary>
        /// <param name="hotelName">Name of the hotel.</param>
        /// <returns>Room result filtered with hotel name.</returns>
        public GenericRepository.IQueryResult<Room> WhereHotelName(string hotelName)
        {
            Queryable = Queryable.Where(room => room.Hotel.Name == hotelName);
            return Result();
        }
    }
}
