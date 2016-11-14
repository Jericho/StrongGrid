using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class EnforcedTlsSettings
	{
		[JsonProperty("require_tls")]
		public bool RequireTls { get; set; }

		[JsonProperty("require_valid_cert")]
		public bool RequireValidCertificate { get; set; }
	}
}
