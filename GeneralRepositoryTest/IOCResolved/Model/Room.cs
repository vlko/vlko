using System.Collections.Generic;
using Castle.ActiveRecord;

namespace GeneralRepositoryTest.IOCResolved.Model
{
	[ActiveRecord]
	public class Room
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
		public virtual bool HasToilet
		{
			get;
			set;
		}

		[Property]
		public virtual bool HasBathroom
		{
			get;
			set;
		}

		[Property]
		public virtual int Beds
		{
			get;
			set;
		}


        [BelongsTo("HotelId", Lazy = FetchWhen.OnInvoke)]
		public virtual Hotel Hotel
		{
			get;
			set;
		}

		[HasMany(typeof(Reservation), ColumnKey = "RoomId", Table = "Reservation", Inverse = true, Lazy = true)]
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
