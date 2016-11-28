using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// Search condition
	/// </summary>
	public class SearchCondition
	{
		/// <summary>
		/// Gets or sets the field.
		/// </summary>
		/// <value>
		/// The field.
		/// </value>
		[JsonProperty("field")]
		public string Field { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>
		/// The value.
		/// </value>
		[JsonProperty("value")]
		public string Value { get; set; }

		/// <summary>
		/// Gets or sets the operator.
		/// </summary>
		/// <value>
		/// The operator.
		/// </value>
		[JsonProperty("operator")]
		public ConditionOperator Operator { get; set; }

		/// <summary>
		/// Gets or sets the logical operator.
		/// </summary>
		/// <value>
		/// The logical operator.
		/// </value>
		[JsonProperty("and_or")]
		public LogicalOperator LogicalOperator { get; set; }
	}
}
