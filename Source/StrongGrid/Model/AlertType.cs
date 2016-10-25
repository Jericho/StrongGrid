using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrongGrid.Model
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum AlertType
	{
//		[Description("usage_limit")]
		[EnumMember(Value = "usage_limit")]
		UsageLimit,
//		[Description("stats_notification")]
		[EnumMember(Value = "stats_notification")]
		StatsNotification,
	}
}
