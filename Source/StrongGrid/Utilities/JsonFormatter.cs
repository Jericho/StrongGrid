using Pathoschild.Http.Client.Formatters;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Utilities
{
	internal class JsonFormatter : MediaTypeFormatterBase
	{
		private static readonly JsonSerializerOptions _deserializerOptions = new JsonSerializerOptions()
		{
			PropertyNameCaseInsensitive = false
		};

		private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()
		{
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

		private static readonly StrongGridJsonSerializerContext _deserializationContext = new StrongGridJsonSerializerContext(_deserializerOptions);
		private static readonly StrongGridJsonSerializerContext _serializationContext = new StrongGridJsonSerializerContext(_serializerOptions);

		public JsonFormatter()
		{
			this.AddMediaType("application/json");
		}

		public override object Deserialize(Type type, Stream stream, HttpContent content, IFormatterLogger formatterLogger)
		{
			var reader = new StreamReader(stream); // don't dispose (stream disposal is handled elsewhere)
			string streamContent = reader.ReadToEnd();
			object deserializedResult = JsonSerializer.Deserialize(streamContent, type, _deserializationContext);
			return deserializedResult;
		}

		public override void Serialize(Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
		{
			var writer = new StreamWriter(stream);
			writer.Write(JsonSerializer.Serialize(value, type, _serializationContext));
			writer.Write(JsonSerializer.Serialize(value, type, _serializerOptions));
			writer.Flush();
		}
	}
}
