using System.Collections.Generic;

namespace vlko.BlogModule.Tests.Repository.IOCResolved.Model
{
	public class Hotel
	{

		public virtual System.Guid Id
		{
			get;
			set;
		}

		public virtual string Name
		{
			get;
			set;
		}

		public virtual string Description
		{
			get;
			set;
		}

		public virtual IList<Room> Rooms
		{
			get;
			set;
		}

		public virtual IList<Reservation> Reservations
		{
			get;
			set;
		}
	}
}
