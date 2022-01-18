using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Converts an enumeration of KeyValuePair to and from JSON.
	/// </summary>
	/// <seealso cref="JsonConverter" />
	internal class KeyValuePairEnumerationConverter : JsonConverter<KeyValuePair<string, string>[]>
	{
		public override KeyValuePair<string, string>[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (JsonDocument.TryParseValue(ref reader, out var doc))
			{
				if (doc.RootElement.ValueKind == JsonValueKind.Object)
				{
					var pairs = doc.RootElement
						.EnumerateObject()
						.Select(property => new KeyValuePair<string, string>(property.Name, property.Value.GetString()));

					return pairs.ToArray();
				}
			}

			return Array.Empty<KeyValuePair<string, string>>();
		}

		public override void Write(Utf8JsonWriter writer, KeyValuePair<string, string>[] value, JsonSerializerOptions options)
		{
			if (value == null) return;

			writer.WriteStartObject();

			foreach (var pair in (IEnumerable<KeyValuePair<string, string>>)value)
			{
				writer.WritePropertyName(pair.Key);
				writer.WriteStringValue(pair.Value);
			}

			writer.WriteEndObject();
		}
	}
}
