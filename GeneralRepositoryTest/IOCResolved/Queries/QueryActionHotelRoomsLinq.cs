using System.Linq;
using GeneralRepositoryTest.IOCResolved.Model;
using vlko.core.ActiveRecord;

namespace GeneralRepositoryTest.IOCResolved.Queries
{
    public class QueryActionHotelRoomsLinq : BaseLinqQueryAction<Room>, IQueryActionHotelRooms
    {
        /// <summary>
        /// Wheres the name of the hotel.
        /// </summary>
        /// <param name="hotelName">Name of the hotel.</param>
        /// <returns>Room result filtered with hotel name.</returns>
        public GenericRepository.IQueryResult<Room> WhereHotelName(string hotelName)
        {
			return Result(Queryable.Where(room => room.Hotel.Name == hotelName));
        }
    }
}
