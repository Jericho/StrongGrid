using StrongGrid.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Json
{
	/// <summary>
	/// Converts a <see cref="Field"/> to and from JSON.
	/// This converter is intended to be used with SendGrid's new API.
	/// See <seealso cref="LegacyCustomFieldsConverter"/> for the converter for the legacy API.
	/// </summary>
	/// <seealso cref="JsonConverter" />
	internal class CustomFieldsConverter : BaseJsonConverter<Field[]>
	{
		public override Field[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (JsonDocument.TryParseValue(ref reader, out var doc))
			{
				var fields = new List<Field>();

				foreach (var property in doc.RootElement.EnumerateObject())
				{
					var propertyKind = property.Value.ValueKind;
					var propertyName = property.Name;
					var field = (Field)null;

					if (propertyKind == JsonValueKind.String)
					{
						var stringValue = property.Value.GetString();
						if (DateTime.TryParse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime dateValue))
						{
							field = new Field<DateTime>(propertyName, dateValue);
						}
						else
						{
							field = new Field<string>(propertyName, stringValue);
						}
					}
					else if (propertyKind == JsonValueKind.Number)
					{
						field = new Field<long>(propertyName, property.Value.GetInt64());
					}
					else
					{
						throw new JsonException($"{propertyKind} is an unknown field type");
					}

					fields.Add(field);
				}

				return fields.ToArray();
			}

			return Array.Empty<Field>();
		}

		public override void Write(Utf8JsonWriter writer, Field[] value, JsonSerializerOptions options)
		{
			if (value == null) return;

			writer.WriteStartObject();

			foreach (var customField in value.OfType<Field<string>>())
			{
				writer.WritePropertyName(customField.Id);
				writer.WriteStringValue(customField.Value);
			}

			foreach (var customField in value.OfType<Field<long>>())
			{
				writer.WritePropertyName(customField.Id);
				writer.WriteNumberValue(customField.Value);
			}

			foreach (var customField in value.OfType<Field<DateTime>>())
			{
				writer.WritePropertyName(customField.Id);
				writer.WriteStringValue(customField.Value.ToUniversalTime().ToString("o"));
			}

			writer.WriteEndObject();
		}
	}
}
