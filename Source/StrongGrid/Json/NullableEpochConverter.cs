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
		private readonly EpochConverter _epochConverter = new EpochConverter();

		public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			switch (reader.TokenType)
			{
				case JsonTokenType.None:
				case JsonTokenType.Null:
					return null;

				default:
					return _epochConverter.Read(ref reader, typeToConvert, options);
			}
		}

		public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
		{
			if (!value.HasValue) writer.WriteNullValue();
			else _epochConverter.Write(writer, value.Value, options);
		}
	}
}
