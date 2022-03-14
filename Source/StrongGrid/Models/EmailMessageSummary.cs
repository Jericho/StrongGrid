using StrongGrid.Json;
using StrongGrid.Models.EmailActivities;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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
		[JsonPropertyName("from_email")]
		public string From { get; set; }

		/// <summary>
		/// Gets or sets the message id.
		/// </summary>
		/// <value>
		/// The message id.
		/// </value>
		[JsonPropertyName("msg_id")]
		public string MessageId { get; set; }

		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		/// <value>
		/// The subject.
		/// </value>
		[JsonPropertyName("subject")]
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the 'to' address.
		/// </summary>
		/// <value>
		/// The 'to' address.
		/// </value>
		[JsonPropertyName("to_email")]
		public string To { get; set; }

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		[JsonPropertyName("status")]
		public EmailActivityStatus Status { get; set; }

		/// <summary>
		/// Gets or sets the template Id.
		/// </summary>
		/// <value>
		/// The template identifier.
		/// </value>
		[JsonPropertyName("template_id")]
		public string TemplateId { get; set; }

		/// <summary>
		/// Gets or sets the group Id.
		/// </summary>
		/// <value>
		/// The group identifier.
		/// </value>
		[JsonPropertyName("asm_group_id")]
		public string AsmGroupId { get; set; }

		/// <summary>
		/// Gets or sets the teammate.
		/// </summary>
		/// <value>
		/// The teammate.
		/// </value>
		[JsonPropertyName("teammate")]
		public string Teammate { get; set; }

		/// <summary>
		/// Gets or sets the API key id.
		/// </summary>
		/// <value>
		/// The API key identifier.
		/// </value>
		[JsonPropertyName("api_key_id")]
		public string ApiKeyId { get; set; }

		/// <summary>
		/// Gets or sets the IP used to send to the remote MTA.
		/// </summary>
		/// <value>
		/// The IP address.
		/// </value>
		[JsonPropertyName("outbound_ip")]
		public string OutboundIpAddress { get; set; }

		/// <summary>
		/// Gets or sets whether or not the outbound IP is dedicated vs shared.
		/// </summary>
		/// <value>
		/// The type of IP address.
		/// </value>
		[JsonPropertyName("outbound_ip_type")]
		public IpAddressType OutboundIpAddressType { get; set; }

		/// <summary>
		/// Gets or sets the marketing campaign id.
		/// </summary>
		/// <value>
		/// The marketing campaign identifier.
		/// </value>
		[JsonPropertyName("marketing_campaign_id")]
		public string MarketingCampaignId { get; set; }

		/// <summary>
		/// Gets or sets the name of the marketing campaign.
		/// </summary>
		/// <value>
		/// The name of the marketing campaign.
		/// </value>
		[JsonPropertyName("marketing_campaign_name")]
		public string MarketingCampaignName { get; set; }

		/// <summary>
		/// Gets or sets the marketing cmapaign split id.
		/// </summary>
		/// <value>
		/// The marketing campaign split identifier.
		/// </value>
		[JsonPropertyName("marketing_campaign_split_id")]
		public string MarketingCampaignSplitId { get; set; }

		/// <summary>
		/// Gets or sets the version of the marketing campaign.
		/// </summary>
		/// <value>
		/// The marketing campaign version.
		/// </value>
		[JsonPropertyName("marketing_campaign_version")]
		public string MarketingCampaignVersion { get; set; }

		/// <summary>
		/// Gets or sets the categories associated to the message.
		/// </summary>
		/// <value>
		/// The categories.
		/// </value>
		[JsonPropertyName("categories")]
		public string[] Categories { get; set; }

		/// <summary>
		/// Gets or sets the events.
		/// </summary>
		/// <value>
		/// The events.
		/// </value>
		[JsonPropertyName("events")]
		[JsonConverter(typeof(EmailActivityEventConverter))]
		public Event[] Events { get; set; }

		/// <summary>
		/// Gets or sets the custom arguments.
		/// </summary>
		/// <value>
		/// The custom arguments.
		/// </value>
		[JsonPropertyName("unique_args")]
		[JsonConverter(typeof(KeyValuePairEnumerationConverter))]
		public KeyValuePair<string, string>[] CustomArguments { get; set; }
	}
}
