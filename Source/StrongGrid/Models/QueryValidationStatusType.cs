using StrongGrid.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the status of a segment query.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<QueryValidationStatusType>))]
	public enum QueryValidationStatusType
	{
		/// <summary>
		/// Pending.
		/// </summary>
		[EnumMember(Value = "pending")]
		Pending,

		/// <summary>
		/// Valid.
		/// </summary>
		[EnumMember(Value = "valid")]
		Valid,

		/// <summary>
		/// Invalid.
		/// </summary>
		[EnumMember(Value = "invalid")]
		Invalid,
	}
}
