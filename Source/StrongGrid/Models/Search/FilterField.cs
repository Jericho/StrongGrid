using System.Runtime.Serialization;

namespace StrongGrid.Models.Search
{
	public enum FilterField
	{
		[EnumMember(Value = "msg_id")]
		MessageId,

		[EnumMember(Value = "from_email	")]
		From,

		[EnumMember(Value = "subject")]
		Subject,

		[EnumMember(Value = "to_email")]
		To,

		[EnumMember(Value = "status")]
		Status,

		[EnumMember(Value = "template_id")]
		TemplateId,

		[EnumMember(Value = "template_name")]
		TemplateName,

		[EnumMember(Value = "campaign_name")]
		CampaignName,

		[EnumMember(Value = "campaign_id")]
		CampaignId,

		[EnumMember(Value = "api_key_id")]
		ApiKeyId,

		[EnumMember(Value = "api_key_name")]
		ApiKeyName,

		[EnumMember(Value = "events")]
		Events,

		[EnumMember(Value = "originating_ip")]
		OriginatingIpAddress,

		[EnumMember(Value = "categories")]
		Categories,

		[EnumMember(Value = "unique_args")]
		CustomArguments,

		[EnumMember(Value = "outbound_ip")]
		OutboundIpAddress,

		[EnumMember(Value = "last_event_time")]
		LastEventTime,

		[EnumMember(Value = "clicks")]
		Clicks,

		[EnumMember(Value = "unsubscribe_group_name")]
		UnsubscribeGroupName,

		[EnumMember(Value = "unsubscribe_group_id")]
		UnsubscribeGroupId,

		[EnumMember(Value = "teammate")]
		Teammate
	}
}
