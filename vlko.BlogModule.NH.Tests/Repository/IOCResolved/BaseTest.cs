using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.core.NH.Repository;
using vlko.core.Repository;
using vlko.core.Repository.RepositoryAction;
using vlko.BlogModule.Tests.Repository.IOCResolved.Model;
using vlko.BlogModule.Tests.Repository.IOCResolved.Queries;

namespace vlko.BlogModule.Tests.Repository.IOCResolved
{
    public class BaseTest
    {
        BaseRepository<Hotel> _hotelBaseRepository = new Repository<Hotel>();
        BaseRepository<Room> _roomBaseRepository = new Repository<Room>();
        BaseRepository<Reservation> _reservationBaseRepository = new Repository<Reservation>();

        public virtual void Intialize()
        {
            using (var tran = RepositoryFactory.StartTransaction())
            {
                var hotel1 = new Hotel { Id = Guid.NewGuid(), Name = "Hotel1" };
                _hotelBaseRepository.GetAction<ICreateAction<Hotel>>().Create(hotel1);
                var hotel2 = new Hotel { Id = Guid.NewGuid(), Name = "Hotel2" };

                _hotelBaseRepository.GetAction<ICreateAction<Hotel>>().Create(hotel2);
                var rooms = new Room[] { 
                    new Room { Id = Guid.NewGuid(), Name = "101",
                        HasBathroom = false, HasToilet = false, Beds = 2,
                        Hotel = hotel1
                    },
                    new Room { Id = Guid.NewGuid(), Name = "102",
                        HasBathroom = false, HasToilet = true, Beds = 3,
                        Hotel = hotel1
                    },
                    new Room { Id = Guid.NewGuid(), Name = "103",
                        HasBathroom = true, HasToilet = true, Beds = 3,
                        Hotel = hotel1
                    },
                    new Room { Id = Guid.NewGuid(), Name = "104",
                        HasBathroom = true, HasToilet = true, Beds = 3,
                        Hotel = hotel1
                    },
                    new Room { Id = Guid.NewGuid(), Name = "105",
                        HasBathroom = true, HasToilet = true, Beds = 4,
                        Hotel = hotel1
                    },
                    new Room { Id = Guid.NewGuid(), Name = "201",
                        HasBathroom = true, HasToilet = true, Beds = 5,
                        Hotel = hotel2
                    },
                    new Room { Id = Guid.NewGuid(), Name = "202",
                        HasBathroom = true, HasToilet = true, Beds = 5,
                        Hotel = hotel2
                    },
                    new Room { Id = Guid.NewGuid(), Name = "203",
                        HasBathroom = true, HasToilet = true, Beds = 5,
                        Hotel = hotel2
                    }
                };
                foreach (var room in rooms)
                {
                    _roomBaseRepository.GetAction<ICreateAction<Room>>().Create(room);
                }

                var reservations = new Reservation[] { 
                    new Reservation { 
                        CompositeKey = new ReservationCompositeKey(hotel1.Id, rooms[0].Id, new DateTime(2009, 1, 2)),
                        Name = "jozo",
                        Room = rooms[0]
                    },
                    new Reservation { 
                        CompositeKey = new ReservationCompositeKey(hotel1.Id, rooms[0].Id, new DateTime(2009, 1, 3)),
                        Name = "jozo",
                        Room = rooms[0]
                    },
                    new Reservation { 
                        CompositeKey = new ReservationCompositeKey(hotel1.Id, rooms[1].Id, new DateTime(2009, 1, 2)),
                        Name = "fero",
                        Room = rooms[1]
                    },
                    new Reservation { 
                        CompositeKey = new ReservationCompositeKey(hotel2.Id, rooms[5].Id, new DateTime(2009, 1, 2)),
                        Name = "gejza",
                        Room = rooms[5]
                    },
                };
                foreach (var reservation in reservations)
                {
                    _reservationBaseRepository.GetAction<ICreateAction<Reservation>>().Create(reservation);
                }

                tran.Commit();
            }

        }

        /// <summary>
        /// Queries all hotels.
        /// </summary>
        public virtual void QueryAllHotels()
        {
            using (var session = RepositoryFactory.StartUnitOfWork())
            {
				var query = _hotelBaseRepository.GetAction<IQueryActionAll<Hotel>>()
                    .Execute()
                    .OrderBy(room => room.Name);

                Assert.AreEqual(2, query.Count());

                var result = query.ToArray();
                Assert.AreEqual("Hotel1", result[0].Name);
                Assert.AreEqual("Hotel2", result[1].Name);
            }
        }

        /// <summary>
        /// Queries the name of the hotel rooms by.
        /// </summary>
        public virtual void QueryHotelRoomsByName()
        {
            using (var session = RepositoryFactory.StartUnitOfWork())
            {
				var query = _roomBaseRepository.GetAction<IQueryActionHotelRooms>()
                    .WhereHotelName("Hotel1")
                    .OrderBy(room => room.Name);

                Assert.AreEqual(5, query.Count());

                var result = query.ToArray();
                Assert.AreEqual("101", result[0].Name);
                Assert.AreEqual("102", result[1].Name);
                Assert.AreEqual("103", result[2].Name);
                Assert.AreEqual("104", result[3].Name);
                Assert.AreEqual("105", result[4].Name);
            }
        }

        /// <summary>
        /// Queries the name of the hotel rooms by.
        /// </summary>
        public virtual void QueryReservationsForDate()
        {
            using (var session = RepositoryFactory.StartUnitOfWork())
            {
				var query = _reservationBaseRepository.GetAction<IQueryActionReservationForDay>()
                    .WhereDate(new DateTime(2009, 1, 2))
                    .OrderBy(reserv => reserv.Room.Name);

                Assert.AreEqual(3, query.Count());

                var result = query.ToArray();
                Assert.AreEqual("101", result[0].Room.Name);
                Assert.AreEqual("102", result[1].Room.Name);
                Assert.AreEqual("201", result[2].Room.Name);
            }
        }
        /// <summary>
        /// Queries projection.
        /// </summary>
        public virtual void QueryProjection()
        {
            using (var session = RepositoryFactory.StartUnitOfWork())
            {
				var query = _roomBaseRepository.GetAction<IQueryActionProjection>()
                    .DoProjection()
                    .OrderBy(proj => proj.HotelName)
                    .OrderBy(proj => proj.RoomName);

                var pagedResult = query.ToPage(2, 2);
                Assert.AreEqual(2, pagedResult.Length);
                Assert.AreEqual("105", pagedResult[0].RoomName);
                Assert.AreEqual("201", pagedResult[1].RoomName);

                Assert.AreEqual(8, query.Count());

                var result = query.ToArray();
                Assert.AreEqual("101", result[0].RoomName);
                Assert.AreEqual("102", result[1].RoomName);
                Assert.AreEqual("103", result[2].RoomName);
                Assert.AreEqual("104", result[3].RoomName);
                Assert.AreEqual("105", result[4].RoomName);
                Assert.AreEqual("201", result[5].RoomName);
                Assert.AreEqual("202", result[6].RoomName);
                Assert.AreEqual("203", result[7].RoomName);
            }
        }
    }
}
