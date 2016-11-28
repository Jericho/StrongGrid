using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class VerificationStatus
	{
		/// <summary>
		/// Gets or sets a value indicating whether this instance is completed.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is completed; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("status")]
		public bool IsCompleted { get; set; }

		/// <summary>
		/// Gets or sets the reason.
		/// </summary>
		/// <value>
		/// The reason.
		/// </value>
		[JsonProperty("reason")]
		public string Reason { get; set; }
	}
}
