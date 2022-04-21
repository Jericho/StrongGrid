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
	internal class SendGridDateTimeConverter : BaseJsonConverter<DateTime>
	{
		private const string SENDGRID_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
		private const string SENDGRID_DATEONLY_FORMAT = "yyyy-MM-dd";

		public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var provider = new CultureInfo("en-US");
			var style = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;
			var dateAsString = reader.GetString();

			if (DateTime.TryParseExact(dateAsString, SENDGRID_DATETIME_FORMAT, provider, style, out DateTime sendGridDateAndTime))
			{
				return sendGridDateAndTime;
			}
			else if (DateTime.TryParseExact(dateAsString, SENDGRID_DATEONLY_FORMAT, provider, style, out DateTime sendGridDateOnly))
			{
				return sendGridDateOnly;
			}
			else
			{
				throw new Exception("Unable to convert JSON value into a date.");
			}
		}

		public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToString(SENDGRID_DATETIME_FORMAT));
		}
	}
}
