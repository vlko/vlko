using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vlko.core.InversionOfControl
{
	/// <summary>
	/// The Inversion of Control factory.
	/// </summary>
	public static class IoC
	{
        static IoCScopeHolder _scopeHolder = new IoCScopeHolder();
        /// <summary>
        /// Default scope to resolve items, you can use indexer to access scopes by ident.
        /// </summary>
        public static IoCScopeHolder Scope => _scopeHolder;
	}
}
