using NHibernate;
using NHibernate.Criterion;
using NHibernate.Criterion.Lambda;
using vlko.core.Repository;
using vlko.model.Implementation.NH.Repository;
using vlko.model.Tests.Repository.IOCResolved.Model;

namespace vlko.model.Tests.Repository.IOCResolved.Queries
{
    public class QueryActionProjectionCriterion : BaseCriterionQueryAction<Room>, IQueryActionProjection
    {
        public IQueryResult<RoomWithHotelProjection> DoProjection()
        {
			Hotel Hotel = null;
			string RoomName = null, HotelName = null;
        	return new ProjectionQueryResult<Room, RoomWithHotelProjection>(
        		Criteria.JoinAlias(room => room.Hotel, () => Hotel),
        		Projections.ProjectionList()
        			.Add(Projections.Property<Room>(room => room.Name).WithAlias(() => RoomName))
					.Add(Projections.Property<Room>(room => room.Hotel.Name).WithAlias(() => HotelName)));
        }

    }
}
