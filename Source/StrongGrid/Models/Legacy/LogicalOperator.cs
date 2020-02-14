using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Models.Legacy
{
	/// <summary>
	/// Enumeration to indicate a logical operator.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum LogicalOperator
	{
		/// <summary>
		/// None.
		/// </summary>
		[EnumMember(Value = "")]
		None,

		/// <summary>
		/// And.
		/// </summary>
		[EnumMember(Value = "and")]
		And,

		/// <summary>
		/// Or.
		/// </summary>
		[EnumMember(Value = "or")]
		Or
	}
}
