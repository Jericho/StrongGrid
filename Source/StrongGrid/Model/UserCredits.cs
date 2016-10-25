using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model
{
	public class UserCredits
	{
		[JsonProperty("remain")]
		public long Remaining { get; set; }

		[JsonProperty("total")]
		public long Total { get; set; }

		[JsonProperty("overage")]
		public long Overage { get; set; }

		[JsonProperty("used")]
		public long Used { get; set; }

		[JsonProperty("last_reset")]
		public DateTime LastReset { get; set; }

		[JsonProperty("next_reset")]
		public DateTime NextReset { get; set; }

		[JsonProperty("reset_frequency")]
		public string ResetFrequency { get; set; }
	}
}
