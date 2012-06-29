using System;
using System.ComponentModel.Composition;
using NHibernate.Mapping.ByCode;

namespace vlko.core.NH
{
	[InheritedExport]
	public interface IComponentDbInit
	{
		/// <summary>
		/// Lists the of model types for this component.
		/// </summary>
		/// <returns>List of model types for this component.</returns>
		Type[] ListOfModelTypes();

        /// <summary>
        /// Initializes the mappings.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        void InitMappings(ConventionModelMapper mapper);
	}
}