using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StrongGrid.Model.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrongGrid.Utilities
{
	public class WebHookEventConverter : JsonConverter
	{
		private static readonly string[] _knownProperties =
		{
			"asm_group_id",
			"attempt",
			"category",
			"cert_err",
			"email",
			"event",
			"ip",
			"newsletter",
			"reason",
			"response",
			"send_at",
			"sg_event_id",
			"sg_message_id",
			"smtp-id",
			"status",
			"template",
			"timestamp",
			"tls",
			"type",
			"url",
			"url_offset",
			"useragent",
			"userid",
			"pool"
		};

		/// <summary>
		/// Determines whether this instance can convert the specified object type.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns>
		/// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Event) || objectType == typeof(Event[]);
		}

		public override bool CanRead
		{
			get { return true; }
		}

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
		/// <exception cref="System.Exception">Unable to determine the field type</exception>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jsonObject = JObject.Load(reader);

			JToken eventTypeJsonProperty = null;
			jsonObject.TryGetValue("event", StringComparison.OrdinalIgnoreCase, out eventTypeJsonProperty);

			var eventType = (EventType)eventTypeJsonProperty.ToObject(typeof(EventType));

			var webHookEvent = (Event)null;
			switch (eventType)
			{
				case EventType.Bounce:
					webHookEvent = jsonObject.ToObject<BouncedEvent>(serializer);
					break;
				case EventType.Click:
					webHookEvent = jsonObject.ToObject<ClickEvent>(serializer);
					break;
				case EventType.Deferred:
					webHookEvent = jsonObject.ToObject<DeferredEvent>(serializer);
					break;
				case EventType.Delivered:
					webHookEvent = jsonObject.ToObject<DeliveredEvent>(serializer);
					break;
				case EventType.Dropped:
					webHookEvent = jsonObject.ToObject<DroppedEvent>(serializer);
					break;
				case EventType.Open:
					webHookEvent = jsonObject.ToObject<OpenEvent>(serializer);
					break;
				case EventType.Processed:
					webHookEvent = jsonObject.ToObject<ProcessedEvent>(serializer);
					break;
				default:
					throw new Exception($"{eventTypeJsonProperty.ToString()} is an unknown type of event");
			}

			var properties = jsonObject
				.ToObject<Dictionary<string, object>>()
				.Where(p => !_knownProperties.Contains(p.Key));

			foreach (var prop in properties)
			{
				webHookEvent.UniqueArguments.Add(prop.Key, prop.Value == null ? null : prop.Value.ToString());
			}

			return webHookEvent;
		}
	}
}
