using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Model
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum AlertType
	{
		/// <summary>
		/// The usage limit
		/// </summary>
		[EnumMember(Value = "usage_limit")]
		UsageLimit,

		/// <summary>
		/// The stats notification
		/// </summary>
		[EnumMember(Value = "stats_notification")]
		StatsNotification,
	}
}
