using StrongGrid.Json;
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models.Legacy
{
	/// <summary>
	/// Enumeration to indicate a logical operator.
	/// </summary>
	[Obsolete("The legacy client, legacy resources and legacy model classes are obsolete")]
	[JsonConverter(typeof(StringEnumConverter<LogicalOperator>))]
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
