using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model.Webhooks
{
	public class DroppedEvent : DeliveryEvent
	{
		[JsonProperty("reason", NullValueHandling = NullValueHandling.Ignore)]
		public string Reason { get; set; }
	}
}
