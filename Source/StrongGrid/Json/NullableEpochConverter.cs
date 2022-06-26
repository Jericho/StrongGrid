using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Json
{
	/// <summary>
	/// Converts a 'Unix time' expressed as the number of seconds since midnight on January 1st 1970 to and from JSON.
	/// </summary>
	/// <seealso cref="JsonConverter" />
	internal class NullableEpochConverter : BaseJsonConverter<DateTime?>
	{
		public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			switch (reader.TokenType)
			{
				case JsonTokenType.None:
				case JsonTokenType.Null:
				case JsonTokenType.String when string.IsNullOrEmpty(reader.GetString()):
					return null;
				case JsonTokenType.Number:
					var secondsSinceEpoch = reader.GetInt64();
					return secondsSinceEpoch.FromUnixTime();
				default:
					throw new Exception($"Unable to convert {reader.TokenType.ToEnumString()} to nullable DateTime");
			}
		}

		public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
		{
			if (value.HasValue)
			{
				var secondsSinceEpoch = value.Value.ToUnixTime();
				writer.WriteNumberValue(secondsSinceEpoch);
			}
			else
			{
				writer.WriteNullValue();
			}
		}
	}
}
