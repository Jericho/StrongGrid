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
		[EnumMember(Value = "unclassified")]
		Unclassified,

		/// <summary>
		/// Invalid Addres.
		/// </summary>
		[EnumMember(Value = "invalid")]
		InvalidAddress,

		/// <summary>
		/// Technical.
		/// </summary>
		[EnumMember(Value = "technical")]
		Technical,

		/// <summary>
		/// Content.
		/// </summary>
		[EnumMember(Value = "content")]
		Content,

		/// <summary>
		/// Reputation.
		/// </summary>
		[EnumMember(Value = "reputation")]
		Reputation,

		/// <summary>
		/// Frequency or volume.
		/// </summary>
		[EnumMember(Value = "frequency")] // SendGrid does not document this value. I took a wild guess.
		FrequencyOrVolume,

		/// <summary>
		/// Mailbox Unavailable.
		/// </summary>
		[EnumMember(Value = "mailbox")] // SendGrid does not document this value. I took a wild guess.
		MailboxUnavailable,
	}
}
