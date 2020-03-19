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

		/// <summary>
		/// Gets or sets the user identifier.
		/// </summary>
		/// <value>
		/// The user identifier.
		/// </value>
		[JsonProperty("sg_user_id", NullValueHandling = NullValueHandling.Ignore)]
		public long? UserId { get; set; }

		/// <summary>
		/// Gets the unique arguments.
		/// </summary>
		/// <value>
		/// The unique arguments.
		/// </value>
		[JsonIgnore]
		public IDictionary<string, string> UniqueArguments { get; } = new Dictionary<string, string>();
	}
}
