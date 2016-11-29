using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Model
{
	/// <summary>
	/// Enumeration to indicate the status of a batch
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum BatchStatus
	{
		/// <summary>
		/// Batch has been paused
		/// </summary>
		[EnumMember(Value = "pause")]
		Paused,

		/// <summary>
		/// Batch has been canceled
		/// </summary>
		[EnumMember(Value = "cancel")]
		Canceled
	}
}
