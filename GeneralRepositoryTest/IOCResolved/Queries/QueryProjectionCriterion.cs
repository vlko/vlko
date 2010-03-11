using NHibernate.LambdaExtensions;
using GeneralRepositoryTest.IOCResolved.Model;
using GenericRepository;
using NHibernate.Criterion;
using vlko.model.ActiveRecord;

namespace GeneralRepositoryTest.IOCResolved.Queries
{
    public class QueryProjectionCriterion : BaseCriterionQuery<Room>, IQueryProjection
    {
        public IQueryResult<RoomWithHotelProjection> DoProjection()
        {
            Hotel Hotel = null;
            string RoomName = null, HotelName = null;
            return new ProjectionQueryResult<Room, RoomWithHotelProjection>(
                    Criteria
                            .CreateAlias<Room>(room => room.Hotel, () => Hotel),
                    Projections.ProjectionList()
                                       .Add(LambdaProjection.Property<Room>(room => room.Name)
                                            .As(() => RoomName))
                                       .Add(LambdaProjection.Property<Room>(room => room.Hotel.Name)
                                            .As(() => HotelName)));
        }

    }
}
