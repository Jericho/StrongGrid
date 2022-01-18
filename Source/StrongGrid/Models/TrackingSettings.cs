using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Tracking settings.
	/// </summary>
	public class TrackingSettings
	{
		/// <summary>
		/// Gets or sets the click tracking.
		/// </summary>
		/// <value>
		/// The click tracking.
		/// </value>
		[JsonPropertyName("click_tracking")]
		public ClickTrackingSettings ClickTracking { get; set; }

		/// <summary>
		/// Gets or sets the open tracking.
		/// </summary>
		/// <value>
		/// The open tracking.
		/// </value>
		[JsonPropertyName("open_tracking")]
		public OpenTrackingSettings OpenTracking { get; set; }

		/// <summary>
		/// Gets or sets the subscription tracking.
		/// </summary>
		/// <value>
		/// The subscription tracking.
		/// </value>
		[JsonPropertyName("subscription_tracking")]
		public SubscriptionTrackingSettings SubscriptionTracking { get; set; }

		/// <summary>
		/// Gets or sets the google analytics.
		/// </summary>
		/// <value>
		/// The google analytics.
		/// </value>
		[JsonPropertyName("ganalytics")]
		public GoogleAnalyticsSettings GoogleAnalytics { get; set; }
	}
}
