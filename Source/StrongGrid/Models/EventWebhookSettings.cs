using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Event Webhook settings.
	/// </summary>
	public class EventWebhookSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="EventWebhookSettings"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("enabled")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		[JsonPropertyName("url")]
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the ASM Group Resubscribe notification is sent.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("group_resubscribe")]
		public bool GroupResubscribe { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the ASM Group Unsubscribe notification is sent.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("group_unsubscribe")]
		public bool GroupUnsubscribe { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Delivered notification is sent.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("delivered")]
		public bool Delivered { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the SpamReport notification is sent.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("spam_report")]
		public bool SpamReport { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Bounce notification is sent.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("bounce")]
		public bool Bounce { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Deferred notification is sent.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("deferred")]
		public bool Deferred { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Unsubscribe notification is sent.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("unsubscribe")]
		public bool Unsubscribe { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Processed notification is sent.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("processed")]
		public bool Processed { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Open notification is sent.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("open")]
		public bool Open { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Click notification is sent.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("click")]
		public bool Click { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Dropped notification is sent.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("dropped")]
		public bool Dropped { get; set; }
	}
}
