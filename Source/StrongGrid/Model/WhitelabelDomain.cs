using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class WhitelabelDomain
	{
		[JsonProperty("id")]
		public long Id { get; set; }

		[JsonProperty("domain")]
		public string Domain { get; set; }

		[JsonProperty("subdomain")]
		public string Subdomain { get; set; }

		[JsonProperty("username")]
		public string Username { get; set; }

		[JsonProperty("user_id")]
		public long UserId { get; set; }

		[JsonProperty("ips")]
		public string[] IpAddresses { get; set; }

		[JsonProperty("custom_spf")]
		public bool IsCustomSpf { get; set; }

		[JsonProperty("default")]
		public bool IsDefault { get; set; }

		[JsonProperty("legacy")]
		public bool IsLegacy { get; set; }

		[JsonProperty("automatic_security")]
		public bool IsAutomaticSecurity { get; set; }

		[JsonProperty("valid")]
		public bool IsValid { get; set; }

		[JsonProperty("dns")]
		public WhitelabelDomainDns DNS { get; set; }
	}
}
