using System.Runtime.Serialization;

namespace StrongGrid.Models.Search
{
	public enum SearchConditionOperator
	{
		[EnumMember(Value = "=")]
		Equal,

		[EnumMember(Value = "!=")]
		NotEqual,

		[EnumMember(Value = ">")]
		GreaterThan,

		[EnumMember(Value = "<")]
		LessThan,

		[EnumMember(Value = ">=")]
		GreaterEqual,

		[EnumMember(Value = "<=")]
		LessEqual,

		[EnumMember(Value = "BETWEEN")]
		Beetween,

		[EnumMember(Value = "NOT BETWEEN")]
		NotBetween,

		[EnumMember(Value = "IN")]
		In,

		[EnumMember(Value = "NOT IN")]
		NotIn,

		[EnumMember(Value = "IS")]
		Is,

		[EnumMember(Value = "IS NOT")]
		IsNot,

		[EnumMember(Value = "LIKE")]
		Like,

		[EnumMember(Value = "NOT LIKE")]
		NotLike
	}
}
