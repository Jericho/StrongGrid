using StrongGrid.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the type of SMTP failure.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<BounceClassification>))]
	public enum BounceClassification
	{
		/// <summary>
		/// Unclassified.
		/// </summary>
		[EnumMember(Value = "Unclassified")]
		Unclassified,

		/// <summary>
		/// Invalid Addres.
		/// </summary>
		[EnumMember(Value = "Invalid Address")]
		InvalidAddress,

		/// <summary>
		/// Technical.
		/// </summary>
		[EnumMember(Value = "Technical")]
		Technical,

		/// <summary>
		/// Content.
		/// </summary>
		[EnumMember(Value = "Content")]
		Content,

		/// <summary>
		/// Reputation.
		/// </summary>
		[EnumMember(Value = "Reputation")]
		Reputation,

		/// <summary>
		/// Frequency or volume.
		/// </summary>
		[EnumMember(Value = "Frequency or Volume Too High")]
		FrequencyOrVolume,

		/// <summary>
		/// Mailbox Unavailable.
		/// </summary>
		[EnumMember(Value = "Mailbox Unavailable")]
		MailboxUnavailable,
	}
}
