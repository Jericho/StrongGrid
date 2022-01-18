using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Google Analytics global settings.
	/// </summary>
	public class GoogleAnalyticsGlobalSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GoogleAnalyticsGlobalSettings" /> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("enabled")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the utm source.
		/// </summary>
		/// <value>
		/// The utm source.
		/// </value>
		[JsonPropertyName("utm_source")]
		public string UtmSource { get; set; }

		/// <summary>
		/// Gets or sets the utm medium.
		/// </summary>
		/// <value>
		/// The utm medium.
		/// </value>
		[JsonPropertyName("utm_medium")]
		public string UtmMedium { get; set; }

		/// <summary>
		/// Gets or sets the utm term.
		/// </summary>
		/// <value>
		/// The utm term.
		/// </value>
		[JsonPropertyName("utm_term")]
		public string UtmTerm { get; set; }

		/// <summary>
		/// Gets or sets the content of the utm.
		/// </summary>
		/// <value>
		/// The content of the utm.
		/// </value>
		[JsonPropertyName("utm_content")]
		public string UtmContent { get; set; }

		/// <summary>
		/// Gets or sets the utm campaign.
		/// </summary>
		/// <value>
		/// The utm campaign.
		/// </value>
		[JsonPropertyName("utm_campaign")]
		public string UtmCampaign { get; set; }
	}
}
