using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the type of export file.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum FileType
	{
		/// <summary>
		/// CSV.
		/// </summary>
		[EnumMember(Value = "csv")]
		Csv,

		/// <summary>
		/// JSON.
		/// </summary>
		[EnumMember(Value = "json")]
		Json
	}
}
