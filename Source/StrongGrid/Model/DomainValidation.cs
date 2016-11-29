using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// Domain validation
	/// </summary>
	public class DomainValidation
	{
		/// <summary>
		/// Gets or sets the domain identifier.
		/// </summary>
		/// <value>
		/// The domain identifier.
		/// </value>
		[JsonProperty("id")]
		public long DomainId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the domain is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("valid")]
		public bool IsValid { get; set; }

		/// <summary>
		/// Gets or sets the validation results.
		/// </summary>
		/// <value>
		/// The validation results.
		/// </value>
		[JsonProperty("validation_resuts")]
		public DomainValidationResults ValidationResults { get; set; }
	}
}
