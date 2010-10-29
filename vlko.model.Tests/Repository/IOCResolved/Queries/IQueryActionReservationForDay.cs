using System;
using vlko.model.Repository;
using vlko.model.Tests.Repository.IOCResolved.Model;

namespace vlko.model.Tests.Repository.IOCResolved.Queries
{
	public interface IQueryActionReservationForDay : IAction<Reservation>
    {
        /// <summary>
        /// Wheres the date.
        /// </summary>
        /// <param name="reservationDate">The reservation date.</param>
        /// <returns>List of reservations with resolved room relation.</returns>
        IQueryResult<Reservation> WhereDate(DateTime reservationDate);
    }
}
