using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class WhitelabelIp
	{
		[JsonProperty("id")]
		public long Id { get; set; }

		[JsonProperty("ip")]
		public string IpAddress { get; set; }

		[JsonProperty("rdns")]
		public string RDNS { get; set; }

		[JsonProperty("subdomain")]
		public string Subdomain { get; set; }

		[JsonProperty("domain")]
		public string Domain { get; set; }

		[JsonProperty("valid")]
		public bool IsValid { get; set; }

		[JsonProperty("legacy")]
		public bool IsLegacy { get; set; }

		[JsonProperty("a_record")]
		public DnsRecord ARecord { get; set; }
	}
}
