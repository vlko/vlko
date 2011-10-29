using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Raven.Client;
using Raven.Client.Document;
using vlko.core.InversionOfControl;
using vlko.core.RavenDB.Indexes;
using vlko.core.RavenDB.Repository;
using vlko.core.RavenDB.Repository.ReferenceProxy;
using vlko.core.Roots;

namespace vlko.core.RavenDB
{
	public static class DBInit
	{
		/// <summary>
		/// Lists the of model types.
		/// </summary>
		/// <returns>List of model types.</returns>
		public static Type[] ListOfModelTypes()
		{
			List<Type> result = new List<Type>();
			foreach (var componentDbInit in IoC.ResolveAllInstances<IComponentDbInit>())
			{
				result.AddRange(componentDbInit.ListOfModelTypes());
			}
			return result.ToArray();

		}

		/// <summary>
		/// Registers the indexes.
		/// </summary>
		/// <param name="documentStore">The document store.</param>
		private static void RegisterIndexes(IDocumentStore documentStore)
		{

		}

		/// <summary>
		/// Registers the document store.
		/// </summary>
		/// <param name="documentStore">The document store.</param>
		public static void RegisterDocumentStore(IDocumentStore documentStore)
		{
			SessionFactory.DocumentStoreInstance = documentStore;

			documentStore.Conventions.CustomizeJsonSerializer += json => json.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;

			// register custom contract resolver to handle relations
			documentStore.Conventions.JsonContractResolver = new RelationContractResolver(
				(DefaultRavenContractResolver)documentStore.Conventions.JsonContractResolver,
				documentStore.Conventions,
				ListOfModelTypes());

			var componentInstances = IoC.ResolveAllInstances<IComponentDbInit>().ToArray();

			// customize document store
			foreach (var componentDbInit in componentInstances)
			{
				componentDbInit.CustomizeDocumentStore(documentStore);
			}

			// register indexes
			foreach (var componentDbInit in componentInstances)
			{
				componentDbInit.RegisterIndexes(documentStore);
			}
		}
	}
}
