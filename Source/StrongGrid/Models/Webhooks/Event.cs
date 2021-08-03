using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// A webhook event.
	/// </summary>
	[JsonConverter(typeof(EventConverter))]
	public abstract class Event
	{
		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonPropertyName("event")]
		[JsonConverter(typeof(StringEnumConverter<EventType>))]
		public EventType EventType { get; set; }

		/// <summary>
		/// Gets or sets the email address of the intended recipient.
		/// </summary>
		/// <value>
		/// The email address.
		/// </value>
		[JsonPropertyName("email")]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the timestamp.
		/// </summary>
		/// <value>
		/// The timestamp.
		/// </value>
		[JsonPropertyName("timestamp")]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime Timestamp { get; set; }

		/// <summary>
		/// Gets or sets the internal event identifier.
		/// </summary>
		/// <remarks>
		/// You can use this unique Id for deduplication purposes.
		/// This Id is up to 100 characters long and is URL safe.
		/// </remarks>
		/// <value>
		/// The internal event identifier.
		/// </value>
		[JsonPropertyName("sg_event_id")]
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
		[JsonPropertyName("sg_message_id")]
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
		[JsonPropertyName("marketing_campaign_id")]
		public long? MarketingCampaignId { get; set; }

		/// <summary>
		/// Gets or sets the name of the marketing campaign.
		/// </summary>
		/// <value>
		/// The name of the marketing campaign.
		/// </value>
		[JsonPropertyName("marketing_campaign_name")]
		public string MarketingCampaignName { get; set; }

		/// <summary>
		/// Gets or sets the user identifier.
		/// </summary>
		/// <value>
		/// The user identifier.
		/// </value>
		[JsonPropertyName("sg_user_id")]
		public long? UserId { get; set; }

		/// <summary>
		/// Gets or sets the newsletter.
		/// </summary>
		/// <value>
		/// The newsletter.
		/// </value>
		[JsonPropertyName("newsletter")]
		public Newsletter Newsletter { get; set; }

		/// <summary>
		/// Gets or sets the marketing campaign version.
		/// </summary>
		/// <value>
		/// The marketing campaign version.
		/// </value>
		[JsonPropertyName("marketing_campaign_version")]
		public string MarketingCampaignVersion { get; set; }

		/// <summary>
		/// Gets or sets the marketing campaign split identifier.
		/// </summary>
		/// <value>
		/// The marketing campaign split identifier.
		/// </value>
		[JsonPropertyName("marketing_campaign_split_id")]
		public long? MarketingCampaignSplitId { get; set; }
	}
}
