using Newtonsoft.Json;

namespace StrongGrid.Models
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
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public long DomainId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the domain is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("valid", NullValueHandling = NullValueHandling.Ignore)]
		public bool IsValid { get; set; }

		/// <summary>
		/// Gets or sets the validation results.
		/// </summary>
		/// <value>
		/// The validation results.
		/// </value>
		[JsonProperty("validation_resuts", NullValueHandling = NullValueHandling.Ignore)]
		public DomainValidationResults ValidationResults { get; set; }
	}
}
