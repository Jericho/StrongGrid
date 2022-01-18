using StrongGrid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Converts a  MailPersonalization object to and from JSON.
	/// </summary>
	/// <seealso cref="JsonConverter" />
	internal class MailPersonalizationConverter : JsonConverter<MailPersonalization>
	{
		public MailPersonalizationConverter()
		{
		}

		public override MailPersonalization Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			throw new NotSupportedException("The MailPersonalizationConverter can only be used to write JSON");
		}

		public override void Write(Utf8JsonWriter writer, MailPersonalization value, JsonSerializerOptions options)
		{
			if (value == null) return;

			writer.WriteStartObject();

			var allProperties = typeof(MailPersonalization).GetProperties();
			var isUsedWithDynamicTemplate = value.IsUsedWithDynamicTemplate;

			foreach (var propertyInfo in allProperties)
			{
				var propertyCustomAttributes = propertyInfo.GetCustomAttributes(false);
				var propertyConverterAttribute = propertyCustomAttributes.OfType<JsonConverterAttribute>().FirstOrDefault();
				var propertyIsIgnored = propertyCustomAttributes.OfType<JsonIgnoreAttribute>().Any();
				var propertyName = propertyCustomAttributes.OfType<JsonPropertyNameAttribute>().FirstOrDefault()?.Name ?? propertyInfo.Name;
				var propertyValue = propertyInfo.GetValue(value);

				// Skip the property if it's decorated with the 'ignore' attribute
				if (propertyIsIgnored) continue;

				// Ignore the property if it contains a null value
				if (propertyValue == null) continue;

				// Special case: substitutions
				if (propertyInfo.Name == "Substitutions")
				{
					// Ignore this property when email is sent using a dynamic template
					if (isUsedWithDynamicTemplate) continue;

					// Ignore this property if the enumeration is empty
					var substitutions = (IEnumerable<KeyValuePair<string, string>>)propertyValue;
					if (!substitutions.Any()) continue;

					// Write the substitutions to JSON
					WriteJsonProperty(writer, propertyName, propertyValue, options, propertyConverterAttribute);
				}

				// Special case: dynamic data
				else if (propertyInfo.Name == "DynamicData")
				{
					// Ignore this property when email is sent without using a template or when using a 'legacy' template
					if (!isUsedWithDynamicTemplate) continue;

					// Write the dynamic data to JSON
					WriteJsonProperty(writer, propertyName, propertyValue, options, propertyConverterAttribute);
				}

				// Write the property to JSON
				else
				{
					WriteJsonProperty(writer, propertyName, propertyValue, options, propertyConverterAttribute);
				}
			}

			// End of JSON serialization
			writer.WriteEndObject();
		}

		public void WriteJsonProperty(Utf8JsonWriter writer, string propertyName, object propertyValue, JsonSerializerOptions options, JsonConverterAttribute propertyConverterAttribute)
		{
			writer.WritePropertyName(propertyName);

			// It's important to clone the options in order to be able to modify the 'Converters' list
			var clonedOptions = new JsonSerializerOptions(options);

			if (propertyConverterAttribute != null)
			{
				clonedOptions.Converters.Add((JsonConverter)Activator.CreateInstance(propertyConverterAttribute.ConverterType));
			}

			JsonSerializer.Serialize(writer, propertyValue, propertyValue.GetType(), clonedOptions);
		}
	}
}
