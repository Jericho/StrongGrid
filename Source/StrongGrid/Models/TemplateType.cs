using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the type of template.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
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
