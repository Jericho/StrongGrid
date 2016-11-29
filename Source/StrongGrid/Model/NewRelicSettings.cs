using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// New Relic settings
	/// </summary>
	public class NewRelicSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="NewRelicSettings" /> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the license key.
		/// </summary>
		/// <value>
		/// The license key.
		/// </value>
		[JsonProperty("license_key", NullValueHandling = NullValueHandling.Ignore)]
		public string LicenseKey { get; set; }
	}
}
