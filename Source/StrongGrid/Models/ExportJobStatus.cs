using StrongGrid.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the status of an export job.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<ExportJobStatus>))]
	public enum ExportJobStatus
	{
		/// <summary>
		/// Job is not started.
		/// </summary>
		[EnumMember(Value = "pending")]
		Pending,

		/// <summary>
		/// Job is finished without any errors.
		/// </summary>
		[EnumMember(Value = "ready")]
		Ready,

		/// <summary>
		/// Job has failed.
		/// </summary>
		[EnumMember(Value = "failure")]
		Failed
	}
}
