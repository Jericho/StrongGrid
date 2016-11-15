using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrongGrid.Model
{
	public enum Frequency
	{
		[Description("hourly")]
		Hourly,
		[Description("daily")]
		Daily,
		[Description("weekly")]
		Weekly,
		[Description("monthly")]
		Monthly
	}
}
