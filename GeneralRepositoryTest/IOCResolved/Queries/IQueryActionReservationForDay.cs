using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralRepositoryTest.IOCResolved.Model;
using GenericRepository;

namespace GeneralRepositoryTest.IOCResolved.Queries
{
    public interface IQueryActionReservationForDay : IQueryAction<Reservation>
    {
        /// <summary>
        /// Wheres the date.
        /// </summary>
        /// <param name="reservationDate">The reservation date.</param>
        /// <returns>List of reservations with resolved room relation.</returns>
        IQueryResult<Reservation> WhereDate(DateTime reservationDate);
    }
}
