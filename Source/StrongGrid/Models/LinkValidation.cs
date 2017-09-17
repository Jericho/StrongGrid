using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Link validation
	/// </summary>
	public class LinkValidation
	{
		/// <summary>
		/// Gets or sets the link identifier.
		/// </summary>
		/// <value>
		/// The link identifier.
		/// </value>
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public long LinkId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this link is valid.
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
		[JsonProperty("validation_results", NullValueHandling = NullValueHandling.Ignore)]
		public LinkValidationResults ValidationResults { get; set; }
	}
}
