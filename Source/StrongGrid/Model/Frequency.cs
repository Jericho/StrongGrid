using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Model
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum Frequency
	{
		[EnumMember(Value = "hourly")]
		Hourly,
		[EnumMember(Value = "daily")]
		Daily,
		[EnumMember(Value = "weekly")]
		Weekly,
		[EnumMember(Value = "monthly")]
		Monthly
	}
}
