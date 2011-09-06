using System;
using vlko.BlogModule.NH.Tests.Repository.IOCResolved.Model;
using vlko.core.NH.Repository;
using vlko.core.Repository;

namespace vlko.BlogModule.NH.Tests.Repository.IOCResolved.Queries
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
