using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the type of a field
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum FieldType
	{
		/// <summary>
		/// Date
		/// </summary>
		[EnumMember(Value = "date")]
		Date,

		/// <summary>
		/// Text
		/// </summary>
		[EnumMember(Value = "text")]
		Text,

		/// <summary>
		/// Number
		/// </summary>
		[EnumMember(Value = "number")]
		Number
	}
}
