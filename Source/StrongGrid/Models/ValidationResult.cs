using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Validation result
	/// </summary>
	public class ValidationResult
	{
		/// <summary>
		/// Gets or sets a value indicating whetherthis validation is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("valid", NullValueHandling = NullValueHandling.Ignore)]
		public bool IsValid { get; set; }

		/// <summary>
		/// Gets or sets the reason.
		/// </summary>
		/// <value>
		/// The reason.
		/// </value>
		[JsonProperty("reason", NullValueHandling = NullValueHandling.Ignore)]
		public string Reason { get; set; }
	}
}
