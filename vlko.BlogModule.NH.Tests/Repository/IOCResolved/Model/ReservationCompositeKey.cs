using System;

namespace vlko.BlogModule.NH.Tests.Repository.IOCResolved.Model
{
	[Serializable]
	public class ReservationCompositeKey
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="ReservationCompositeKey"/> class.
		/// </summary>
		public ReservationCompositeKey()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReservationCompositeKey"/> class.
		/// </summary>
		/// <param name="hotelId">The hotel id.</param>
		/// <param name="roomId">The room id.</param>
		/// <param name="reservationDate">The reservation date.</param>
		public ReservationCompositeKey(Guid hotelId, Guid roomId, DateTime reservationDate)
		{
			HotelId = hotelId;
			RoomId = roomId;
			ReservationDate = reservationDate;
		}

		public virtual Guid HotelId
		{
			get;
			set;
		}

		public virtual Guid RoomId
		{
			get;
			set;
		}

		public virtual DateTime ReservationDate
		{
			get;
			set;
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return String.Join(":", new string[] {
			                                     	HotelId.ToString(),
			                                     	RoomId.ToString(),
			                                     	ReservationDate.ToString()});
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <exception cref="T:System.NullReferenceException">
		/// The <paramref name="obj"/> parameter is null.
		/// </exception>
		public override bool Equals(object obj)
		{
			if ((obj == this))
			{
				return true;
			}
			if (((obj == null)
			     || (obj.GetType() != this.GetType())))
			{
				return false;
			}
			var test = ((ReservationCompositeKey)(obj));
			return ((HotelId == test.HotelId)
			        && (RoomId == test.RoomId)
			        && (ReservationDate == test.ReservationDate));
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		public override int GetHashCode()
		{
			return XorHelper(HotelId.GetHashCode(), RoomId.GetHashCode(), ReservationDate.GetHashCode());
		}

		/// <summary>
		/// Xors the helper.
		/// </summary>
		/// <param name="first">The first.</param>
		/// <param name="second">The second.</param>
		/// <param name="third">The third.</param>
		/// <returns>Xored value.</returns>
		private static int XorHelper(int first, int second, int third)
		{
			return first ^ second ^ third;
		}
	}
}