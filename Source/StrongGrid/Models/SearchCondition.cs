using Newtonsoft.Json;

namespace StrongGrid.Models
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
		[JsonProperty("field", NullValueHandling = NullValueHandling.Ignore)]
		public string Field { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>
		/// The value.
		/// </value>
		[JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
		public string Value { get; set; }

		/// <summary>
		/// Gets or sets the operator.
		/// </summary>
		/// <value>
		/// The operator.
		/// </value>
		[JsonProperty("operator", NullValueHandling = NullValueHandling.Ignore)]
		public ConditionOperator Operator { get; set; }

		/// <summary>
		/// Gets or sets the logical operator.
		/// </summary>
		/// <value>
		/// The logical operator.
		/// </value>
		[JsonProperty("and_or", NullValueHandling = NullValueHandling.Ignore)]
		public LogicalOperator LogicalOperator { get; set; }
	}
}
