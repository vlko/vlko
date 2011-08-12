using System;

namespace vlko.core.RavenDB.Repository.ReferenceProxy
{
	public class DenormalizedReference
	{
		public string Id { get; set; }
		public Type ReferenceInstanceType { get; set; }
	}
}