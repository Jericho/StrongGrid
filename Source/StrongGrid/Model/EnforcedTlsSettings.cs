using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class EnforcedTlsSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether [require TLS].
		/// </summary>
		/// <value>
		///   <c>true</c> if [require TLS]; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("require_tls")]
		public bool RequireTls { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [require valid certificate].
		/// </summary>
		/// <value>
		/// <c>true</c> if [require valid certificate]; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("require_valid_cert")]
		public bool RequireValidCertificate { get; set; }
	}
}
