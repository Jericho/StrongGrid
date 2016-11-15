using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StrongGrid.Model;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace StrongGrid.Utilities
{
	public class EnumDescriptionConverter : StringEnumConverter
	{

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var isNullable = objectType.GetTypeInfo().IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>);
			var enumType = isNullable ? Nullable.GetUnderlyingType(objectType) : objectType;

			if (reader.TokenType == JsonToken.Null)
			{
				if (!isNullable)
				{
					throw new JsonSerializationException(string.Format("Cannot convert null value to {0}.", objectType));
				}

				return null;
			}

			try
			{
				if (reader.TokenType == JsonToken.String)
				{
					var enumText = reader.Value.ToString();

					var enumVal = objectType.GetMembers()
						.Where(x => x.GetCustomAttributes(typeof(DescriptionAttribute)).Any())
						.FirstOrDefault(x => ((DescriptionAttribute)x.GetCustomAttribute(typeof(DescriptionAttribute))).Description == enumText);

					if (enumVal == null) return Enum.Parse(enumType, enumText);

					return Enum.Parse(enumType, enumVal.Name);
				}
			}
			catch (Exception ex)
			{
				throw new JsonSerializationException(string.Format("Error converting value {0} to type '{1}'.", reader.Value, objectType), ex);
			}

			throw new JsonSerializationException(string.Format("Unexpected token {0} when parsing enum.", reader.TokenType));
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}

			var enumDescription = ((Enum)value).GetDescription();
			writer.WriteValue(enumDescription);
		}
	}
}
