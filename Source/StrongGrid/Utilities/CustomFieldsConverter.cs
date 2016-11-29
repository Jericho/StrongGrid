using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Converts a <see cref="Field"/> to and from JSON.
	/// </summary>
	/// <seealso cref="Newtonsoft.Json.JsonConverter" />
	public class CustomFieldsConverter : JsonConverter
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

			writer.WriteStartArray();
			foreach (var customField in ((Field[])value).OfType<Field<string>>())
			{
				serializer.Serialize(writer, customField);
			}

			foreach (var customField in ((Field[])value).OfType<Field<long>>())
			{
				serializer.Serialize(writer, customField);
			}

			foreach (var customField in ((Field[])value).OfType<Field<long?>>())
			{
				serializer.Serialize(writer, customField);
			}

			foreach (var customField in ((Field[])value).OfType<Field<DateTime>>())
			{
				serializer.Serialize(writer, customField);
			}

			foreach (var customField in ((Field[])value).OfType<Field<DateTime?>>())
			{
				serializer.Serialize(writer, customField);
			}

			writer.WriteEndArray();
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
		/// <exception cref="System.Exception">Unable to determine the field type</exception>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.StartArray)
			{
				var fields = new List<Field>();
				var jArray = JArray.Load(reader);

				foreach (var item in jArray)
				{
					var id = GetPropertyValue<int?>(item, "id");
					var name = GetPropertyValue<string>(item, "name");
					var type = GetPropertyValue<string>(item, "type");
					var field = (Field)null;

					switch (type)
					{
						case "date":
							var unixTime = GetPropertyValue<long?>(item, "value");
							if (unixTime.HasValue) field = new Field<DateTime?>(name, unixTime.Value.FromUnixTime());
							else field = new Field<DateTime?>(name, null);
							break;
						case "text":
							field = new Field<string>(name, GetPropertyValue<string>(item, "value"));
							break;
						case "number":
							field = new Field<long?>(name, GetPropertyValue<long?>(item, "value"));
							break;
						default:
							throw new Exception("Unable to determine the field type");
					}

					if (id.HasValue) field.Id = id.Value;
					fields.Add(field);
				}

				return fields.ToArray();
			}

			return Enumerable.Empty<Field>().ToArray();
		}

		private static T GetPropertyValue<T>(JToken item, string name)
		{
			if (item[name] == null) return default(T);
			return item[name].Value<T>();
		}
	}
}
