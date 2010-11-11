using System;
using System.Linq;
using vlko.core.Repository;
using vlko.model.Implementation.NH.Repository;
using vlko.model.Tests.Repository.IOCResolved.Model;

namespace vlko.model.Tests.Repository.IOCResolved.Queries
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
