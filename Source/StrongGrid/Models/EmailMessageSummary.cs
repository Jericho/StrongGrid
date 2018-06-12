using Newtonsoft.Json;
using StrongGrid.Models.EmailActivities;
using StrongGrid.Utilities;
using System.Collections.Generic;

namespace StrongGrid.Models
{
	/// <summary>
	/// The summary about a specific message.
	/// </summary>
	public class EmailMessageSummary
	{
		/// <summary>
		/// Gets or sets the 'from' address.
		/// </summary>
		/// <value>
		/// The 'from' address.
		/// </value>
		[JsonProperty("from_email", NullValueHandling = NullValueHandling.Ignore)]
		public string From { get; set; }

		/// <summary>
		/// Gets or sets the message id.
		/// </summary>
		/// <value>
		/// The message id.
		/// </value>
		[JsonProperty("msg_id", NullValueHandling = NullValueHandling.Ignore)]
		public string MessageId { get; set; }

		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		/// <value>
		/// The subject.
		/// </value>
		[JsonProperty("subject", NullValueHandling = NullValueHandling.Ignore)]
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the 'to' address.
		/// </summary>
		/// <value>
		/// The 'to' address.
		/// </value>
		[JsonProperty("to_email", NullValueHandling = NullValueHandling.Ignore)]
		public string To { get; set; }

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		[JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
		public EmailActivityStatus Status { get; set; }

		/// <summary>
		/// Gets or sets the template Id.
		/// </summary>
		/// <value>
		/// The template identifier.
		/// </value>
		[JsonProperty("template_id", NullValueHandling = NullValueHandling.Ignore)]
		public string TemplateId { get; set; }

		/// <summary>
		/// Gets or sets the group Id.
		/// </summary>
		/// <value>
		/// The group identifier.
		/// </value>
		[JsonProperty("asm_group_id", NullValueHandling = NullValueHandling.Ignore)]
		public string AsmGroupId { get; set; }

		/// <summary>
		/// Gets or sets the teammate.
		/// </summary>
		/// <value>
		/// The teammate.
		/// </value>
		[JsonProperty("teammate", NullValueHandling = NullValueHandling.Ignore)]
		public string Teammate { get; set; }

		/// <summary>
		/// Gets or sets the API key id.
		/// </summary>
		/// <value>
		/// The API key identifier.
		/// </value>
		[JsonProperty("api_key_id", NullValueHandling = NullValueHandling.Ignore)]
		public string ApiKeyId { get; set; }

		/// <summary>
		/// Gets or sets the IP of the user who sent the message.
		/// </summary>
		/// <value>
		/// The IP address.
		/// </value>
		[JsonProperty("originating_ip", NullValueHandling = NullValueHandling.Ignore)]
		public string OriginatingIpAddress { get; set; }

		/// <summary>
		/// Gets or sets the IP used to send to the remote MTA.
		/// </summary>
		/// <value>
		/// The IP address.
		/// </value>
		[JsonProperty("outbound_ip", NullValueHandling = NullValueHandling.Ignore)]
		public string OutboundIpAddress { get; set; }

		/// <summary>
		/// Gets or sets whether or not the outbound IP is dedicated vs shared.
		/// </summary>
		/// <value>
		/// The type of IP address.
		/// </value>
		[JsonProperty("outbound_ip_type", NullValueHandling = NullValueHandling.Ignore)]
		public IpAddressType OutboundIpAddressType { get; set; }

		/// <summary>
		/// Gets or sets the marketing campaign id.
		/// </summary>
		/// <value>
		/// The marketing campaign identifier.
		/// </value>
		[JsonProperty("marketing_campaign_id", NullValueHandling = NullValueHandling.Ignore)]
		public string MarketingCampaignId { get; set; }

		/// <summary>
		/// Gets or sets the name of the marketing campaign.
		/// </summary>
		/// <value>
		/// The name of the marketing campaign.
		/// </value>
		[JsonProperty("marketing_campaign_name", NullValueHandling = NullValueHandling.Ignore)]
		public string MarketingCampaignName { get; set; }

		/// <summary>
		/// Gets or sets the marketing cmapaign split id.
		/// </summary>
		/// <value>
		/// The marketing campaign split identifier.
		/// </value>
		[JsonProperty("marketing_campaign_split_id", NullValueHandling = NullValueHandling.Ignore)]
		public string MarketingCampaignSplitId { get; set; }

		/// <summary>
		/// Gets or sets the version of the marketing campaign.
		/// </summary>
		/// <value>
		/// The marketing campaign version.
		/// </value>
		[JsonProperty("marketing_campaign_version", NullValueHandling = NullValueHandling.Ignore)]
		public string MarketingCampaignVersion { get; set; }

		/// <summary>
		/// Gets or sets the categories associated to the message.
		/// </summary>
		/// <value>
		/// The categories.
		/// </value>
		[JsonProperty("categories", NullValueHandling = NullValueHandling.Ignore)]
		public string[] Categories { get; set; }

		/// <summary>
		/// Gets or sets the events.
		/// </summary>
		/// <value>
		/// The events.
		/// </value>
		[JsonProperty("events", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EmailActivityEventConverter))]
		public Event[] Events { get; set; }

		/// <summary>
		/// Gets or sets the custom arguments.
		/// </summary>
		/// <value>
		/// The custom arguments.
		/// </value>
		[JsonProperty("unique_args", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(KeyValuePairEnumerationConverter))]
		public KeyValuePair<string, string>[] CustomArguments { get; set; }
	}
}
