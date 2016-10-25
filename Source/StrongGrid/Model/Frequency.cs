using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrongGrid.Model
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum Frequency
	{
//		[Description("hourly")]
		[EnumMember(Value = "hourly")]
		Hourly,
		[EnumMember(Value = "daily")]
//		[Description("daily")]
		Daily,
		[EnumMember(Value = "weekly")]
//		[Description("weekly")]
		Weekly,
		[EnumMember(Value = "monthly")]
//		[Description("monthly")]
		Monthly
	}
}
