using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Verification status.
	/// </summary>
	public class VerificationStatus
	{
		/// <summary>
		/// Gets or sets a value indicating whether this instance is completed.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is completed; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("status")]
		public bool IsCompleted { get; set; }

		/// <summary>
		/// Gets or sets the reason.
		/// </summary>
		/// <value>
		/// The reason.
		/// </value>
		[JsonPropertyName("reason")]
		public string Reason { get; set; }
	}
}
