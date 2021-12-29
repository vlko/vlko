using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using System;
using vlko.core.Roots;

namespace vlko.core.RavenDB
{
    public class ComponentDbInit : IComponentDbInit
    {
        /// <summary>
        /// Preload data.
        /// </summary>
        /// <param name="documentStore">The document store.</param>
        /// <param name="ident">Store ident (default one has ident null)</param>
        public void PreloadData(IDocumentStore documentStore, string ident = null)
        {

        }

    }
}