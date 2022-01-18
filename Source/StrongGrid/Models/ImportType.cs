using StrongGrid.Utilities;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the type of import job.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<ImportType>))]
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
		Delete,

		/// <summary>
		/// Update/insert contacts.
		/// </summary>
		[EnumMember(Value = "upsert_contacts")]
		UpsertContacts
	}
}
