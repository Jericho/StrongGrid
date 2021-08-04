using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Converts a <see cref="Models.Legacy.Field"/> to and from JSON.
	/// This converter is intended to be used with SendGrid's legacy API.
	/// See <seealso cref="CustomFieldsConverter"/> for the converter for the new API.
	/// </summary>
	/// <seealso cref="JsonConverter" />
	internal class LegacyCustomFieldsConverter : JsonConverter<Models.Legacy.Field[]>
	{
		public override Models.Legacy.Field[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (JsonDocument.TryParseValue(ref reader, out var doc))
			{
				if (doc.RootElement.ValueKind == JsonValueKind.Array)
				{
					var fields = new List<Models.Legacy.Field>();

					foreach (var item in doc.RootElement.EnumerateArray())
					{
						var id = item.TryGetProperty("id", out JsonElement idProperty) ? idProperty.GetInt32() : (int?)null;
						var name = item.TryGetProperty("name", out JsonElement nameProperty) ? nameProperty.GetString() : null;
						var type = item.TryGetProperty("type", out JsonElement typeProperty) ? typeProperty.GetString() : null;
						var field = (Models.Legacy.Field)null;

						switch (type)
						{
							case "date":
								if (!item.TryGetProperty("value", out JsonElement dateProperty)) field = new Models.Legacy.Field<DateTime?>(name, null);
								else if (dateProperty.ValueKind == JsonValueKind.Number) field = new Models.Legacy.Field<DateTime>(name, dateProperty.GetInt64().FromUnixTime());
								else field = new Models.Legacy.Field<DateTime>(name, long.Parse(dateProperty.GetString()).FromUnixTime());
								break;
							case "text":
								if (!item.TryGetProperty("value", out JsonElement textProperty)) field = new Models.Legacy.Field<string>(name, null);
								else field = new Models.Legacy.Field<string>(name, textProperty.GetString());
								break;
							case "number":
								if (!item.TryGetProperty("value", out JsonElement numericProperty)) field = new Models.Legacy.Field<long?>(name, null);
								else if (numericProperty.ValueKind == JsonValueKind.Number) field = new Models.Legacy.Field<long>(name, numericProperty.GetInt64());
								else field = new Models.Legacy.Field<long>(name, long.Parse(numericProperty.GetString()));
								break;
							default:
								throw new Exception($"{type} is an unknown field type");
						}

						if (id.HasValue) field.Id = id.Value;
						fields.Add(field);
					}

					return fields.ToArray();
				}
			}

			return Array.Empty<Models.Legacy.Field>();
		}

		public override void Write(Utf8JsonWriter writer, Models.Legacy.Field[] value, JsonSerializerOptions options)
		{
			if (value == null) return;

			var serializationOptions = new JsonSerializerOptions()
			{
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
			};

			writer.WriteStartArray();
			foreach (var customField in value.OfType<Models.Legacy.Field<string>>())
			{
				JsonSerializer.Serialize(writer, customField, serializationOptions);
			}

			foreach (var customField in value.OfType<Models.Legacy.Field<long>>())
			{
				JsonSerializer.Serialize(writer, customField, serializationOptions);
			}

			foreach (var customField in value.OfType<Models.Legacy.Field<long?>>())
			{
				JsonSerializer.Serialize(writer, customField, serializationOptions);
			}

			foreach (var customField in value.OfType<Models.Legacy.Field<DateTime>>())
			{
				JsonSerializer.Serialize(writer, customField, serializationOptions);
			}

			foreach (var customField in value.OfType<Models.Legacy.Field<DateTime?>>())
			{
				JsonSerializer.Serialize(writer, customField, serializationOptions);
			}

			writer.WriteEndArray();
		}
	}
}
