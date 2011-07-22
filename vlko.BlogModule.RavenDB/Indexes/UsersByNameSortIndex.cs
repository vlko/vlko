using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using vlko.BlogModule.Roots;
using vlko.core.Roots;

namespace vlko.BlogModule.RavenDB.Indexes
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
