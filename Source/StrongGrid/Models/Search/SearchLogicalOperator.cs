using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Enumeration to indicate a logical operator.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum SearchLogicalOperator
	{
		/// <summary>
		/// And.
		/// </summary>
		[EnumMember(Value = "AND")]
		And,

		/// <summary>
		/// Or.
		/// </summary>
		[EnumMember(Value = "OR")]
		Or
	}
}
