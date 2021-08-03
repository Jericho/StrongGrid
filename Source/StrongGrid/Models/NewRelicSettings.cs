using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// New Relic settings.
	/// </summary>
	public class NewRelicSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="NewRelicSettings" /> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("enabled")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the license key.
		/// </summary>
		/// <value>
		/// The license key.
		/// </value>
		[JsonPropertyName("license_key")]
		public string LicenseKey { get; set; }
	}
}
