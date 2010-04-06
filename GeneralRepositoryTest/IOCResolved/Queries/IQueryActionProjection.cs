using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralRepositoryTest.IOCResolved.Model;
using GenericRepository;

namespace GeneralRepositoryTest.IOCResolved.Queries
{
    public interface IQueryActionProjection : IQueryAction<Room>
    {

        /// <summary>
        /// Does the projection.
        /// </summary>
        /// <returns>Room with hotel projection.</returns>
        IQueryResult<RoomWithHotelProjection> DoProjection();
    }
}
