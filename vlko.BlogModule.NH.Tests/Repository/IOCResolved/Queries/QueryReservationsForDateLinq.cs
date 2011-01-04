using System;
using System.Linq;
using vlko.BlogModule.NH.Repository;
using vlko.core.Repository;
using vlko.BlogModule.Tests.Repository.IOCResolved.Model;

namespace vlko.BlogModule.Tests.Repository.IOCResolved.Queries
{
    public class QueryActionReservationForDayLinq : BaseLinqQueryAction<Reservation>, IQueryActionReservationForDay
    {
        /// <summary>
        /// Wheres the date.
        /// </summary>
        /// <param name="reservationDate">The reservation date.</param>
        /// <returns>
        /// List of reservations with resolved room relation.
        /// </returns>
        public IQueryResult<Reservation> WhereDate(DateTime reservationDate)
        {
			return Result(Queryable.Where(reserv => reserv.CompositeKey.ReservationDate == reservationDate));
        }
    }
}
