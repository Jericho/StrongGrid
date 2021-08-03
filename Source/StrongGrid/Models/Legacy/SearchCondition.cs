using System.Text.Json.Serialization;

namespace StrongGrid.Models.Legacy
{
	/// <summary>
	/// Search condition.
	/// </summary>
	public class SearchCondition
	{
		/// <summary>
		/// Gets or sets the field.
		/// </summary>
		/// <value>
		/// The field.
		/// </value>
		[JsonPropertyName("field")]
		public string Field { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>
		/// The value.
		/// </value>
		[JsonPropertyName("value")]
		public string Value { get; set; }

		/// <summary>
		/// Gets or sets the operator.
		/// </summary>
		/// <value>
		/// The operator.
		/// </value>
		[JsonPropertyName("operator")]
		public ConditionOperator Operator { get; set; }

		/// <summary>
		/// Gets or sets the logical operator.
		/// </summary>
		/// <value>
		/// The logical operator.
		/// </value>
		[JsonPropertyName("and_or")]
		public LogicalOperator LogicalOperator { get; set; }
	}
}
