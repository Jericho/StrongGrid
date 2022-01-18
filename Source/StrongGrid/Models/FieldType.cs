using StrongGrid.Utilities;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the type of a field.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<FieldType>))]
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
