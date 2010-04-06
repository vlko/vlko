using System;
using System.Linq;
using GeneralRepositoryTest.IOCResolved.Model;
using GenericRepository;
using vlko.core.ActiveRecord;

namespace GeneralRepositoryTest.IOCResolved.Queries
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
            Queryable = Queryable.Where(reserv => reserv.CompositeKey.ReservationDate == reservationDate);
            return Result();
        }
    }
}
