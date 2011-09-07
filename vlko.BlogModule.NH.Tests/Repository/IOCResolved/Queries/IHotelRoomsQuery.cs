using vlko.BlogModule.NH.Tests.Repository.IOCResolved.Model;
using vlko.core.Repository;

namespace vlko.BlogModule.NH.Tests.Repository.IOCResolved.Queries
{
	public interface IHotelRoomsQuery : ICommandGroup<Room>
    {
        /// <summary>
        /// Wheres the name of the hotel.
        /// </summary>
        /// <param name="hotelName">Name of the hotel.</param>
        /// <returns>Room result filtered with hotel name.</returns>
        IQueryResult<Room> WhereHotelName(string hotelName);
    }
}
