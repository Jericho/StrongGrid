using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class WhitelabelLink
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

		[JsonProperty("default")]
		public bool IsDefault { get; set; }

		[JsonProperty("valid")]
		public bool IsValid { get; set; }

		[JsonProperty("legacy")]
		public bool IsLegacy { get; set; }

		[JsonProperty("dns")]
		public WhitelabelLinkDns DNS { get; set; }
	}
}
