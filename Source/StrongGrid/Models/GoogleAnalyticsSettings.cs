using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Allows you to enable tracking provided by Google Analytics.
	/// </summary>
	public class GoogleAnalyticsSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GoogleAnalyticsSettings"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enable", NullValueHandling = NullValueHandling.Ignore)]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the substitution tag.
		/// </summary>
		/// <value>
		/// The substitution tag.
		/// </value>
		[JsonProperty("substitution_tag", NullValueHandling = NullValueHandling.Ignore)]
		public string SubstitutionTag { get; set; }
	}
}
