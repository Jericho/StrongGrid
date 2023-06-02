using System;
using System.Collections;
using System.Linq;
using System.Reflection;
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

		public virtual void SerializeProperty(Utf8JsonWriter writer, object propertyValue, PropertyInfo propertyInfo, JsonSerializerOptions options, Action<string, object, Type, JsonSerializerOptions, JsonConverterAttribute> propertySerializer)
		{
			var propertyCustomAttributes = propertyInfo.GetCustomAttributes(false);
			var propertyConverterAttribute = propertyCustomAttributes.OfType<JsonConverterAttribute>().FirstOrDefault();
			var propertyIsIgnored = propertyCustomAttributes.OfType<JsonIgnoreAttribute>().Any();
			var propertyName = propertyCustomAttributes.OfType<JsonPropertyNameAttribute>().FirstOrDefault()?.Name ?? propertyInfo.Name;
			var propertyType = propertyInfo.PropertyType;

			// Skip the property if it's decorated with the 'ignore' attribute
			if (propertyIsIgnored) return;

			// Ignore the property if it contains a null value
			else if (propertyValue == null) return;

			// Serialize the property.
			else propertySerializer(propertyName, propertyValue, propertyType, options, propertyConverterAttribute);

		}

		internal void Serialize(Utf8JsonWriter writer, T value, JsonSerializerOptions options, Action<string, object, Type, JsonSerializerOptions, JsonConverterAttribute> propertySerializer = null)
		{
			Serialize(writer, value, value.GetType(), options, propertySerializer);
		}

		internal void Serialize(Utf8JsonWriter writer, T value, Type typeOfValue, JsonSerializerOptions options, Action<string, object, Type, JsonSerializerOptions, JsonConverterAttribute> propertySerializer = null)
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
				SerializeProperty(writer, propertyInfo.GetValue(value), propertyInfo, options, propertySerializer);
			}

			writer.WriteEndObject();
		}

		internal void WriteJsonProperty(Utf8JsonWriter writer, string propertyName, object propertyValue, Type propertyType, JsonSerializerOptions options, JsonConverterAttribute propertyConverterAttribute)
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
