using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// Enforce TLS settings
	/// </summary>
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
		/// Gets or sets a value indicating whether a valid certificate is required.
		/// </summary>
		/// <value>
		/// <c>true</c> if a valid certificate is required; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("require_valid_cert")]
		public bool RequireValidCertificate { get; set; }
	}
}
