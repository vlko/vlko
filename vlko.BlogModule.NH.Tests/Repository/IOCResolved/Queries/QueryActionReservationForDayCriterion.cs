using System;
using vlko.core.NH.Repository;
using vlko.core.Repository;
using vlko.BlogModule.Tests.Repository.IOCResolved.Model;

namespace vlko.BlogModule.Tests.Repository.IOCResolved.Queries
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
				.JoinAlias(reserv => reserv.Room, () => Room)
				.Where(reserv => reserv.CompositeKey.ReservationDate == reservationDate);
			return Result();
		}

	}
}
