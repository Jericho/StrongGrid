using StrongGrid.Models;
using StrongGrid.Models.EmailActivities;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Json
{
	/// <summary>
	/// Converts a JSON string into and array of <see cref="Event">events</see>.
	/// </summary>
	/// <seealso cref="JsonConverter" />
	internal class EmailActivityEventConverter : BaseJsonConverter<Event[]>
	{
		public override Event[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (JsonDocument.TryParseValue(ref reader, out var doc))
			{
				if (doc.RootElement.ValueKind == JsonValueKind.Array)
				{
					return doc.RootElement.EnumerateArray()
						.Select(item => Convert(item, options))
						.ToArray();
				}
				else if (doc.RootElement.ValueKind == JsonValueKind.Object)
				{
					return new[] { Convert(doc.RootElement, options) };
				}
			}

			throw new Exception("Unable to convert to Event(s)");
		}

		private static Event Convert(JsonElement jsonElement, JsonSerializerOptions options)
		{
			jsonElement.TryGetProperty("event_name", out JsonElement eventTypeProperty);
			var eventTypeAsString = eventTypeProperty.GetString();
			var eventType = eventTypeAsString.ToEnum<EventType>();

			Event emailActivityEvent;
			switch (eventType)
			{
				case EventType.Bounce:
					emailActivityEvent = jsonElement.ToObject<BounceEvent>(options);
					break;
				case EventType.Open:
					emailActivityEvent = jsonElement.ToObject<OpenEvent>(options);
					break;
				case EventType.Click:
					emailActivityEvent = jsonElement.ToObject<ClickEvent>(options);
					break;
				case EventType.Processed:
					emailActivityEvent = jsonElement.ToObject<ProcessedEvent>(options);
					break;
				case EventType.Dropped:
					emailActivityEvent = jsonElement.ToObject<DroppedEvent>(options);
					break;
				case EventType.Delivered:
					emailActivityEvent = jsonElement.ToObject<DeliveredEvent>(options);
					break;
				case EventType.Deferred:
					emailActivityEvent = jsonElement.ToObject<DeferredEvent>(options);
					break;
				case EventType.SpamReport:
					emailActivityEvent = jsonElement.ToObject<SpamReportEvent>(options);
					break;
				case EventType.Unsubscribe:
					emailActivityEvent = jsonElement.ToObject<UnsubscribeEvent>(options);
					break;
				case EventType.GroupUnsubscribe:
					emailActivityEvent = jsonElement.ToObject<GroupUnsubscribeEvent>(options);
					break;
				case EventType.GroupResubscribe:
					emailActivityEvent = jsonElement.ToObject<GroupResubscribeEvent>(options);
					break;
				default:
					throw new Exception($"{eventTypeAsString} is an unknown event type");
			}

			return emailActivityEvent;
		}
	}
}
