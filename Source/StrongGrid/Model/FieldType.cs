using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Model
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum FieldType
	{
		[EnumMember(Value = "date")]
		Date,
		[EnumMember(Value = "text")]
		Text,
		[EnumMember(Value = "number")]
		Number
	}
}
