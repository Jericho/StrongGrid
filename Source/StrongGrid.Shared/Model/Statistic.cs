using Newtonsoft.Json;
using System;

namespace StrongGrid.Model
{
	public class Statistic
	{
		[JsonProperty("date")]
		public DateTime Date { get; set; }

		[JsonProperty("stats")]
		public Stat[] Stats { get; set; }
	}
}
