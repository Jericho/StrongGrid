using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// A webhook event.
	/// </summary>
	public abstract class Event
	{
		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonProperty("event", NullValueHandling = NullValueHandling.Ignore)]
		public EventType EventType { get; set; }

		/// <summary>
		/// Gets or sets the email address of the intended recipient.
		/// </summary>
		/// <value>
		/// The email address.
		/// </value>
		[JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the timestamp.
		/// </summary>
		/// <value>
		/// The timestamp.
		/// </value>
		[JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime Timestamp { get; set; }

		/// <summary>
		/// Gets or sets the internal event identifier.
		/// </summary>
		/// <remarks>
		/// You can use this unique Id for deduplication purposes. This Id is up to 100 characters long and is URL safe.
		/// </remarks>
		/// <value>
		/// The internal event identifier.
		/// </value>
		[JsonProperty("sg_event_id", NullValueHandling = NullValueHandling.Ignore)]
		public string InternalEventId { get; set; }

		/// <summary>
		/// Gets or sets the internal message identifier.
		/// </summary>
		/// <remarks>
		/// This value in this property is useful to SendGrid support.
		/// It contains the message Id and information about where the mail was processed concatenated together.
		/// Having this data available is helpful for troubleshooting purposes.
		/// Developers should use the 'MessageId' property to get the message's unique identifier.
		/// </remarks>
		/// <value>
		/// The internal message identifier.
		/// </value>
		[JsonProperty("sg_message_id", NullValueHandling = NullValueHandling.Ignore)]
		public string InternalMessageId { get; set; }

		/// <summary>
		/// Gets the message identifier.
		/// </summary>
		/// <value>
		/// The message identifier.
		/// </value>
		public string MessageId
		{
			get
			{
				if (InternalMessageId == null) return null;
				var filterIndex = InternalMessageId.IndexOf(".filter", StringComparison.OrdinalIgnoreCase);
				if (filterIndex <= 0) return InternalMessageId;
				return InternalMessageId.Substring(0, filterIndex);
			}
		}

		/// <summary>
		/// Gets the unique arguments.
		/// </summary>
		/// <value>
		/// The unique arguments.
		/// </value>
		[JsonIgnore]
		public IDictionary<string, string> UniqueArguments { get; } = new Dictionary<string, string>();

		/// <summary>
		/// Gets or sets the marketing campaign identifier.
		/// </summary>
		/// <value>
		/// The marketing campaign identifier.
		/// </value>
		[JsonProperty("marketing_campaign_id", NullValueHandling = NullValueHandling.Ignore)]
		public long? MarketingCampaignId { get; set; }

		/// <summary>
		/// Gets or sets the name of the marketing campaign.
		/// </summary>
		/// <value>
		/// The name of the marketing campaign.
		/// </value>
		[JsonProperty("marketing_campaign_name", NullValueHandling = NullValueHandling.Ignore)]
		public string MarketingCampaignName { get; set; }

		/// <summary>
		/// Gets or sets the user identifier.
		/// </summary>
		/// <value>
		/// The user identifier.
		/// </value>
		[JsonProperty("sg_user_id", NullValueHandling = NullValueHandling.Ignore)]
		public long? UserId { get; set; }

		/// <summary>
		/// Gets or sets the newsletter.
		/// </summary>
		/// <value>
		/// The newsletter.
		/// </value>
		[JsonProperty("newsletter", NullValueHandling = NullValueHandling.Ignore)]
		public Newsletter Newsletter { get; set; }

		/// <summary>
		/// Gets or sets the marketing campaign version.
		/// </summary>
		/// <value>
		/// The marketing campaign version.
		/// </value>
		[JsonProperty("marketing_campaign_version", NullValueHandling = NullValueHandling.Ignore)]
		public string MarketingCampaignVersion { get; set; }

		/// <summary>
		/// Gets or sets the marketing campaign split identifier.
		/// </summary>
		/// <value>
		/// The marketing campaign split identifier.
		/// </value>
		[JsonProperty("marketing_campaign_split_id", NullValueHandling = NullValueHandling.Ignore)]
		public long? MarketingCampaignSplitId { get; set; }
	}
}
