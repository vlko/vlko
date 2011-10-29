using System;
using System.Globalization;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using Raven.Client.Document;
using vlko.core.Tools;

namespace vlko.core.RavenDB.Repository.ReferenceProxy
{
	public class ReferenceConverter : JsonConverter
	{
		private static readonly ProxyGenerator _generator = new ProxyGenerator();

		private readonly DocumentConvention _documentConvention;


		public ReferenceConverter(DocumentConvention documentConvention)
		{
			_documentConvention = documentConvention;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var referenceType = value is IDynamicProxy
				? (value as IDynamicProxy).GetOriginalTypeBeforeDynamicProxy()
				: value.GetType();

			var reference = new DenormalizedReference()
			                	{
			                		Id = string.Format(CultureInfo.InvariantCulture, "{0}",
			                		                   _documentConvention.GetIdentityProperty(referenceType).GetValue(value, null)),
			                		ReferenceInstanceType = referenceType
			                	};
			serializer.Serialize(writer, reference);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.StartObject)
			{
				var denormalizedReference = new DenormalizedReference();
				serializer.Populate(reader, denormalizedReference);

				var referenceInstace = _generator.CreateClassProxyWithTarget(
					denormalizedReference.ReferenceInstanceType,
					new Type[] { typeof(IDynamicProxy) },
					InstanceCreator.Create(denormalizedReference.ReferenceInstanceType),
					new ReferenceInterceptor(denormalizedReference.Id, denormalizedReference.ReferenceInstanceType));

				return referenceInstace;
			}
			return null;
		}

		public override bool CanConvert(Type objectType)
		{
			return true;
		}
	}
}