using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrongGrid.Model
{
	public enum BatchStatus
	{
//		[Description("pause")]
		[EnumMember(Value = "pause")]

		Paused,
//		[Description("cancel")]
		[EnumMember(Value = "cancel")]
		Canceled
	}
}
