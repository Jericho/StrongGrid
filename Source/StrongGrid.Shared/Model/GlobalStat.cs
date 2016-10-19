using Newtonsoft.Json;
using System;

namespace StrongGrid.Model
{
	public class GlobalStat
	{
		[JsonProperty("date")]
		public DateTime Date { get; set; }

		[JsonProperty("stats")]
		public Stat[] Stats { get; set; }
	}
}
