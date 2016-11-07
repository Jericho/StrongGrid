using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class DnsRecord
	{
		[JsonProperty("host")]
		public string Host { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("data")]
		public string Data { get; set; }

		[JsonProperty("valid")]
		public bool IsValid { get; set; }
	}
}
