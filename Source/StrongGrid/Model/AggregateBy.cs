using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Model
{
	/// <summary>
	/// Enumeration to indicate how to aggregate statistics
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum AggregateBy
	{
		/// <summary>
		/// No aggregation
		/// </summary>
		[EnumMember(Value = "none")]
		None,

		/// <summary>
		/// Aggregate by day
		/// </summary>
		[EnumMember(Value = "day")]
		Day,

		/// <summary>
		/// Aggregate by week
		/// </summary>
		[EnumMember(Value = "week")]
		Week,

		/// <summary>
		/// Aggregate by month
		/// </summary>
		[EnumMember(Value = "month")]
		Month
	}
}
