using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the status of a campaign
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum CampaignStatus
	{
		/// <summary>
		/// The campaign has been canceled
		/// </summary>
		[EnumMember(Value = "canceled")]
		Canceled,

		/// <summary>
		/// The campaign is in draft mode
		/// </summary>
		[EnumMember(Value = "draft")]
		Draft,

		/// <summary>
		/// The campaign is beeing delivered
		/// </summary>
		[EnumMember(Value = "in progress")]
		InProgress,

		/// <summary>
		/// The campaign has been scheduled
		/// </summary>
		[EnumMember(Value = "scheduled")]
		Scheduled,

		/// <summary>
		/// The campaign has been sent
		/// </summary>
		[EnumMember(Value = "sent")]
		Sent
	}
}
