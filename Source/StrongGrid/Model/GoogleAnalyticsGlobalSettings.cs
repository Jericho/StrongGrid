using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// Google Analytics global settings
	/// </summary>
	public class GoogleAnalyticsGlobalSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GoogleAnalyticsGlobalSettings" /> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the utm source.
		/// </summary>
		/// <value>
		/// The utm source.
		/// </value>
		[JsonProperty("utm_source", NullValueHandling = NullValueHandling.Ignore)]
		public string UtmSource { get; set; }

		/// <summary>
		/// Gets or sets the utm medium.
		/// </summary>
		/// <value>
		/// The utm medium.
		/// </value>
		[JsonProperty("utm_medium", NullValueHandling = NullValueHandling.Ignore)]
		public string UtmMedium { get; set; }

		/// <summary>
		/// Gets or sets the utm term.
		/// </summary>
		/// <value>
		/// The utm term.
		/// </value>
		[JsonProperty("utm_term", NullValueHandling = NullValueHandling.Ignore)]
		public string UtmTerm { get; set; }

		/// <summary>
		/// Gets or sets the content of the utm.
		/// </summary>
		/// <value>
		/// The content of the utm.
		/// </value>
		[JsonProperty("utm_content", NullValueHandling = NullValueHandling.Ignore)]
		public string UtmContent { get; set; }

		/// <summary>
		/// Gets or sets the utm campaign.
		/// </summary>
		/// <value>
		/// The utm campaign.
		/// </value>
		[JsonProperty("utm_campaign", NullValueHandling = NullValueHandling.Ignore)]
		public string UtmCampaign { get; set; }
	}
}
