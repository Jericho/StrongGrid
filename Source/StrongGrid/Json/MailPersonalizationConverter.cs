using StrongGrid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

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
				if (propertyName.Equals("Substitutions", StringComparison.OrdinalIgnoreCase))
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
				else if (propertyName.Equals("DynamicData", StringComparison.OrdinalIgnoreCase)
					  || propertyName.Equals("dynamic_template_data", StringComparison.OrdinalIgnoreCase))
				{
					// Ignore this property when email is sent without using a template or when using a 'legacy' template
					if (!isUsedWithDynamicTemplate) return;

					// Developers can either specify their own serialization options or we use reflection-based serialization
					var dynamicDataSerializationOptions = value.DynamicDataSerializationOptions ?? new JsonSerializerOptions
					{
						TypeInfoResolver = new DefaultJsonTypeInfoResolver()
					};

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
