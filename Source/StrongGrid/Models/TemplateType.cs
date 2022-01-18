using StrongGrid.Utilities;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the type of template.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<TemplateType>))]
	public enum TemplateType
	{
		/// <summary>
		/// The old type of template.
		/// </summary>
		[EnumMember(Value = "legacy")]
		Legacy,

		/// <summary>
		/// The new dynamic type of template.
		/// </summary>
		[EnumMember(Value = "dynamic")]
		Dynamic
	}
}
