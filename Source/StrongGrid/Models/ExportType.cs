using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the type of export job.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ExportType
	{
		/// <summary>
		/// Contacts export.
		/// </summary>
		[EnumMember(Value = "contacts_export")]
		Contacts,

		/// <summary>
		/// Lists export.
		/// </summary>
		[EnumMember(Value = "list_export")]
		Lists,

		/// <summary>
		/// Segments export.
		/// </summary>
		[EnumMember(Value = "segment_export")]
		Segments,
	}
}
