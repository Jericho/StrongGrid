using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Json
{
	/// <summary>
	/// Converts a category (represented either by a string or an array of strings) to and from JSON.
	/// </summary>
	/// <seealso cref="JsonConverter" />
	internal class CategoryConverter : BaseJsonConverter<string[]>
	{
		public override string[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.StartArray)
			{
				reader.Read();

				var values = new List<string>();
				while (reader.TokenType != JsonTokenType.EndArray)
				{
					values.Add(reader.GetString());
					reader.Read();
				}

				return values.ToArray();
			}
			else
			{
				var value = reader.GetString();
				return new[] { value };
			}
		}

		public override void Write(Utf8JsonWriter writer, string[] value, JsonSerializerOptions options)
		{
			if (value is null || value.Length == 0)
			{
				writer.WriteNullValue();
				return;
			}

			writer.WriteStartArray();

			foreach (var item in value)
			{
				writer.WriteStringValue(item);
			}

			writer.WriteEndArray();
		}
	}
}
