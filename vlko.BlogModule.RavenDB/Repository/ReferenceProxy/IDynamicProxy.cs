using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vlko.BlogModule.RavenDB.Repository.ReferenceProxy
{
	public interface IDynamicProxy
	{
		/// <summary>
		/// Gets the original type before dynamic proxy.
		/// </summary>
		/// <returns>Original type.</returns>
		Type GetOriginalTypeBeforeDynamicProxy();
	}
}
