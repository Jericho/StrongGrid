using Newtonsoft.Json;
using StrongGrid.Utilities;

namespace StrongGrid.Model
{
	public class SearchCondition
	{
		[JsonProperty("field")]
		public string Field { get; set; }

		[JsonProperty("value")]
		public string Value { get; set; }

		[JsonProperty("operator")]
		[JsonConverter(typeof(EnumDescriptionConverter))]
		public ConditionOperator Operator { get; set; }

		[JsonProperty("and_or")]
		[JsonConverter(typeof(EnumDescriptionConverter))]
		public LogicalOperator LogicalOperator { get; set; }
	}
}
