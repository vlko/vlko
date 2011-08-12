using System;

namespace vlko.core.RavenDB.Repository.ReferenceProxy
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
