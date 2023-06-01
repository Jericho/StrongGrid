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

		public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (JsonDocument.TryParseValue(ref reader, out var doc))
			{
				var obj = doc.RootElement.ToObject<T>(options);
				return obj;
			}

			return default;
		}

		public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
		{
			if (value == null) return;

			var typeOfValue = value.GetType(); // Do not use typeof(T). See https://github.com/Jericho/StrongGrid/issues/492
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
				Serialize(writer, value, typeOfValue, options, null);
			}
		}

		internal static void Serialize(Utf8JsonWriter writer, T value, JsonSerializerOptions options, Action<string, object, Type, JsonSerializerOptions, JsonConverterAttribute> propertySerializer = null)
		{
			Serialize(writer, value, value.GetType(), options, propertySerializer);
		}

		internal static void Serialize(Utf8JsonWriter writer, T value, Type typeOfValue, JsonSerializerOptions options, Action<string, object, Type, JsonSerializerOptions, JsonConverterAttribute> propertySerializer = null)
		{
			if (value == null)
			{
				writer.WriteNullValue();
				return;
			}

			propertySerializer ??= (propertyName, propertyValue, propertyType, options, propertyConverterAttribute) => WriteJsonProperty(writer, propertyName, propertyValue, propertyType, options, propertyConverterAttribute);

			writer.WriteStartObject();

			var allProperties = typeOfValue.GetProperties();

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

		internal static void WriteJsonProperty(Utf8JsonWriter writer, string propertyName, object propertyValue, Type propertyType, JsonSerializerOptions options, JsonConverterAttribute propertyConverterAttribute)
		{
			writer.WritePropertyName(propertyName);

			if (propertyConverterAttribute != null)
			{
				var customOptions = new JsonSerializerOptions()
				{
					Converters = { (JsonConverter)Activator.CreateInstance(propertyConverterAttribute.ConverterType) }
				};
				JsonSerializer.Serialize(writer, propertyValue, propertyType, customOptions);
			}
			else
			{
				JsonSerializer.Serialize(writer, propertyValue, propertyType, options);
			}
		}
	}
}
