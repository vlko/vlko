using System;
using Raven.Client;
using vlko.core.RavenDB.Indexes;
using vlko.core.Roots;

namespace vlko.core.RavenDB
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
			return new[]
			       	{
			       		typeof (AppSetting),
			       		typeof (User)
			       	};
		}

		/// <summary>
		/// Registers the indexes.
		/// </summary>
		/// <param name="documentStore">The document store.</param>
		public void RegisterIndexes(IDocumentStore documentStore)
		{
			new UsersByNameSortIndex().Execute(documentStore);
		}

		/// <summary>
		/// Customizes the document store.
		/// </summary>
		/// <param name="documentStore">The document store.</param>
		public void CustomizeDocumentStore(IDocumentStore documentStore)
		{
			// nothing here to customize
		}
	}
}