using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Enumeration to indicate the status of the email activity.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum EmailActivityStatus
	{
		/// <summary>
		/// Message is processing.
		/// </summary>
		[EnumMember(Value = "processing")]
		Processing,

		/// <summary>
		/// Message has been received and is ready to be delivered.
		/// </summary>
		[EnumMember(Value = "processed")]
		Processed,

		/// <summary>
		/// Message has not been delivered.
		/// </summary>
		[EnumMember(Value = "not_delivered")]
		NotDelivered,

		/// <summary>
		/// Message has been successfully delivered to the receiving server.
		/// </summary>
		[EnumMember(Value = "delivered")]
		Delivered
	}
}
