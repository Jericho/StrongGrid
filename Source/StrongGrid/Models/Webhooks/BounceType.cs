using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Enumeration to indicate the type of bounce.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
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
