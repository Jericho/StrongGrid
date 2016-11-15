using System.ComponentModel;

namespace StrongGrid.Model
{
	public enum AlertType
	{
		[Description("usage_limit")]
		UsageLimit,
		[Description("stats_notification")]
		StatsNotification,
	}
}
