using System;

namespace vlko.core.RavenDB.Repository
{
	public class UniqueIdentException : Exception
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="UniqueIdentException"/> class.
		/// </summary>
		/// <param name="entityType">Type of the entity.</param>
		/// <param name="propertyName">Name of the property.</param>
		public UniqueIdentException(Type entityType, string propertyName)
			: base(string.Format("Property '{0}' not unique in type '{1}'", propertyName, entityType))
		{
			EntityType = entityType;
			PropertyName = propertyName;
		}

		public Type EntityType { get; set; }
		public string PropertyName { get; set; }
	}
}
