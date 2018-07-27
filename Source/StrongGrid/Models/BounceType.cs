using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the type of bounce.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
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
