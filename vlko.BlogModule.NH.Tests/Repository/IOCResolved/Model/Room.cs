using System.Collections.Generic;

namespace vlko.BlogModule.NH.Tests.Repository.IOCResolved.Model
{
	public class Room
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

		public virtual bool HasToilet
		{
			get;
			set;
		}

		public virtual bool HasBathroom
		{
			get;
			set;
		}

		public virtual int Beds
		{
			get;
			set;
		}


		public virtual Hotel Hotel
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
