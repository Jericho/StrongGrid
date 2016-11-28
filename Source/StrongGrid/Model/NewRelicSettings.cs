using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class NewRelicSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="NewRelicSettings" /> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enabled")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the license key.
		/// </summary>
		/// <value>
		/// The license key.
		/// </value>
		[JsonProperty("license_key")]
		public string LicenseKey { get; set; }
	}
}
