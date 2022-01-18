using StrongGrid.Models;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Converts a <see cref="ClickTrackingSettings"/> to and from JSON.
	/// This converter is necessary because one of the JSON attributes is
	/// sometimes spelled "enabled" and sometimes spelled "enable" depending
	/// on which endpoint in SendGrid's API you are invoking.
	/// </summary>
	/// <seealso cref="JsonConverter" />
	internal class ClickTrackingSettingsConverter : JsonConverter<ClickTrackingSettings>
	{
		public override ClickTrackingSettings Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (JsonDocument.TryParseValue(ref reader, out var doc))
			{
				var enabledInText = false;
				var enabledInHtml = false;

				if (doc.RootElement.TryGetProperty("enable_text", out var enableInTextProperty))
				{
					enabledInText = enableInTextProperty.GetBoolean();
				}

				// First we look for an element called 'enable'
				if (doc.RootElement.TryGetProperty("enable", out var enableInHtmlProperty))
				{
					enabledInHtml = enableInHtmlProperty.GetBoolean();
				}

				// If we did not find 'enable', we look for 'enabled'
				else if (doc.RootElement.TryGetProperty("enabled", out var enabledInHtmlProperty))
				{
					enabledInHtml = enabledInHtmlProperty.GetBoolean();
				}

				return new ClickTrackingSettings()
				{
					EnabledInTextContent = enabledInText,
					EnabledInHtmlContent = enabledInHtml
				};
			}

			return null;
		}

		public override void Write(Utf8JsonWriter writer, ClickTrackingSettings value, JsonSerializerOptions options)
		{
			var valueType = typeof(ClickTrackingSettings);

			writer.WriteStartObject();
			writer.WriteBoolean(((JsonPropertyNameAttribute)valueType.GetProperty("EnabledInTextContent").GetCustomAttributes(typeof(JsonPropertyNameAttribute), false).Single()).Name, value.EnabledInTextContent);
			writer.WriteBoolean(((JsonPropertyNameAttribute)valueType.GetProperty("EnabledInHtmlContent").GetCustomAttributes(typeof(JsonPropertyNameAttribute), false).Single()).Name, value.EnabledInHtmlContent);
			writer.WriteEndObject();
		}
	}
}
