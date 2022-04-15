using System.Runtime.Serialization;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Enumeration to indicate the filtering operator when searching.
	/// </summary>
	public enum SearchComparisonOperator
	{
		/// <summary>
		/// Equal.
		/// </summary>
		[EnumMember(Value = "=")]
		Equal,

		/// <summary>
		/// Not equal.
		/// </summary>
		[EnumMember(Value = "!=")]
		NotEqual,

		/// <summary>
		/// Greather than.
		/// </summary>
		[EnumMember(Value = ">")]
		GreaterThan,

		/// <summary>
		/// Less than.
		/// </summary>
		[EnumMember(Value = "<")]
		LessThan,

		/// <summary>
		/// Greather than or equal.
		/// </summary>
		[EnumMember(Value = ">=")]
		GreaterEqual,

		/// <summary>
		/// Less than or equal.
		/// </summary>
		[EnumMember(Value = "<=")]
		LessEqual,

		/// <summary>
		/// Between.
		/// </summary>
		[EnumMember(Value = "BETWEEN")]
		Between,

		/// <summary>
		/// Not between.
		/// </summary>
		[EnumMember(Value = "NOT BETWEEN")]
		NotBetween,

		/// <summary>
		/// In.
		/// </summary>
		[EnumMember(Value = "IN")]
		In,

		/// <summary>
		/// Not in.
		/// </summary>
		[EnumMember(Value = "NOT IN")]
		NotIn,

		/// <summary>
		/// Like.
		/// </summary>
		[EnumMember(Value = "LIKE")]
		Like,

		/// <summary>
		/// Not like.
		/// </summary>
		[EnumMember(Value = "NOT LIKE")]
		NotLike,

		/// <summary>
		/// Null.
		/// </summary>
		[EnumMember(Value = "IS NULL")]
		IsNull,

		/// <summary>
		/// Not Null.
		/// </summary>
		[EnumMember(Value = "IS NOT NULL")]
		IsNotNull,

		/// <summary>
		/// Contains.
		/// </summary>
		[EnumMember(Value = "CONTAINS")]
		Contains
	}
}
