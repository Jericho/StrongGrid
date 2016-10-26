using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model
{
	public class SpamReport
	{
		[JsonProperty("email")]
		public string Email { get; set; }

		[JsonProperty("ip")]
		public string IpAddress { get; set; }

		[JsonProperty("created")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime CreatedOn { get; set; }
	}
}
