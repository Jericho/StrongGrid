using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model.Webhooks
{
	public class DeferredEvent : DeliveryEvent
	{
		[JsonProperty("response", NullValueHandling = NullValueHandling.Ignore)]
		public string Response { get; set; }

		[JsonProperty("attempt", NullValueHandling = NullValueHandling.Ignore)]
		public int Attempt { get; set; }

		[JsonProperty("asm_group_id", NullValueHandling = NullValueHandling.Ignore)]
		public int AsmGroupId { get; set; }

		[JsonProperty("newsletter", NullValueHandling = NullValueHandling.Ignore)]
		public Newsletter Newsletter { get; set; }
	}
}
