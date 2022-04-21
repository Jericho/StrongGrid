using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Json
{
	/// <summary>
	/// Converts a JSON string into and array of <see cref="KeyValuePair{TKey, TValue}">metrics</see>.
	/// </summary>
	/// <seealso cref="JsonConverter" />
	internal class MetricsConverter : BaseJsonConverter<KeyValuePair<string, long>[]>
	{
		public override KeyValuePair<string, long>[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (JsonDocument.TryParseValue(ref reader, out var doc))
			{
				if (doc.RootElement.ValueKind == JsonValueKind.Object)
				{
					var metrics = doc.RootElement
						.EnumerateObject()
						.Select(property => new KeyValuePair<string, long>(property.Name, property.Value.GetInt64()));

					return metrics.ToArray();
				}
			}

			return Array.Empty<KeyValuePair<string, long>>();
		}

		public override void Write(Utf8JsonWriter writer, KeyValuePair<string, long>[] value, JsonSerializerOptions options)
		{
			if (value == null) return;

			writer.WriteStartObject();

			foreach (var pair in (IEnumerable<KeyValuePair<string, long>>)value)
			{
				writer.WriteNumber(pair.Key, pair.Value);
			}

			writer.WriteEndObject();
		}
	}
}
