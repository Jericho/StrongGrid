using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Model
{
	/// <summary>
	/// Enumeration to indicate the status of a campaign
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum CampaignStatus
	{
		/// <summary>
		/// The campaign is in draft mode
		/// </summary>
		[EnumMember(Value = "draft")]
		Draft,

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
