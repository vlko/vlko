using System;
using System.Collections.Generic;
using ConfOrm;
using ConfOrm.NH;
using vlko.core.Roots;

namespace vlko.core.NH
{
	public class ComponentDbInit : IComponentDbInit
	{
		/// <summary>
		/// Lists the of model types for this component.
		/// </summary>
		/// <returns>
		/// List of model types for this component.
		/// </returns>
		public Type[] ListOfModelTypes()
		{
			return new [] { typeof (AppSetting), typeof(User)};
		}

		/// <summary>
		/// Initializes the mappings.
		/// </summary>
		/// <param name="orm">The orm.</param>
		/// <param name="mapper">The mapper.</param>
		public void InitMappings(ObjectRelationalMapper orm, Mapper mapper)
		{
			// list all the entities we want to map.
			IEnumerable<Type> baseEntities = ListOfModelTypes();

			// we map non Content classes as normal
			orm.TablePerClass(baseEntities);

			orm.Poid<AppSetting>(item => item.Id);

			mapper.Customize<AppSetting>(mapping => mapping.Property(item => item.Value, pm => pm.Length(255)));

			mapper.Customize<User>(mapping =>
			{
				mapping.Property(item => item.Email, pm => pm.Unique(true));
				mapping.Property(item => item.Password, pm => pm.Length(64));
			});
		}
	}
}