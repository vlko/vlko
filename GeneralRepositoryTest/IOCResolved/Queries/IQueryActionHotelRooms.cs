using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericRepository;
using GeneralRepositoryTest.IOCResolved.Model;

namespace GeneralRepositoryTest.IOCResolved.Queries
{
    public interface IQueryActionHotelRooms : IQueryAction<Room>
    {
        /// <summary>
        /// Wheres the name of the hotel.
        /// </summary>
        /// <param name="hotelName">Name of the hotel.</param>
        /// <returns>Room result filtered with hotel name.</returns>
        IQueryResult<Room> WhereHotelName(string hotelName);
    }
}
