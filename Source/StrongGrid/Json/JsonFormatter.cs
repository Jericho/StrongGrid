using Pathoschild.Http.Client.Formatters;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Json
{
	internal class JsonFormatter : MediaTypeFormatterBase
	{
		internal static readonly JsonSerializerContext DeserializationContext;
		internal static readonly JsonSerializerContext SerializationContext;

		internal static readonly JsonSerializerOptions DeserializerOptions;
		internal static readonly JsonSerializerOptions SerializerOptions;

		private const int DefaultBufferSize = 1024;

		static JsonFormatter()
		{
			DeserializerOptions = new JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = false
			};

			SerializerOptions = new JsonSerializerOptions()
			{
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			};

			DeserializationContext = new StrongGridJsonSerializerContext(DeserializerOptions);
			SerializationContext = new StrongGridJsonSerializerContext(SerializerOptions);
		}

		public JsonFormatter()
		{
			this.AddMediaType("application/json");
		}

		public override object Deserialize(Type type, Stream stream, HttpContent content, IFormatterLogger formatterLogger)
		{
			var reader = new StreamReader(
				stream,
				encoding: Encoding.UTF8,
				detectEncodingFromByteOrderMarks: true,
				bufferSize: DefaultBufferSize,
				leaveOpen: true); // don't close (stream disposal is handled elsewhere)
			string streamContent = reader.ReadToEnd();
			object deserializedResult = JsonSerializer.Deserialize(streamContent, type, DeserializationContext);
			return deserializedResult;
		}

		public override void Serialize(Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
		{
			var writer = new StreamWriter(
				stream,
				encoding: new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true),
				bufferSize: DefaultBufferSize,
				leaveOpen: true);
			writer.Write(JsonSerializer.Serialize(value, type, SerializationContext));
			writer.Flush();
		}
	}
}
