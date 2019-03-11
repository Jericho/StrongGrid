using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StrongGrid.Models;
using StrongGrid.Models.EmailActivities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Converts a JSON string into and array of <see cref="Event">events</see>.
	/// </summary>
	/// <seealso cref="Newtonsoft.Json.JsonConverter" />
	internal class EmailActivityEventConverter : JsonConverter
	{
		/// <summary>
		/// Determines whether this instance can convert the specified object type.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns>
		/// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Event);
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON; otherwise, <c>false</c>.
		/// </value>
		public override bool CanRead
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON; otherwise, <c>false</c>.
		/// </value>
		public override bool CanWrite
		{
			get { return false; }
		}

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
			if (reader.TokenType == JsonToken.StartArray)
			{
				var jArray = JArray.Load(reader);
				var items = jArray
					.OfType<JObject>()
					.Select(item => Convert(item, serializer))
					.Where(item => item != null)
					.ToArray();

				return items;
			}
			else if (reader.TokenType == JsonToken.StartObject)
			{
				var jObject = JObject.Load(reader);
				return Convert(jObject, serializer);
			}

			throw new Exception("Unable to convert to Event");
		}

		private Event Convert(JObject jsonObject, JsonSerializer serializer)
		{
			jsonObject.TryGetValue("event_name", StringComparison.OrdinalIgnoreCase, out JToken eventTypeJsonProperty);
			var eventType = (EventType)eventTypeJsonProperty.ToObject(typeof(EventType));

			var emailActivityEvent = (Event)null;
			switch (eventType)
			{
				case EventType.Bounce:
					emailActivityEvent = jsonObject.ToObject<BounceEvent>(serializer);
					break;
				case EventType.Open:
					emailActivityEvent = jsonObject.ToObject<OpenEvent>(serializer);
					break;
				case EventType.Click:
					emailActivityEvent = jsonObject.ToObject<ClickEvent>(serializer);
					break;
				case EventType.Processed:
					emailActivityEvent = jsonObject.ToObject<ProcessedEvent>(serializer);
					break;
				case EventType.Dropped:
					emailActivityEvent = jsonObject.ToObject<DroppedEvent>(serializer);
					break;
				case EventType.Delivered:
					emailActivityEvent = jsonObject.ToObject<DeliveredEvent>(serializer);
					break;
				case EventType.Deferred:
					emailActivityEvent = jsonObject.ToObject<DeferredEvent>(serializer);
					break;
				case EventType.SpamReport:
					emailActivityEvent = jsonObject.ToObject<SpamReportEvent>(serializer);
					break;
				case EventType.Unsubscribe:
					emailActivityEvent = jsonObject.ToObject<UnsubscribeEvent>(serializer);
					break;
				case EventType.GroupUnsubscribe:
					emailActivityEvent = jsonObject.ToObject<GroupUnsubscribeEvent>(serializer);
					break;
				case EventType.GroupResubscribe:
					emailActivityEvent = jsonObject.ToObject<GroupResubscribeEvent>(serializer);
					break;
				default:
					throw new Exception($"{eventTypeJsonProperty.ToString()} is an unknown event type");
			}

			return emailActivityEvent;
		}
	}
}
