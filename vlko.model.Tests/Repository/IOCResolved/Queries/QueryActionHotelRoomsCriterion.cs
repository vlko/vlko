using NHibernate;
using NHibernate.LambdaExtensions;
using vlko.core.Repository;
using vlko.model.Implementation.NH.Repository;
using vlko.model.Tests.Repository.IOCResolved.Model;

namespace vlko.model.Tests.Repository.IOCResolved.Queries
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
            Criteria
                .CreateAlias<Room>(room => room.Hotel, () => Hotel)
                .SetFetchMode<Room>(room => room.Hotel, FetchMode.Lazy)
                .Add<Room>(room => room.Hotel.Name == hotelName);
            return Result();
        }
    }
}
