using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using Raven.Client.Document;

namespace vlko.BlogModule.RavenDB.Repository.ReferenceProxy
{
	public class RelationContractResolver : DefaultRavenContractResolver
	{
		private readonly HashSet<Type> _rootTypes;
		private readonly ReferenceConverter _referenceConverter;

		public RelationContractResolver(DefaultContractResolver original, DocumentConvention documentConvention, IEnumerable<Type> rootTypes)
			: base(true)
		{
			_rootTypes = new HashSet<Type>();
			_referenceConverter = new ReferenceConverter(documentConvention);
			foreach (var rootType in rootTypes)
			{
				_rootTypes.Add(rootType);
			}
			DefaultMembersSearchFlags = original.DefaultMembersSearchFlags;
		}

		protected override JsonProperty CreateProperty(MemberInfo member, Newtonsoft.Json.MemberSerialization memberSerialization)
		{
			JsonProperty property = base.CreateProperty(member, memberSerialization);

			if (_rootTypes.Contains(property.PropertyType))
			{
				property.Converter = _referenceConverter;
				property.MemberConverter = _referenceConverter;
			}
			else
			{
				if (property.PropertyType.IsGenericType)
				{
					foreach (Type @interface in property.PropertyType.GetInterfaces())
					{
						if (@interface.IsGenericType)
						{
							if (@interface.GetGenericTypeDefinition() == typeof(ICollection<>))
							{
								if (_rootTypes.Contains(@interface.GetGenericArguments()[0]))
								{
									property.Ignored = true;
								}
							}
						}
					}
				}
			}
			return property;
		}
	}
}