using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the type of import job.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ImportType
	{
		/// <summary>
		/// Update/insert import.
		/// </summary>
		[EnumMember(Value = "upsert")]
		Upsert,

		/// <summary>
		/// Delete import.
		/// </summary>
		[EnumMember(Value = "delete")]
		Delete
	}
}
