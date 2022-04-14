using StrongGrid.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Json
{
	/// <summary>
	/// Converts a MailPersonalization object to and from JSON.
	/// </summary>
	/// <seealso cref="JsonConverter" />
	internal class MailPersonalizationConverter : BaseJsonConverter<MailPersonalization>
	{
		public MailPersonalizationConverter()
		{
		}

		public override void Write(Utf8JsonWriter writer, MailPersonalization value, JsonSerializerOptions options)
		{
			var isUsedWithDynamicTemplate = value.IsUsedWithDynamicTemplate;

			Serialize(writer, value, options, (propertyName, propertyValue, propertyType, options, propertyConverterAttribute) =>
			{
				// Special case: substitutions
				if (propertyName == "Substitutions")
				{
					// Ignore this property when email is sent using a dynamic template
					if (isUsedWithDynamicTemplate) return;

					// Ignore this property if the enumeration is empty
					var substitutions = (IEnumerable<KeyValuePair<string, string>>)propertyValue;
					if (!substitutions.Any()) return;

					// Write the substitutions to JSON
					WriteJsonProperty(writer, propertyName, propertyValue, propertyType, options, propertyConverterAttribute);
				}

				// Special case: dynamic data
				else if (propertyName == "DynamicData")
				{
					// Ignore this property when email is sent without using a template or when using a 'legacy' template
					if (!isUsedWithDynamicTemplate) return;

					// Developers can either specify their own serialization options or accept the default options
					var dynamicDataSerializationOptions = value.DynamicDataSerializationOptions ?? options;

					// Write the dynamic data to JSON
					WriteJsonProperty(writer, propertyName, propertyValue, propertyType, dynamicDataSerializationOptions, propertyConverterAttribute);
				}

				// Any other property
				else
				{
					// Write the property to JSON
					WriteJsonProperty(writer, propertyName, propertyValue, propertyType, options, propertyConverterAttribute);
				}
			});
		}
	}
}
