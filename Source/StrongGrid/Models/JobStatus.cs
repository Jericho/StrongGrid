using StrongGrid.Utilities;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the status of an import/export job.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<JobStatus>))]
	public enum JobStatus
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
		/// Job finished with some errors.
		/// </summary>
		[EnumMember(Value = "errored")]
		Errored,

		/// <summary>
		/// Job has finshed with all errors or was entirely unprocessable (eg. If you attempt to import file format we do not support).
		/// </summary>
		[EnumMember(Value = "failed")]
		Failed
	}
}
