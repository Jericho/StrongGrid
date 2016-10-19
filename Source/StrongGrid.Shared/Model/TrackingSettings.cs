using Newtonsoft.Json;

namespace StrongGrid.Model
{
    public class TrackingSettings
    {
        [JsonProperty("click_tracking")]
        public ClickTrackingSettings ClickTracking { get; set; }

        [JsonProperty("open_tracking")]
        public OpenTrackingSettings OpenTracking { get; set; }

        [JsonProperty("subscription_tracking")]
        public SubscriptionTrackingSettings SubscriptionTracking { get; set; }

        [JsonProperty("ganalytics")]
        public GoogleAnalyticsSettings GoogleAnalytics { get; set; }
    }
}
