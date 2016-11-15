using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrongGrid.Model
{
	public enum LogicalOperator
	{
		[Description("")]
		None,
		[Description("and")]
		And,
		[Description("or")]
		Or
	}
}
