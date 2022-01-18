using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Link validation.
	/// </summary>
	public class LinkValidation
	{
		/// <summary>
		/// Gets or sets the link identifier.
		/// </summary>
		/// <value>
		/// The link identifier.
		/// </value>
		[JsonPropertyName("id")]
		public long LinkId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this link is valid.
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
		public LinkValidationResults ValidationResults { get; set; }
	}
}
