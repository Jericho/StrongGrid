using StrongGrid.Utilities;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Enumeration to indicate the type of bounce.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<BounceType>))]
	public enum BounceType
	{
		/// <summary>
		/// Hard bounce.
		/// </summary>
		[EnumMember(Value = "bounce")]
		HardBounce,

		/// <summary>
		/// Blocked.
		/// </summary>
		[EnumMember(Value = "blocked")]
		Block
	}
}
