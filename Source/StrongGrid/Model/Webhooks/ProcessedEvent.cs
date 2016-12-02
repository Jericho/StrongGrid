using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model.Webhooks
{
	public class ProcessedEvent : DeliveryEvent
	{
		[JsonProperty("asm_group_id", NullValueHandling = NullValueHandling.Ignore)]
		public int AsmGroupId { get; set; }

		[JsonProperty("newsletter", NullValueHandling = NullValueHandling.Ignore)]
		public Newsletter Newsletter { get; set; }

		[JsonProperty("send_at", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime ProcessedOn { get; set; }
	}
}
