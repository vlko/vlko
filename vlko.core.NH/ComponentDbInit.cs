using System;
using System.Collections.Generic;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
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
        /// <param name="mapper">The mapper.</param>
        public void InitMappings(ConventionModelMapper mapper)
		{
            mapper.Class<AppSetting>(mapping =>
                                         {
                                             mapping.Property(item => item.Value, pm => pm.Length(255));
                                         });

            mapper.Class<User>(mapping =>
            {
                mapping.Property(item => item.Email, pm => pm.Unique(true));
                mapping.Property(item => item.Password, pm => pm.Length(64));
            });
		}
	}
}