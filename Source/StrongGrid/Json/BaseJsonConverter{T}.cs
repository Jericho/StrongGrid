using System;
using System.Collections;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Json
{
	/// <summary>
	/// Base class for other JSON converter classes.
	/// </summary>
	/// <seealso cref="JsonConverter" />
	internal abstract class BaseJsonConverter<T> : JsonConverter<T>
	{
		public BaseJsonConverter()
		{
		}

		public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
		{
			var typeOfValue = typeof(T);

			if (typeOfValue.IsArray)
			{
				var typeOfItems = typeOfValue.GetElementType();

				writer.WriteStartArray();

				foreach (var item in (IEnumerable)value)
				{
					JsonSerializer.Serialize(writer, item, typeOfItems, options);
				}

				writer.WriteEndArray();
			}
			else
			{
				Serialize(writer, value, options, null);
			}
		}

		internal void Serialize(Utf8JsonWriter writer, T value, JsonSerializerOptions options, Action<string, object, Type, JsonSerializerOptions, JsonConverterAttribute> propertySerializer = null)
		{
			if (value == null)
			{
				writer.WriteNullValue();
				return;
			}

			if (propertySerializer == null)
			{
				propertySerializer = (propertyName, propertyValue, propertyType, options, propertyConverterAttribute) => WriteJsonProperty(writer, propertyName, propertyValue, propertyType, options, propertyConverterAttribute);
			}

			writer.WriteStartObject();

			var allProperties = typeof(T).GetProperties();

			foreach (var propertyInfo in allProperties)
			{
				var propertyCustomAttributes = propertyInfo.GetCustomAttributes(false);
				var propertyConverterAttribute = propertyCustomAttributes.OfType<JsonConverterAttribute>().FirstOrDefault();
				var propertyIsIgnored = propertyCustomAttributes.OfType<JsonIgnoreAttribute>().Any();
				var propertyName = propertyCustomAttributes.OfType<JsonPropertyNameAttribute>().FirstOrDefault()?.Name ?? propertyInfo.Name;
				var propertyValue = propertyInfo.GetValue(value);
				var propertyType = propertyInfo.PropertyType;

				// Skip the property if it's decorated with the 'ignore' attribute
				if (propertyIsIgnored) continue;

				// Ignore the property if it contains a null value
				if (propertyValue == null) continue;

				// Serialize the property.
				propertySerializer(propertyName, propertyValue, propertyType, options, propertyConverterAttribute);
			}

			writer.WriteEndObject();
		}

		internal void WriteJsonProperty(Utf8JsonWriter writer, string propertyName, object propertyValue, Type propertyType, JsonSerializerOptions options, JsonConverterAttribute propertyConverterAttribute)
		{
			writer.WritePropertyName(propertyName);

			if (propertyConverterAttribute != null)
			{
				// It's important to clone the options in order to be able to modify the 'Converters' list
				var clonedOptions = new JsonSerializerOptions(options);
				clonedOptions.Converters.Add((JsonConverter)Activator.CreateInstance(propertyConverterAttribute.ConverterType));
				JsonSerializer.Serialize(writer, propertyValue, propertyType, clonedOptions);
			}
			else
			{
				JsonSerializer.Serialize(writer, propertyValue, propertyType, options);
			}
		}
	}
}
