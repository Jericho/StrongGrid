using StrongGrid.Utilities;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate how to aggregate statistics.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<AggregateBy>))]
	public enum AggregateBy
	{
		/// <summary>
		/// No aggregation.
		/// </summary>
		[EnumMember(Value = "none")]
		None,

		/// <summary>
		/// Aggregate by day.
		/// </summary>
		[EnumMember(Value = "day")]
		Day,

		/// <summary>
		/// Aggregate by week.
		/// </summary>
		[EnumMember(Value = "week")]
		Week,

		/// <summary>
		/// Aggregate by month.
		/// </summary>
		[EnumMember(Value = "month")]
		Month
	}
}
