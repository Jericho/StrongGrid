using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model.Webhooks
{
	public class SpamReportEvent : EngagementEvent
	{
		[JsonProperty("asm_group_id", NullValueHandling = NullValueHandling.Ignore)]
		public int AsmGroupId { get; set; }
	}
}
