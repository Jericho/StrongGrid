
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Domain validation.
	/// </summary>
	public class DomainValidation
	{
		/// <summary>
		/// Gets or sets the domain identifier.
		/// </summary>
		/// <value>
		/// The domain identifier.
		/// </value>
		[JsonPropertyName("id")]
		public long DomainId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the domain is valid.
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
		public DomainValidationResults ValidationResults { get; set; }
	}
}
