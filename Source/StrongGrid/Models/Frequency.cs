using StrongGrid.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate frequency.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<Frequency>))]
	public enum Frequency
	{
		/// <summary>
		/// Hourly.
		/// </summary>
		[EnumMember(Value = "hourly")]
		Hourly,

		/// <summary>
		/// Daily.
		/// </summary>
		[EnumMember(Value = "daily")]
		Daily,

		/// <summary>
		/// Weekly.
		/// </summary>
		[EnumMember(Value = "weekly")]
		Weekly,

		/// <summary>
		/// Monthly.
		/// </summary>
		[EnumMember(Value = "monthly")]
		Monthly
	}
}
