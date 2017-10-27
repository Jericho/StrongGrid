using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the editor used in the UI.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum EditorType
	{
		/// <summary>
		/// This editor allows you to edit the HTML source
		/// </summary>
		[EnumMember(Value = "code")]
		Code,

		/// <summary>
		/// This editor allows you to drag and drop snippets
		/// </summary>
		[EnumMember(Value = "design")]
		Design
	}
}
