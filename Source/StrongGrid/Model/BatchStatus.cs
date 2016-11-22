using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Model
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum BatchStatus
	{
		[EnumMember(Value = "pause")]
		Paused,
		[EnumMember(Value = "cancel")]
		Canceled
	}
}
