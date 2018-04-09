using System.Runtime.Serialization;

namespace StrongGrid.Models.Search
{
	/// <summary>
	/// Enumeration to indicate the filtering operator when searching email activities
	/// </summary>
	public enum SearchConditionOperator
	{
		/// <summary>
		/// Equal
		/// </summary>
		[EnumMember(Value = "=")]
		Equal,

		/// <summary>
		/// Not equal
		/// </summary>
		[EnumMember(Value = "!=")]
		NotEqual,

		/// <summary>
		/// Greather than
		/// </summary>
		[EnumMember(Value = ">")]
		GreaterThan,

		/// <summary>
		/// Less than
		/// </summary>
		[EnumMember(Value = "<")]
		LessThan,

		/// <summary>
		/// Greather than or equal
		/// </summary>
		[EnumMember(Value = ">=")]
		GreaterEqual,

		/// <summary>
		/// Less than or equal
		/// </summary>
		[EnumMember(Value = "<=")]
		LessEqual,

		/// <summary>
		/// Between
		/// </summary>
		[EnumMember(Value = "BETWEEN")]
		Beetween,

		/// <summary>
		/// Not between
		/// </summary>
		[EnumMember(Value = "NOT BETWEEN")]
		NotBetween,

		/// <summary>
		/// In
		/// </summary>
		[EnumMember(Value = "IN")]
		In,

		/// <summary>
		/// Not in
		/// </summary>
		[EnumMember(Value = "NOT IN")]
		NotIn,

		/// <summary>
		/// Is
		/// </summary>
		[EnumMember(Value = "IS")]
		Is,

		/// <summary>
		/// Is not
		/// </summary>
		[EnumMember(Value = "IS NOT")]
		IsNot,

		/// <summary>
		/// Like
		/// </summary>
		[EnumMember(Value = "LIKE")]
		Like,

		/// <summary>
		/// Not like
		/// </summary>
		[EnumMember(Value = "NOT LIKE")]
		NotLike
	}
}
