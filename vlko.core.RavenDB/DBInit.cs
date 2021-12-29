using Raven.Client;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using vlko.core.InversionOfControl;
using vlko.core.RavenDB;
using vlko.core.RavenDB.DBAccess;

namespace vlko.core.DBAccess
{
    public static class DBInit
    {

        /// <summary>
        /// Registers raven session for specified document store.
        /// </summary>
        /// <param name="store">The document store.</param>
        public static void RegisterRavenSessionProvider(this DB.DBInfoHolder infoHolder, IDocumentStore store, bool initializedStore = false)
        {
            infoHolder.RegisterSessionProvider(new RavenAsyncDatabase(store));
            infoHolder.RegisterSessionProvider(new RavenDatabase(store));

            if (!initializedStore)
            {
                SetGenericsConvention(store);
                store.Initialize();
            }

            PreloadData(store, infoHolder.Ident);
        }

        public static void SetGenericsConvention(IDocumentStore documentStore)
        {
            // generics without generic type
            documentStore.Conventions.FindCollectionName = type =>
            {
                if (type.IsGenericType)
                    return type.Name;
                return DocumentConventions.DefaultGetCollectionName(type);
            };
        }

        static void PreloadData(IDocumentStore store, string ident)
        {
            var componentInstances = IoC.Scope[ident].ResolveAllInstances<IComponentDbInit>().ToArray();

            // preload data
            foreach (var componentDbInit in componentInstances)
            {
                componentDbInit.PreloadData(store, ident);
            }
        }
    }
}
