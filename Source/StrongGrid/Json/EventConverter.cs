using StrongGrid.Models;
using StrongGrid.Models.Webhooks;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Json
{
	/// <summary>
	/// Converts a JSON string received from a webhook into and array of <see cref="Event">events</see>.
	/// </summary>
	/// <seealso cref="JsonConverter" />
	internal class EventConverter : BaseJsonConverter<Event>
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
			"marketing_campaign_id",
			"marketing_campaign_name",
			"marketing_campaign_split_id",
			"marketing_campaign_version",
			"newsletter",
			"pool",
			"reason",
			"response",
			"send_at",
			"sg_content_type",
			"sg_event_id",
			"sg_machine_open",
			"sg_message_id",
			"sg_user_id",
			"smtp-id",
			"status",
			"template",
			"timestamp",
			"tls",
			"type",
			"url",
			"url_offset",
			"useragent"
		};

		public override bool CanConvert(Type type)
		{
			return type.IsAssignableFrom(typeof(Event));
		}

		public override Event Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (JsonDocument.TryParseValue(ref reader, out var doc))
			{
				if (doc.RootElement.TryGetProperty("event", out var type))
				{
					var typeAsString = type.GetString();
					var eventType = typeAsString.ToEnum<EventType>();

					var rootElement = doc.RootElement.GetRawText();

					var webHookEvent = (Event)null;
					switch (eventType)
					{
						case EventType.Bounce:
							webHookEvent = JsonSerializer.Deserialize<BouncedEvent>(rootElement, JsonFormatter.DeserializerOptions);
							break;
						case EventType.Click:
							webHookEvent = JsonSerializer.Deserialize<ClickedEvent>(rootElement, JsonFormatter.DeserializerOptions);
							break;
						case EventType.Deferred:
							webHookEvent = JsonSerializer.Deserialize<DeferredEvent>(rootElement, JsonFormatter.DeserializerOptions);
							break;
						case EventType.Delivered:
							webHookEvent = JsonSerializer.Deserialize<DeliveredEvent>(rootElement, JsonFormatter.DeserializerOptions);
							break;
						case EventType.Dropped:
							webHookEvent = JsonSerializer.Deserialize<DroppedEvent>(rootElement, JsonFormatter.DeserializerOptions);
							break;
						case EventType.GroupResubscribe:
							webHookEvent = JsonSerializer.Deserialize<GroupResubscribeEvent>(rootElement, JsonFormatter.DeserializerOptions);
							break;
						case EventType.GroupUnsubscribe:
							webHookEvent = JsonSerializer.Deserialize<GroupUnsubscribeEvent>(rootElement, JsonFormatter.DeserializerOptions);
							break;
						case EventType.Open:
							webHookEvent = JsonSerializer.Deserialize<OpenedEvent>(rootElement, JsonFormatter.DeserializerOptions);
							break;
						case EventType.Processed:
							webHookEvent = JsonSerializer.Deserialize<ProcessedEvent>(rootElement, JsonFormatter.DeserializerOptions);
							break;
						case EventType.SpamReport:
							webHookEvent = JsonSerializer.Deserialize<SpamReportEvent>(rootElement, JsonFormatter.DeserializerOptions);
							break;
						case EventType.Unsubscribe:
							webHookEvent = JsonSerializer.Deserialize<UnsubscribeEvent>(rootElement, JsonFormatter.DeserializerOptions);
							break;
						default:
							throw new Exception($"{typeAsString} is an unknown event type");
					}

					var properties = doc.RootElement
						.EnumerateObject()
						.Where(property => !_knownProperties.Contains(property.Name));

					foreach (var property in properties)
					{
						webHookEvent.UniqueArguments.Add(property.Name, property.Value.GetString());
					}

					return webHookEvent;
				}

				throw new JsonException("Failed to extract 'event' property, it might be missing?");
			}

			throw new JsonException("Failed to parse JsonDocument");
		}
	}
}
