using System;
using System.ComponentModel.Composition;
using Raven.Client;

namespace vlko.core.RavenDB
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
		/// Registers the indexes.
		/// </summary>
		/// <param name="documentStore">The document store.</param>
		void RegisterIndexes(IDocumentStore documentStore);

		/// <summary>
		/// Customizes the document store.
		/// </summary>
		/// <param name="documentStore">The document store.</param>
		void CustomizeDocumentStore(IDocumentStore documentStore);
	}
}