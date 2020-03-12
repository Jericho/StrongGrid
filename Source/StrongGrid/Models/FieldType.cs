using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the type of a field.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum FieldType
	{
		/// <summary>
		/// Date.
		/// </summary>
		[EnumMember(Value = "Date")]
		Date,

		/// <summary>
		/// Text.
		/// </summary>
		[EnumMember(Value = "Text")]
		Text,

		/// <summary>
		/// Number.
		/// </summary>
		[EnumMember(Value = "Number")]
		Number
	}
}
