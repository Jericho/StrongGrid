using StrongGrid.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the type of SMTP failure.
	/// </summary>
	/// <remarks>
	/// SendGrid says there are seven possible classifications but the only documented json value is 'invalid'.
	/// As of January 2022, we haven't been able to get confirmation from SendGrid support for the other six values.
	///
	/// Therefore there's a high likelihood that you'll get an exception when we attempt to parse the json value.
	/// The exception you'll get will be similar to: "There is no value in the BounceClassification enum that
	/// corresponds to 'THE JSON VALUE'". Please notify us <a href="https://github.com/Jericho/StrongGrid/issues/477">here</a>
	/// if you get this exception so we can update the enum.
	/// </remarks>
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
