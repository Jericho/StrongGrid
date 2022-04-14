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
	internal class MailSettingsConverter : BaseJsonConverter<MailSettings>
	{
		public MailSettingsConverter()
		{
		}

		public override MailSettings Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (JsonDocument.TryParseValue(ref reader, out var doc))
			{
				FooterSettings footer = null;
				if (doc.RootElement.TryGetProperty("footer", out var footerProperty))
				{
					footer = footerProperty.ToObject<FooterSettings>(options);
				}

				return new MailSettings()
				{
					BypassBounceManagement = GetEnabledPropertyValue(doc.RootElement, "bypass_bounce_management", false),
					BypassListManagement = GetEnabledPropertyValue(doc.RootElement, "bypass_list_management", false),
					BypassSpamManagement = GetEnabledPropertyValue(doc.RootElement, "bypass_spam_management", false),
					BypassUnsubscribeManagement = GetEnabledPropertyValue(doc.RootElement, "bypass_unsubscribe_management", false),
					Footer = footer,
					SandboxModeEnabled = GetEnabledPropertyValue(doc.RootElement, "sandbox_mode", false)
				};
			}

			return null;
		}

		public override void Write(Utf8JsonWriter writer, MailSettings value, JsonSerializerOptions options)
		{
			Serialize(writer, value, options, (propertyName, propertyValue, propertyType, options, propertyConverterAttribute) =>
			{
				// bypass_xxx_management should be omited when the value is 'false'.
				// See https://github.com/Jericho/StrongGrid/issues/395 for details.
				if (propertyName == "bypass_list_management")
				{
					if ((bool)propertyValue) WriteEnabledProperty(writer, propertyName, true);
				}
				else if (propertyName == "bypass_spam_management")
				{
					if ((bool)propertyValue) WriteEnabledProperty(writer, propertyName, true);
				}
				else if (propertyName == "bypass_bounce_management")
				{
					if ((bool)propertyValue) WriteEnabledProperty(writer, propertyName, true);
				}
				else if (propertyName == "bypass_unsubscribe_management")
				{
					if ((bool)propertyValue) WriteEnabledProperty(writer, propertyName, true);
				}
				else if (propertyName == "sandbox_mode")
				{
					WriteEnabledProperty(writer, propertyName, (bool)propertyValue);
				}

				// Any other property
				else
				{
					WriteJsonProperty(writer, propertyName, propertyValue, propertyType, options, propertyConverterAttribute);
				}
			});
		}

		private static void WriteEnabledProperty(Utf8JsonWriter writer, string propertyName, bool value)
		{
			writer.WritePropertyName(propertyName);
			writer.WriteStartObject();
			writer.WritePropertyName("enable");
			writer.WriteBooleanValue(value);
			writer.WriteEndObject();
		}

		private static bool GetEnabledPropertyValue(JsonElement parentProperty, string propertyName, bool defaultValue)
		{
			if (!parentProperty.TryGetProperty(propertyName, out var property)) return defaultValue;
			if (!property.TryGetProperty("enabled", out var enabledProperty)) return defaultValue;
			return enabledProperty.GetBoolean();
		}
	}
}
