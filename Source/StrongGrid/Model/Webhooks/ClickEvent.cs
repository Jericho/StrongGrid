using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model.Webhooks
{
	public class ClickEvent : EngagementEvent
	{
		[JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
		public string Url { get; set; }

		[JsonProperty("url_offset", NullValueHandling = NullValueHandling.Ignore)]
		public UrlOffset UrlOffset { get; set; }

		[JsonProperty("asm_group_id", NullValueHandling = NullValueHandling.Ignore)]
		public int AsmGroupId { get; set; }

		[JsonProperty("newsletter", NullValueHandling = NullValueHandling.Ignore)]
		public Newsletter Newsletter { get; set; }
	}
}
