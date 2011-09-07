using System;
using vlko.BlogModule.NH.Tests.Repository.IOCResolved.Model;
using vlko.core.Repository;

namespace vlko.BlogModule.NH.Tests.Repository.IOCResolved.Queries
{
	public interface IReservationForDayQuery : ICommandGroup<Reservation>
    {
        /// <summary>
        /// Wheres the date.
        /// </summary>
        /// <param name="reservationDate">The reservation date.</param>
        /// <returns>List of reservations with resolved room relation.</returns>
        IQueryResult<Reservation> WhereDate(DateTime reservationDate);
    }
}
