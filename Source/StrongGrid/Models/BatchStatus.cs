using StrongGrid.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the status of a batch.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter<BatchStatus>))]
	public enum BatchStatus
	{
		/// <summary>
		/// Batch has been paused.
		/// </summary>
		[EnumMember(Value = "pause")]
		Paused,

		/// <summary>
		/// Batch has been canceled.
		/// </summary>
		[EnumMember(Value = "cancel")]
		Canceled
	}
}
