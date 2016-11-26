using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Model
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum CampaignStatus
	{
		[EnumMember(Value = "draft")]
		Draft,
		[EnumMember(Value = "scheduled")]
		Scheduled,
		[EnumMember(Value = "sent")]
		Sent
	}
}
