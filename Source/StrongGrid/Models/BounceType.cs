using StrongGrid.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the type of bounce.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<BounceType>))]
	public enum BounceType
	{
		/// <summary>
		/// Bounce.
		/// </summary>
		[EnumMember(Value = "bounce")]
		Bounce,

		/// <summary>
		/// Blocked.
		/// </summary>
		[EnumMember(Value = "blocked")]
		Blocked,

		/// <summary>
		/// Expired.
		/// </summary>
		[EnumMember(Value = "expired")]
		Expired
	}
}
