using StrongGrid.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the editor used in the UI.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<EditorType>))]
	public enum EditorType
	{
		/// <summary>
		/// Unspecified.
		/// </summary>
		[EnumMember(Value = null)]
		Unspecified,

		/// <summary>
		/// This editor allows you to edit the HTML source.
		/// </summary>
		[EnumMember(Value = "code")]
		Code,

		/// <summary>
		/// This editor allows you to drag and drop snippets.
		/// </summary>
		[EnumMember(Value = "design")]
		Design
	}
}
