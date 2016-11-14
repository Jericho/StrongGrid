using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Model
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum AlertType
	{
		[EnumMember(Value = "usage_limit")]
		UsageLimit,
		[EnumMember(Value = "stats_notification")]
		StatsNotification,
	}
}
