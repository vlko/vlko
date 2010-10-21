using System.Linq;
using vlko.model.Repository;
using vlko.model.Repository.NH;
using vlko.model.Tests.Repository.IOCResolved.Model;

namespace vlko.model.Tests.Repository.IOCResolved.Queries
{
    public class QueryActionHotelRoomsLinq : BaseLinqQueryAction<Room>, IQueryActionHotelRooms
    {
        /// <summary>
        /// Wheres the name of the hotel.
        /// </summary>
        /// <param name="hotelName">Name of the hotel.</param>
        /// <returns>Room result filtered with hotel name.</returns>
        public IQueryResult<Room> WhereHotelName(string hotelName)
        {
			return Result(Queryable.Where(room => room.Hotel.Name == hotelName));
        }
    }
}
