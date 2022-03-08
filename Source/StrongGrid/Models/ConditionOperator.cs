using StrongGrid.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate a condition operator.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<ConditionOperator>))]
	public enum ConditionOperator
	{
		/// <summary>
		/// Equal.
		/// </summary>
		[EnumMember(Value = "eq")]
		Equal,

		/// <summary>
		/// Not equal.
		/// </summary>
		[EnumMember(Value = "ne")]
		NotEqual,

		/// <summary>
		/// Less than.
		/// </summary>
		[EnumMember(Value = "lt")]
		LessThan,

		/// <summary>
		/// Greather than.
		/// </summary>
		[EnumMember(Value = "gt")]
		GreatherThan,

		/// <summary>
		/// Contains.
		/// </summary>
		[EnumMember(Value = "contains")]
		Contains
	}
}
