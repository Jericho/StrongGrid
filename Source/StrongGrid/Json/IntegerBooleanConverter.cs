using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Json
{
	/// <summary>
	/// Converts a boolean expressed as 1 for True or 0 for False to and from JSON.
	/// </summary>
	internal class IntegerBooleanConverter : JsonConverter<bool>
	{
		public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.Number) return false;
			return reader.GetInt32() == 1;
		}

		public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
		{
			writer.WriteNumberValue(value ? 1 : 0);
		}
	}
}
