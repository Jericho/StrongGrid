using System;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Event Webhook settings.
	/// </summary>
	public class EventWebhookSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether the webhook is configured to send bounce events.
		/// </summary>
		/// <remarks>
		/// A bounce occurs when a receiving server could not or would not accept a message.
		/// </remarks>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("bounce")]
		public bool Bounce { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the webhook is configured to send click events.
		/// </summary>
		/// <remarks>
		/// Click events occur when a recipient clicks on a link within the message. You must enable Click Tracking to receive this type of event.
		/// </remarks>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("click")]
		public bool Click { get; set; }

		/// <summary>
		/// Gets or sets the date this webhooks settings was created.
		/// </summary>
		/// <value>
		/// The created on.
		/// </value>
		[JsonPropertyName("created_date")]
		public DateTime? CreatedOn { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the the webhook is configured to send deferred events.
		/// </summary>
		/// <remarks>
		/// Deferred events occur when a recipient's email server temporarily rejects a message.
		/// </remarks>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("deferred")]
		public bool Deferred { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Delivered notification is enabled.
		/// </summary>
		/// <remarks>
		/// Delivered events occur when a message has been successfully delivered to the receiving server.
		/// </remarks>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("delivered")]
		public bool Delivered { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the webhook is configured to send dropped events.
		/// </summary>
		/// <remarks>
		/// Dropped events occur when your message is not delivered by Twilio SendGrid.
		/// Dropped events are accomponied by a reason property, which indicates why the message was dropped.
		/// Reasons for a dropped message include:
		///   - Invalid SMTPAPI header, Spam Content (if spam checker app enabled)
		///   - Unsubscribed Address
		///   - Bounced Address
		///   - Spam Reporting Address
		///   - Invalid
		///   - Recipient List over Package Quota.
		/// </remarks>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("dropped")]
		public bool Dropped { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="EventWebhookSettings"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("enabled")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets an optional friendly name assigned to the Event Webhook to help you differentiate it.
		/// </summary>
		/// <remarks>
		/// The friendly name is for convenience only.
		/// You should use the webhook id property for any programmatic tasks.
		/// </remarks>
		[JsonPropertyName("friendly_name")]
		public string FriendlyName { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the webhook is configured to send group resubscribe events.
		/// </summary>
		/// <remarks>
		/// Group resubscribes occur when recipients resubscribe to a specific unsubscribe group by updating their subscription preferences.
		/// You must enable Subscription Tracking to receive this type of event.
		/// </remarks>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("group_resubscribe")]
		public bool GroupResubscribe { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the webhook is configured to send group unsubscribe events.
		/// </summary>
		/// <remarks>
		/// Group unsubscribes occur when recipients unsubscribe from a specific unsubscribe group either by direct link or by updating their subscription preferences.
		/// You must enable Subscription Tracking to receive this type of event.
		/// </remarks>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("group_unsubscribe")]
		public bool GroupUnsubscribe { get; set; }

		/// <summary>
		/// Gets or sets a unique string used to identify the webhook.
		/// </summary>
		/// <remarks>
		/// A webhook's ID is generated programmatically and cannot be changed after creation.
		/// You can assign a natural language identifier to your webhook using the <see cref="FriendlyName"/> property.
		/// </remarks>
		[JsonPropertyName("id")]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the OAuth client ID SendGrid sends to your OAuth server or service provider to generate an OAuth access token.
		/// </summary>
		[JsonPropertyName("oauth_client_id")]
		public string OauthClientId { get; set; }

		/// <summary>
		/// Gets or sets the ThURL where SendGrid sends the OAuth client ID and client secret to generate an access token.
		/// </summary>
		/// <remarks>
		/// This should be your OAuth server or service provider.
		/// </remarks>
		[JsonPropertyName("oauth_token_url")]
		public string OauthTokenUrl { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the webhook is configured to send open events.
		/// </summary>
		/// <remarks>
		/// Open events occur when a recipient has opened the HTML message.You must enable Open Tracking to receive this type of event.
		/// </remarks>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("open")]
		public bool Open { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the webhook is configured to send processed events.
		/// </summary>
		/// <remarks>
		/// Processed events occur when a message has been received by Twilio SendGrid and is ready to be delivered.
		/// </remarks>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("processed")]
		public bool Processed { get; set; }

		/// <summary>
		/// Gets or sets the public key which can be used to verify the SendGrid signature.
		/// </summary>
		[JsonPropertyName("public_key")]
		public string PublicKey { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the webhook is configured to send spam report events.
		/// </summary>
		/// <remarks>
		/// Spam reports occur when recipients mark a message as spam.
		/// </remarks>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("spam_report")]
		public bool SpamReport { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the webhook is configured to send unsubscribe events.
		/// </summary>
		/// <remarks>
		/// Unsubscribes occur when recipients click on a message's subscription management link.
		/// You must enable Subscription Tracking to receive this type of event.
		/// </remarks>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("unsubscribe")]
		public bool Unsubscribe { get; set; }

		/// <summary>
		/// Gets or sets the date this webhooks settings was updated.
		/// </summary>
		/// <value>
		/// The created on.
		/// </value>
		[JsonPropertyName("updated_date")]
		public DateTime? UpdatedOn { get; set; }

		/// <summary>
		/// Gets or sets the URL where SendGrid will send event data.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		[JsonPropertyName("url")]
		public string Url { get; set; }
	}
}
