using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
		private bool _isUsedWithDynamicTemplate;

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

			var substitutions = Enumerable.Empty<KeyValuePair<string, string>>();

			writer.WriteStartObject();

#if NETSTANDARD1
			var props = value.GetType().GetTypeInfo().DeclaredProperties;
#else
			var props = value.GetType().GetProperties();
#endif

			foreach (var propertyInfo in props)
			{
				var customAttributes = propertyInfo.GetCustomAttributes(false);

				// Special cases when used with a dynamic template
				if (_isUsedWithDynamicTemplate && propertyInfo.Name == "Substitutions")
				{
					var tempSubstitutions = propertyInfo.GetValue(value);
					if (tempSubstitutions != null) substitutions = (IEnumerable<KeyValuePair<string, string>>)tempSubstitutions;
					continue;
				}

				// Skip properties that have the 'ignore' attribute (if any)
				var ignorAttribute = customAttributes.OfType<JsonIgnoreAttribute>().FirstOrDefault();
				if (ignorAttribute != null) continue;

				// Ignore properties that contain a null value
				var tempVal = propertyInfo.GetValue(value);
				if (tempVal == null) continue;

				// Write the property name to JSON
				var propertyName = customAttributes.OfType<JsonPropertyAttribute>().FirstOrDefault()?.PropertyName ?? propertyInfo.Name;
				writer.WritePropertyName(propertyName);

				// Write the property value to JSON
				var converterAttribute = customAttributes.OfType<JsonConverterAttribute>().FirstOrDefault();
				if (converterAttribute != null)
				{
					var propertyJsonSerializer = new JsonSerializer();
					var propertyJsonConverter = Activator.CreateInstance(converterAttribute.ConverterType);
					propertyJsonSerializer.Converters.Add((JsonConverter)propertyJsonConverter);
					propertyJsonSerializer.Serialize(writer, tempVal);
				}
				else
				{
					serializer.Serialize(writer, tempVal);
				}
			}

			// Write the Substitutions to JSON
			if (_isUsedWithDynamicTemplate && substitutions.Any())
			{
				writer.WritePropertyName("dynamic_template_data");
				writer.WriteStartObject();

				foreach (var pair in substitutions)
				{
					writer.WritePropertyName(pair.Key);
					serializer.Serialize(writer, pair.Value);
				}

				writer.WriteEndObject();
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
	}
}
