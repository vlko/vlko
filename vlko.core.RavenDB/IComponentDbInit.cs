using Raven.Client.Documents;
using System;

namespace vlko.core.RavenDB
{
    public interface IComponentDbInit
    {
        /// <summary>
        /// Registers the indexes.
        /// </summary>
        /// <param name="documentStore">The document store.</param>
        /// <param name="ident">Store ident (default one has ident null)</param>
        void PreloadData(IDocumentStore documentStore, string ident = null);
    }
}