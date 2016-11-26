using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class SearchCondition
	{
		[JsonProperty("field")]
		public string Field { get; set; }

		[JsonProperty("value")]
		public string Value { get; set; }

		[JsonProperty("operator")]
		public ConditionOperator Operator { get; set; }

		[JsonProperty("and_or")]
		public LogicalOperator LogicalOperator { get; set; }
	}
}
