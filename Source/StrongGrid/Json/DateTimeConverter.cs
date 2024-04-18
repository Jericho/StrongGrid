using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Json
{
	/// <summary>
	/// Converts a <see cref="DateTime" /> expressed in a format acceptable to SendGrid to and from JSON.
	/// </summary>
	/// <seealso cref="JsonConverter" />
	internal class DateTimeConverter : BaseJsonConverter<DateTime>
	{
		private static readonly CultureInfo Format_Provider = new CultureInfo("en-US");
		private static readonly DateTimeStyles DateTime_Style = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;

		public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			switch (reader.TokenType)
			{
				case JsonTokenType.None:
				case JsonTokenType.Null:
					throw new JsonException($"Unable to convert a null value into a date");

				case JsonTokenType.String when string.IsNullOrEmpty(reader.GetString()):
					throw new JsonException($"Unable to convert an empty string into a date.");

				case JsonTokenType.String:
					// First we try to use the built-in converter.
					// This works when the data from the SendGrid endpoint has been formatted with a standard format
					if (reader.TryGetDateTime(out DateTime simpleDateTime)) return simpleDateTime;

					// The built-in converter was unable to deserialize the data presumably because it's formatted in an unexpected way.
					// Therefore we need to guess the format that SenGrid is using.
					return ConvertToDateTime(reader.GetString());

				default:
					throw new JsonException($"Unable to convert \"{reader.TokenType.ToEnumString()}\" into a date");
			}
		}

		public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
		}

		private static DateTime ConvertToDateTime(string dateAsString)
		{
			if (DateTime.TryParseExact(dateAsString, "yyyy-MM-dd HH:mm:ss zzz 'UTC'", Format_Provider, DateTime_Style, out DateTime sendGridDateTimeWithOffset))
			{
				return sendGridDateTimeWithOffset;
			}
			else if (DateTime.TryParseExact(dateAsString, "yyyy-MM-dd HH:mm:ss", Format_Provider, DateTime_Style, out DateTime sendGridDateWithTime))
			{
				return sendGridDateWithTime;
			}
			else
			{
				throw new JsonException($"Unable to convert \"{dateAsString}\" into a date");
			}
		}
	}
}
