using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Reverse DNS validation.
	/// </summary>
	public class ReverseDnsValidation
	{
		/// <summary>
		/// Gets or sets the ip identifier.
		/// </summary>
		/// <value>
		/// The ip identifier.
		/// </value>
		[JsonPropertyName("id")]
		public long IpId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the IP is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("valid")]
		public bool IsValid { get; set; }

		/// <summary>
		/// Gets or sets the validation results.
		/// </summary>
		/// <value>
		/// The validation results.
		/// </value>
		[JsonPropertyName("validation_results")]
		public ReverseDnsValidationResults ValidationResults { get; set; }
	}
}
