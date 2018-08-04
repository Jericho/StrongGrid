using Newtonsoft.Json;
using StrongGrid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Converts a  MailPersonalization object to and from JSON.
	/// </summary>
	/// <seealso cref="Newtonsoft.Json.JsonConverter" />
	internal class MailPersonalizationConverter : JsonConverter
	{
		private readonly bool _isUsedWithDynamicTemplate;

		public MailPersonalizationConverter()
			: this(false)
		{
		}

		public MailPersonalizationConverter(bool isUsedWithDynamicTemplate)
		{
			_isUsedWithDynamicTemplate = isUsedWithDynamicTemplate;
		}

		/// <summary>
		/// Determines whether this instance can convert the specified object type.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns>
		/// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(MailPersonalization);
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON; otherwise, <c>false</c>.
		/// </value>
		public override bool CanRead
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON; otherwise, <c>false</c>.
		/// </value>
		public override bool CanWrite
		{
			get { return true; }
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

			writer.WriteStartObject();

#if NETSTANDARD1
			var props = value.GetType().GetTypeInfo().DeclaredProperties;
#else
			var props = value.GetType().GetProperties();
#endif

			foreach (var propertyInfo in props)
			{
				var propertyCustomAttributes = propertyInfo.GetCustomAttributes(false);
				var propertyConverterAttribute = propertyCustomAttributes.OfType<JsonConverterAttribute>().FirstOrDefault();
				var propertyIsIgnored = propertyCustomAttributes.OfType<JsonIgnoreAttribute>().Any();
				var propertyName = propertyCustomAttributes.OfType<JsonPropertyAttribute>().FirstOrDefault()?.PropertyName ?? propertyInfo.Name;
				var propertyValue = propertyInfo.GetValue(value);

				// Skip the property if it's decorated with the 'ignore' attribute
				if (propertyIsIgnored) continue;

				// Ignore the property if it contains a null value
				if (propertyValue == null) continue;

				// Special case: substitutions
				if (propertyInfo.Name == "Substitutions")
				{
					// Ignore this property when email is sent using a dynamic template
					if (_isUsedWithDynamicTemplate) continue;

					// Ignore this property if the enumeration is empty
					var substitutions = (IEnumerable<KeyValuePair<string, string>>)propertyValue;
					if (!substitutions.Any()) continue;

					// Write the substitutions to JSON
					WriteJsonProperty(writer, propertyName, propertyValue, serializer, propertyConverterAttribute);
				}

				// Special case: dynamic data
				else if (propertyInfo.Name == "DynamicData")
				{
					// Ignore this property when email is sent without using a template or when using a 'legacy' template
					if (!_isUsedWithDynamicTemplate) continue;

					// Write the dynamic data to JSON
					WriteJsonProperty(writer, propertyName, propertyValue, serializer, propertyConverterAttribute);
				}

				// Write the property to JSON
				else
				{
					WriteJsonProperty(writer, propertyName, propertyValue, serializer, propertyConverterAttribute);
				}
			}

			// End of JSON serialization
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
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotSupportedException("The MailPersonalizationConverter can only be used to write JSON");
		}

		private void WriteJsonProperty(JsonWriter writer, string propertyName, object propertyValue, JsonSerializer serializer, JsonConverterAttribute propertyConverterAttribute)
		{
			writer.WritePropertyName(propertyName);

			if (propertyConverterAttribute != null)
			{
				var converter = (JsonConverter)Activator.CreateInstance(propertyConverterAttribute.ConverterType);
				var propertyJsonSerializer = new JsonSerializer();
				propertyJsonSerializer.Converters.Add(converter);
				propertyJsonSerializer.Serialize(writer, propertyValue);
			}
			else
			{
				serializer.Serialize(writer, propertyValue);
			}
		}
	}
}
