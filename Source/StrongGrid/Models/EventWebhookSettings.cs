using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Event Webhook settings
	/// </summary>
	public class EventWebhookSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="EventWebhookSettings"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		[JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the ASM Group Resubscribe notification is sent
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("group_resubscribe", NullValueHandling = NullValueHandling.Ignore)]
		public bool GroupResubscribe { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the ASM Group Unsubscribe notification is sent
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("group_unsubscribe", NullValueHandling = NullValueHandling.Ignore)]
		public bool GroupUnsubscribe { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Delivered notification is sent
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("delivered", NullValueHandling = NullValueHandling.Ignore)]
		public bool Delivered { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the SpamReport notification is sent
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("spam_report", NullValueHandling = NullValueHandling.Ignore)]
		public bool SpamReport { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Bounce notification is sent
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("bounce", NullValueHandling = NullValueHandling.Ignore)]
		public bool Bounce { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Deferred notification is sent
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("deferred", NullValueHandling = NullValueHandling.Ignore)]
		public bool Deferred { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Unsubscribe notification is sent
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("unsubscribe", NullValueHandling = NullValueHandling.Ignore)]
		public bool Unsubscribe { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Processed notification is sent
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("processed", NullValueHandling = NullValueHandling.Ignore)]
		public bool Processed { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Open notification is sent
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("open", NullValueHandling = NullValueHandling.Ignore)]
		public bool Open { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Click notification is sent
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("click", NullValueHandling = NullValueHandling.Ignore)]
		public bool Click { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Dropped notification is sent
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("dropped", NullValueHandling = NullValueHandling.Ignore)]
		public bool Dropped { get; set; }
	}
}
