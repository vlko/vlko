using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;

namespace GeneralRepositoryTest.IOCResolved.Queries
{
    public class RoomWithHotelProjection
    {
        public RoomWithHotelProjection()
        {

        }

        // this constructor is only needed for correct linq query
        public RoomWithHotelProjection(string roomName, string hotelName)
        {
            RoomName = roomName;
            HotelName = hotelName;
        }

        public string RoomName { get; set; }
        public string HotelName { get; set; }
    }
}
