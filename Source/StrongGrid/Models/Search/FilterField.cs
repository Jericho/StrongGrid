using System.Runtime.Serialization;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Enumeration to indicate the filter field when searching for email activities
	/// </summary>
	public enum FilterField
	{
		/// <summary>
		/// The identifier of the message
		/// </summary>
		[EnumMember(Value = "msg_id")]
		MessageId,

		/// <summary>
		/// The email address of the sender
		/// </summary>
		[EnumMember(Value = "from_email	")]
		From,

		/// <summary>
		/// The subject of the message
		/// </summary>
		[EnumMember(Value = "subject")]
		Subject,

		/// <summary>
		/// The email address of the recipient
		/// </summary>
		[EnumMember(Value = "to_email")]
		To,

		/// <summary>
		/// The type of email activity
		/// </summary>
		[EnumMember(Value = "status")]
		ActivityType,

		/// <summary>
		/// The identifier of the template
		/// </summary>
		[EnumMember(Value = "template_id")]
		TemplateId,

		/// <summary>
		/// The name of the template
		/// </summary>
		[EnumMember(Value = "template_name")]
		TemplateName,

		/// <summary>
		/// The name of the campaign
		/// </summary>
		[EnumMember(Value = "campaign_name")]
		CampaignName,

		/// <summary>
		/// The identifier of the campaign
		/// </summary>
		[EnumMember(Value = "campaign_id")]
		CampaignId,

		/// <summary>
		/// The identifier of the Api Key
		/// </summary>
		[EnumMember(Value = "api_key_id")]
		ApiKeyId,

		/// <summary>
		/// The name of the Api Key
		/// </summary>
		[EnumMember(Value = "api_key_name")]
		ApiKeyName,

		/// <summary>
		/// Seems like a duplicate of 'status'.
		/// </summary>
		[EnumMember(Value = "events")]
		Events,

		/// <summary>
		/// IP address of the person who sent the message
		/// </summary>
		[EnumMember(Value = "originating_ip")]
		OriginatingIpAddress,

		/// <summary>
		/// Custom tags that you create
		/// </summary>
		[EnumMember(Value = "categories")]
		Categories,

		/// <summary>
		/// The SendGrid dedicated IP address used to send the email
		/// </summary>
		[EnumMember(Value = "outbound_ip")]
		OutboundIpAddress,

		/// <summary>
		/// Date and time of the last email activity
		/// </summary>
		[EnumMember(Value = "last_event_time")]
		LastEventTime,

		/// <summary>
		/// Number of clicks
		/// </summary>
		[EnumMember(Value = "clicks")]
		Clicks,

		/// <summary>
		/// The name of the unsubscribe group
		/// </summary>
		[EnumMember(Value = "unsubscribe_group_name")]
		UnsubscribeGroupName,

		/// <summary>
		/// The identified of the unsubscribe group
		/// </summary>
		[EnumMember(Value = "unsubscribe_group_id")]
		UnsubscribeGroupId,

		/// <summary>
		/// The teamates username
		/// </summary>
		[EnumMember(Value = "teammate")]
		Teammate
	}
}
