using NHibernate.LambdaExtensions;
using System;
using vlko.core.Repository;
using vlko.model.Implementation.NH.Repository;
using vlko.model.Tests.Repository.IOCResolved.Model;

namespace vlko.model.Tests.Repository.IOCResolved.Queries
{
    public class QueryActionReservationForDayCriterion : BaseCriterionQueryAction<Reservation>, IQueryActionReservationForDay
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
            Room Room = null;
            Criteria
                .CreateAlias<Reservation>(reserv => reserv.Room, () => Room)
                .Add<Reservation>(reserv => reserv.CompositeKey.ReservationDate == reservationDate);
            return Result();
        }

    }
}
