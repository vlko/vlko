using System;
using System.ComponentModel.Composition;
using ConfOrm;
using ConfOrm.NH;

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
		/// <param name="orm">The orm.</param>
		/// <param name="mapper">The mapper.</param>
		void InitMappings(ObjectRelationalMapper orm, Mapper mapper);
	}
}