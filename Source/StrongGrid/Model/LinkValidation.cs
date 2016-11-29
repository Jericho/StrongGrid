using Newtonsoft.Json;

namespace StrongGrid.Model
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
		[JsonProperty("id")]
		public long LinkId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this link is valid.
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
		[JsonProperty("validation_results")]
		public LinkValidationResults ValidationResults { get; set; }
	}
}
