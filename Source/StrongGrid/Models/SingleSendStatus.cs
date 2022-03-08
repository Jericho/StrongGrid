using StrongGrid.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the status of a single send.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<SingleSendStatus>))]
	public enum SingleSendStatus
	{
		/// <summary>
		/// The single send is in draft mode.
		/// </summary>
		[EnumMember(Value = "draft")]
		Draft,

		/// <summary>
		/// The single send has been scheduled.
		/// </summary>
		[EnumMember(Value = "scheduled")]
		Scheduled,

		/// <summary>
		/// The campaign has been triggered.
		/// </summary>
		[EnumMember(Value = "triggered")]
		Triggered
	}
}
