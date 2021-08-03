using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Validation result.
	/// </summary>
	public class ValidationResult
	{
		/// <summary>
		/// Gets or sets a value indicating whether this validation is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("valid")]
		public bool IsValid { get; set; }

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
