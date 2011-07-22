using vlko.core.NH.Repository;
using vlko.core.Repository;
using vlko.BlogModule.Tests.Repository.IOCResolved.Model;

namespace vlko.BlogModule.Tests.Repository.IOCResolved.Queries
{
	public class QueryActionHotelRoomsCriterion : BaseCriterionQueryAction<Room>, IQueryActionHotelRooms
	{
		/// <summary>
		/// Wheres the name of the hotel.
		/// </summary>
		/// <param name="hotelName">Name of the hotel.</param>
		/// <returns>Room result filtered with hotel name.</returns>
		public IQueryResult<Room> WhereHotelName(string hotelName)
		{
			Hotel Hotel = null;
			Criteria = Criteria
				.JoinAlias(room => room.Hotel, () => Hotel)
				.Where(room => Hotel.Name == hotelName)
				.Fetch(room => room.Hotel).Lazy;
			return Result();
		}
	}
}
