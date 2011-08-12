using System.Linq;
using Raven.Client.Indexes;
using vlko.core.Roots;

namespace vlko.core.RavenDB.Indexes
{
	public class UsersByNameSortIndex : AbstractIndexCreationTask<User>
	{
		public UsersByNameSortIndex()
		{
			Map = users => from user in users
			               select new {user.Name, user.Email, user.VerifyToken};
		}
	}
}
