using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Converts a <see cref="DateTime" /> expressed in a format acceptable to SendGrid to and from JSON.
	/// </summary>
	/// <seealso cref="Newtonsoft.Json.Converters.DateTimeConverterBase" />
	internal class SendGridDateTimeConverter : DateTimeConverterBase
	{
		private const string SENDGRID_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

		/// <summary>
		/// Determines whether this instance can convert the specified object type.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns>
		/// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(DateTime);
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
			writer.WriteValue(((DateTime)value).ToString(SENDGRID_DATETIME_FORMAT));
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
			if (reader.Value == null) return null;
			var date = DateTime.ParseExact(reader.Value.ToString(), SENDGRID_DATETIME_FORMAT, new CultureInfo("en-US"), DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
			return date;
		}
	}
}
