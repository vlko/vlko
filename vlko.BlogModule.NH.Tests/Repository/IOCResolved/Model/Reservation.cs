namespace vlko.BlogModule.NH.Tests.Repository.IOCResolved.Model
{
	public class Reservation
	{
		/// <summary>
		/// Gets or sets the composite key.
		/// </summary>
		/// <value>The composite key.</value>
		public virtual ReservationCompositeKey CompositeKey { get; set; }

		public virtual string Name
		{
			get;
			set;
		}

		public virtual Hotel Hotel
		{
			get;
			set;
		}

		public virtual Room Room
		{
			get;
			set;
		}
	}
}
