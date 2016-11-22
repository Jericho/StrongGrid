using System.ComponentModel;

namespace StrongGrid.Model
{
	public enum ConditionOperator
	{
		[Description("eq")]
		Equal,
		[Description("ne")]
		NotEqual,
		[Description("lt")]
		LessThan,
		[Description("gt")]
		GreatherThan,
		[Description("contains")]
		Contains
	}
}
