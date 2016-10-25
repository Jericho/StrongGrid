using System.Runtime.Serialization;

namespace StrongGrid.Model
{
	public enum LogicalOperator
	{
		[EnumMember(Value = "")]
		None,
		[EnumMember(Value = "and")]
		And,
		[EnumMember(Value = "or")]
		Or
	}
}
