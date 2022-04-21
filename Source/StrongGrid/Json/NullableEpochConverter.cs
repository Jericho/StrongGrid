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
			if (reader.TokenType == JsonTokenType.Null) return null;
			var secondsSinceEpoch = reader.GetInt64();
			return secondsSinceEpoch.FromUnixTime();
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
