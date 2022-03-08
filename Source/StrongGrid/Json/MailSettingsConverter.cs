using StrongGrid.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Json
{
	/// <summary>
	/// Converts a MailSettings object to and from JSON.
	/// </summary>
	/// <seealso cref="JsonConverter" />
	internal class MailSettingsConverter : JsonConverter<MailSettings>
	{
		public MailSettingsConverter()
		{
		}

		public override MailSettings Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			throw new NotSupportedException("The MailSettingsConverter can only be used to write JSON");
		}

		public override void Write(Utf8JsonWriter writer, MailSettings value, JsonSerializerOptions options)
		{
			if (value == null) return;

			writer.WriteStartObject();

			// bypass_xxx_management should be omited when the value is 'false'.
			// See https://github.com/Jericho/StrongGrid/issues/395 for details.
			if (value.BypassListManagement) WriteEnabledProperty(writer, "bypass_list_management", true);
			if (value.BypassSpamManagement) WriteEnabledProperty(writer, "bypass_spam_management", true);
			if (value.BypassBounceManagement) WriteEnabledProperty(writer, "bypass_bounce_management", true);
			if (value.BypassUnsubscribeManagement) WriteEnabledProperty(writer, "bypass_unsubscribe_management", true);

			if (value.Footer != null)
			{
				writer.WritePropertyName("footer");
				JsonSerializer.Serialize(writer, value.Footer, typeof(FooterSettings), options);
			}

			WriteEnabledProperty(writer, "sandbox_mode", value.SandboxModeEnabled);

			// End of JSON serialization
			writer.WriteEndObject();
		}

		private static void WriteEnabledProperty(Utf8JsonWriter writer, string propertyName, bool value)
		{
			writer.WritePropertyName(propertyName);
			writer.WriteStartObject();
			writer.WritePropertyName("enable");
			writer.WriteBooleanValue(value);
			writer.WriteEndObject();
		}
	}
}
