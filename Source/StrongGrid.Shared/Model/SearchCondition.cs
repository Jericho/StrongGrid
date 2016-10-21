using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StrongGrid.Model
{
	public class SearchCondition
	{
		[JsonProperty("field")]
		public string Field { get; set; }

		[JsonProperty("value")]
		public string Value { get; set; }

		[JsonProperty("operator")]
		[JsonConverter(typeof(StringEnumConverter))]
		public ConditionOperator Operator { get; set; }

		[JsonProperty("and_or")]
		[JsonConverter(typeof(StringEnumConverter))]
		public LogicalOperator LogicalOperator { get; set; }
	}
}
