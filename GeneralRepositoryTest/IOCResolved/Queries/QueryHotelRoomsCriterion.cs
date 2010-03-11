using NHibernate.LambdaExtensions;
using GeneralRepositoryTest.IOCResolved.Model;
using NHibernate;
using vlko.model.ActiveRecord;

namespace GeneralRepositoryTest.IOCResolved.Queries
{
    public class QueryHotelRoomsCriterion : BaseCriterionQuery<Room>, IQueryHotelRooms
    {
        /// <summary>
        /// Wheres the name of the hotel.
        /// </summary>
        /// <param name="hotelName">Name of the hotel.</param>
        /// <returns>Room result filtered with hotel name.</returns>
        public GenericRepository.IQueryResult<Room> WhereHotelName(string hotelName)
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
