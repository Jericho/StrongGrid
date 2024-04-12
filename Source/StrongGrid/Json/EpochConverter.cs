using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Json
{
	/// <summary>
	/// Converts a 'Unix time' expressed as the number of seconds since midnight on January 1st 1970 to and from JSON.
	/// </summary>
	/// <seealso cref="JsonConverter" />
	internal class EpochConverter : BaseJsonConverter<DateTime>
	{
		public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			switch (reader.TokenType)
			{
				case JsonTokenType.None:
				case JsonTokenType.Null:
					throw new JsonException($"Unable to convert a null value into a date");

				case JsonTokenType.Number:
					var secondsSinceEpoch = reader.GetInt64();
					return secondsSinceEpoch.FromUnixTime();

				default:
					throw new JsonException($"Unable to convert {reader.TokenType.ToEnumString()} into a date");
			}
		}

		public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
		{
			var secondsSinceEpoch = value.ToUnixTime();
			writer.WriteNumberValue(secondsSinceEpoch);
		}
	}
}
