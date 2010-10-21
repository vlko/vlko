using System.Collections.Generic;
using Castle.ActiveRecord;

namespace vlko.model.Tests.Repository.IOCResolved.Model
{
	[ActiveRecord]
	public class Hotel
	{

		[PrimaryKey(PrimaryKeyType.Guid)]
		public virtual System.Guid Id
		{
			get;
			set;
		}

		[Property]
		public virtual string Name
		{
			get;
			set;
		}

		[Property]
		public virtual string Description
		{
			get;
			set;
		}

		[HasMany(typeof(Room), ColumnKey = "HotelId", Table = "Room", Lazy = true)]
		public virtual IList<Room> Rooms
		{
			get;
			set;
		}

		[HasMany(typeof(Reservation), ColumnKey = "HotelId", Table = "Reservation", Inverse = true, Lazy = true)]
		public virtual IList<Reservation> Reservations
		{
			get;
			set;
		}

        #region ILocalPk Members

        public object LocalPk
        {
            get { return Id; }
        }

        #endregion
    }
}
