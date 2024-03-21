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
		private const string SENDGRID_DATETIME_UTC_FORMAT = "yyyy-MM-dd'T'HH:mm:ssZ";
		private const string SENDGRID_DATETIME_WITH_OFFSET_FORMAT = "yyyy-MM-dd HH:mm:ss zzz 'UTC'";
		private const string SENDGRID_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
		private const string SENDGRID_DATEONLY_FORMAT = "yyyy-MM-dd";

		public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var provider = new CultureInfo("en-US");
			var style = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;
			var dateAsString = reader.GetString();

			/*
			 * Most of the time, the SendGrid API returns dates as strings which look like this: 2024-02-14T15:47:29Z
			 * which the System.Text.Json seriaizer is able to desiralize without any problems. If the SendGrid API
			 * consistently used this format, there would be no need for this custom converter.
			 *
			 * However, I have observed varuious formats used by different endpoints:
			 *   - sometimes they include only the date: 2024-03-21
			 *   - sometimes they include the date and time: 2024-03-21 15:21:29
			 *   - sometimes they use the ISO format: 2024-02-14T15:47:29Z
			 *   - sometimes they use a custom format that includes timezone offset: 2024-02-14 15:47:29 +0000 UTC
			 *
			 * To make maters even more complicated, I have observed one specific endpoint (/v3/marketing/contacts/search)
			 * intermitently switching between formats.
			*/

			if (DateTime.TryParseExact(dateAsString, SENDGRID_DATETIME_UTC_FORMAT, provider, style, out DateTime sendGridDateTimeUtc))
			{
				return sendGridDateTimeUtc;
			}
			else if (DateTime.TryParseExact(dateAsString, SENDGRID_DATETIME_WITH_OFFSET_FORMAT, provider, style, out DateTime sendGridDateTimeWithOffset))
			{
				return sendGridDateTimeWithOffset;
			}
			else if (DateTime.TryParseExact(dateAsString, SENDGRID_DATETIME_FORMAT, provider, style, out DateTime sendGridDateAndTime))
			{
				return sendGridDateAndTime;
			}
			else if (DateTime.TryParseExact(dateAsString, SENDGRID_DATEONLY_FORMAT, provider, style, out DateTime sendGridDateOnly))
			{
				return sendGridDateOnly;
			}
			else
			{
				throw new Exception($"SendGridDateTimeConverter in unable to convert \"{dateAsString}\" into a date.");
			}
		}

		public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToString(SENDGRID_DATETIME_FORMAT));
		}
	}
}
