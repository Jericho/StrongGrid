using StrongGrid.Utilities;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models.Legacy
{
	/// <summary>
	/// Enumeration to indicate the status of a campaign.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<CampaignStatus>))]
	public enum CampaignStatus
	{
		/// <summary>
		/// The campaign has been canceled.
		/// </summary>
		[EnumMember(Value = "canceled")]
		Canceled,

		/// <summary>
		/// The campaign is in draft mode.
		/// </summary>
		[EnumMember(Value = "draft")]
		Draft,

		/// <summary>
		/// The campaign is beeing delivered.
		/// </summary>
		[EnumMember(Value = "in progress")]
		InProgress,

		/// <summary>
		/// The campaign has been scheduled.
		/// </summary>
		[EnumMember(Value = "scheduled")]
		Scheduled,

		/// <summary>
		/// The campaign has been sent.
		/// </summary>
		[EnumMember(Value = "sent")]
		Sent
	}
}
