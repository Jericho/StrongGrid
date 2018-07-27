using Newtonsoft.Json;

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
		[JsonProperty("click_tracking", NullValueHandling = NullValueHandling.Ignore)]
		public ClickTrackingSettings ClickTracking { get; set; }

		/// <summary>
		/// Gets or sets the open tracking.
		/// </summary>
		/// <value>
		/// The open tracking.
		/// </value>
		[JsonProperty("open_tracking", NullValueHandling = NullValueHandling.Ignore)]
		public OpenTrackingSettings OpenTracking { get; set; }

		/// <summary>
		/// Gets or sets the subscription tracking.
		/// </summary>
		/// <value>
		/// The subscription tracking.
		/// </value>
		[JsonProperty("subscription_tracking", NullValueHandling = NullValueHandling.Ignore)]
		public SubscriptionTrackingSettings SubscriptionTracking { get; set; }

		/// <summary>
		/// Gets or sets the google analytics.
		/// </summary>
		/// <value>
		/// The google analytics.
		/// </value>
		[JsonProperty("ganalytics", NullValueHandling = NullValueHandling.Ignore)]
		public GoogleAnalyticsSettings GoogleAnalytics { get; set; }
	}
}
