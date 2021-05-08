using Newtonsoft.Json;
using StrongGrid.Models;
using System;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Converts a MailSettings object to and from JSON.
	/// </summary>
	/// <seealso cref="Newtonsoft.Json.JsonConverter" />
	internal class MailSettingsConverter : JsonConverter
	{
		public MailSettingsConverter()
		{
		}

		/// <summary>
		/// Determines whether this instance can convert the specified object type.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns>
		/// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(MailSettings);
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON; otherwise, <c>false</c>.
		/// </value>
		public override bool CanRead
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON; otherwise, <c>false</c>.
		/// </value>
		public override bool CanWrite
		{
			get { return true; }
		}

		/// <summary>
		/// Writes the JSON representation of the object.
		/// </summary>
		/// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
		/// <param name="value">The value.</param>
		/// <param name="serializer">The calling serializer.</param>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null) return;

			writer.WriteStartObject();

			var mailSettings = (MailSettings)value;

			writer.WritePropertyName("bypass_list_management");
			writer.WriteStartObject();
			writer.WritePropertyName("enable");
			serializer.Serialize(writer, mailSettings.BypassListManagement);
			writer.WriteEndObject();

			writer.WritePropertyName("bypass_spam_management");
			writer.WriteStartObject();
			writer.WritePropertyName("enable");
			serializer.Serialize(writer, mailSettings.BypassSpamManagement);
			writer.WriteEndObject();

			writer.WritePropertyName("bypass_bounce_management");
			writer.WriteStartObject();
			writer.WritePropertyName("enable");
			serializer.Serialize(writer, mailSettings.BypassBounceManagement);
			writer.WriteEndObject();

			writer.WritePropertyName("bypass_unsubscribe_management");
			writer.WriteStartObject();
			writer.WritePropertyName("enable");
			serializer.Serialize(writer, mailSettings.BypassUnsubscribeManagement);
			writer.WriteEndObject();

			if (mailSettings.Footer != null)
			{
				writer.WritePropertyName("footer");
				serializer.Serialize(writer, mailSettings.Footer);
			}

			writer.WritePropertyName("sandbox_mode");
			writer.WriteStartObject();
			writer.WritePropertyName("enable");
			serializer.Serialize(writer, mailSettings.SandboxModeEnabled);
			writer.WriteEndObject();

			// End of JSON serialization
			writer.WriteEndObject();
		}

		/// <summary>
		/// Reads the JSON representation of the object.
		/// </summary>
		/// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="existingValue">The existing value of object being read.</param>
		/// <param name="serializer">The calling serializer.</param>
		/// <returns>
		/// The object value.
		/// </returns>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotSupportedException("The MailSettingsConverter can only be used to write JSON");
		}
	}
}
