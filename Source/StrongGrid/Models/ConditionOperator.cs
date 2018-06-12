using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate a condition operator.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
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
