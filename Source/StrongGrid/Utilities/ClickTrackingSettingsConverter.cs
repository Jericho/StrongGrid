using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StrongGrid.Models;
using System;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Converts a <see cref="ClickTrackingSettings"/> to and from JSON.
	/// This converter is necessary because one of the JSON attributes is
	/// sometimes spelled "enabled" and sometimes spelled "enable" depending
	/// on which endpoint in SendGrid's API you are invoking.
	/// </summary>
	/// <seealso cref="Newtonsoft.Json.JsonConverter" />
	internal class ClickTrackingSettingsConverter : JsonConverter
	{
		/// <summary>
		/// Determines whether this instance can convert the specified object type.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns>
		/// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanConvert(Type objectType) => objectType == typeof(ClickTrackingSettings);

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON; otherwise, <c>false</c>.
		/// </value>
		public override bool CanRead => true;

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON; otherwise, <c>false</c>.
		/// </value>
		public override bool CanWrite => false;

		/// <summary>
		/// Writes the JSON representation of the object.
		/// </summary>
		/// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
		/// <param name="value">The value.</param>
		/// <param name="serializer">The calling serializer.</param>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
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
		/// <exception cref="System.Exception">Unable to determine the field type.</exception>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.StartObject)
			{
				var jObject = JObject.Load(reader);

				jObject.TryGetValue("enable_text", out JToken enableInTextProperty);
				jObject.TryGetValue("enable", out JToken enableInHtmlProperty);
				jObject.TryGetValue("enabled", out JToken enabledInHtmlProperty);

				var enabledInText = enableInTextProperty?.Value<bool>() ?? false;
				var enabledInHtml = enableInHtmlProperty?.Value<bool>() ?? enabledInHtmlProperty?.Value<bool>() ?? false;

				return new ClickTrackingSettings()
				{
					EnabledInTextContent = enabledInText,
					EnabledInHtmlContent = enabledInHtml
				};
			}

			return null;
		}
	}
}
