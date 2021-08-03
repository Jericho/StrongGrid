using StrongGrid.Utilities;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the type of alert.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<AlertType>))]
	public enum AlertType
	{
		/// <summary>
		/// The usage limit.
		/// </summary>
		[EnumMember(Value = "usage_limit")]
		UsageLimit,

		/// <summary>
		/// The stats notification.
		/// </summary>
		[EnumMember(Value = "stats_notification")]
		StatsNotification,
	}
}
