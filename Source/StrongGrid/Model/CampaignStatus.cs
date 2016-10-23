using System.ComponentModel;

namespace StrongGrid.Model
{
	public enum CampaignStatus
	{
		[Description("draft")]
		Draft,
		[Description("scheduled")]
		Scheduled,
		[Description("sent")]
		Sent
	}
}
