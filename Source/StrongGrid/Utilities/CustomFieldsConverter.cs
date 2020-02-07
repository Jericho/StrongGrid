using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StrongGrid.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Converts a <see cref="Field"/> to and from JSON.
	/// This converter is intended to be used with SendGrid's new API.
	/// See <seealso cref="LegacyCustomFieldsConverter"/> for the converter for the legacy API.
	/// </summary>
	/// <seealso cref="Newtonsoft.Json.JsonConverter" />
	internal class CustomFieldsConverter : JsonConverter
	{
		/// <summary>
		/// Determines whether this instance can convert the specified object type.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns>
		/// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanConvert(Type objectType)
		{
			return true;
		}

		/// <summary>
		/// Writes the JSON representation of the object.
		/// </summary>
		/// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
		/// <param name="value">The value.</param>
		/// <param name="serializer">The calling serializer.</param>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null) return;

			var fields = (Field[])value;

			writer.WriteStartObject();
			foreach (var customField in fields.OfType<Field<string>>())
			{
				writer.WritePropertyName(customField.Id);
				writer.WriteValue(customField.Value);
			}

			foreach (var customField in fields.OfType<Field<long>>())
			{
				writer.WritePropertyName(customField.Id);
				writer.WriteValue(customField.Value);
			}

			foreach (var customField in fields.OfType<Field<DateTime>>())
			{
				writer.WritePropertyName(customField.Id);
				writer.WriteValue(customField.Value.ToUniversalTime().ToString("o"));
			}

			writer.WriteEndObject();
		}

		/// <summary>
		/// Reads the JSON representation of the object.
		/// </summary>
		/// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="existingValue">The existing value of object being read.</param>
		/// <param name="serializer">The calling serializer.</param>
		/// <returns>
		/// The object value.
		/// </returns>
		/// <exception cref="System.Exception">Unable to determine the field type.</exception>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.StartObject)
			{
				var fields = new List<Field>();
				var jObject = JObject.Load(reader);
				foreach (var property in jObject.Properties())
				{
					var propertyType = property.Value.Type;
					var propertyName = property.Name;
					var field = (Field)null;

					switch (propertyType)
					{
						case JTokenType.Date:
							field = new Field<DateTime>(propertyName, property.Value.Value<DateTime>());
							break;
						case JTokenType.String:
							field = new Field<string>(propertyName, property.Value.Value<string>());
							break;
						case JTokenType.Integer:
							field = new Field<long>(propertyName, property.Value.Value<long>());
							break;
						default:
							throw new Exception($"{propertyType} is an unknown field type");
					}

					fields.Add(field);
				}

				return fields.ToArray();
			}

			return Array.Empty<Field>();
		}
	}
}
