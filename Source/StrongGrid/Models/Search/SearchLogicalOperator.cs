using StrongGrid.Utilities;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Enumeration to indicate a logical operator.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<SearchLogicalOperator>))]
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
