using StrongGrid.Utilities;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the type of export file.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<FileType>))]
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
