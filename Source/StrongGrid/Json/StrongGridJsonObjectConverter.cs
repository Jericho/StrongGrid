using System;
using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Json
{
	/// <summary>
	/// Converts a JsonObject to and from JSON.
	/// </summary>
	/// <seealso cref="JsonConverter" />
	internal class StrongGridJsonObjectConverter : JsonConverter<StrongGridJsonObject>
	{
		public override StrongGridJsonObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (JsonDocument.TryParseValue(ref reader, out var doc))
			{
				if (doc.RootElement.ValueKind == JsonValueKind.Object)
				{
					var jsonObject = new StrongGridJsonObject();
					foreach (var property in doc.RootElement.EnumerateObject())
					{
						switch (property.Value.ValueKind)
						{
							/*
							case JsonValueKind.Array:
								AddDeepProperty(propertyName, property.Value.GetString());
								break;
							*/

							case JsonValueKind.False:
								jsonObject.AddProperty(property.Name, false);
								break;

							/*
							case JsonValueKind.Null:
								AddDeepProperty(propertyName, value.GetString());
								break;
							*/

							/*
							case JsonValueKind.Number:
								AddDeepProperty(propertyName, value.getn.GetString());
								break;
							*/

							/*
							case JsonValueKind.Object:
								AddDeepProperty(propertyName, value.GetString());
								break;
							*/

							case JsonValueKind.String:
								jsonObject.AddProperty(property.Name, property.Value.GetString());
								break;

							case JsonValueKind.True:
								jsonObject.AddProperty(property.Name, true);
								break;

							/*
							case JsonValueKind.Undefined:
								AddDeepProperty(propertyName, value.GetString());
								break;
							*/

							default:
								throw new Exception($"Don't know how to handle {property.Name} because its value type is {property.Value.ValueKind}");
						}
					}

					return jsonObject;
				}
			}

			return null;
		}

		public override void Write(Utf8JsonWriter writer, StrongGridJsonObject value, JsonSerializerOptions options)
		{
			if (value == null) return;

			writer.WriteStartObject();

			foreach (var pair in value.Properties)
			{
				writer.WritePropertyName(pair.Key);
				WriteValue(writer, pair.Value, options);
			}

			writer.WriteEndObject();
		}

		private void WriteValue(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
		{
			if (value is bool boolValue)
			{
				writer.WriteBooleanValue(boolValue);
			}
			else if (value is string strValue)
			{
				writer.WriteStringValue(strValue);
			}
			else if (value is decimal decimalValue)
			{
				writer.WriteNumberValue(decimalValue);
			}
			else if (value is double doubleValue)
			{
				writer.WriteNumberValue(doubleValue);
			}
			else if (value is float floatValue)
			{
				writer.WriteNumberValue(floatValue);
			}
			else if (value is int intValue)
			{
				writer.WriteNumberValue(intValue);
			}
			else if (value is long longValue)
			{
				writer.WriteNumberValue(longValue);
			}
			else if (value is uint uintValue)
			{
				writer.WriteNumberValue(uintValue);
			}
			else if (value is ulong ulongValue)
			{
				writer.WriteNumberValue(ulongValue);
			}
			else if (value is StrongGridJsonObject jsonObject)
			{
				Write(writer, jsonObject, options);
			}
			else if (value is IEnumerable enumerableOject)
			{
				writer.WriteStartArray();

				foreach (var item in enumerableOject)
				{
					WriteValue(writer, item, options);
				}

				writer.WriteEndArray();
			}
			else
			{
				try
				{
					JsonSerializer.Serialize(writer, value, options);
				}
				catch (Exception e)
				{
					throw new Exception($"Unable to write a value of type {value.GetType().FullName}", e);
				}
			}
		}
	}
}
