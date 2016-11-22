using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Model
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum AggregateBy
	{
		[EnumMember(Value = "none")]
		None,
		[EnumMember(Value = "day")]
		Day,
		[EnumMember(Value = "week")]
		Week,
		[EnumMember(Value = "month")]
		Month
	}
}
