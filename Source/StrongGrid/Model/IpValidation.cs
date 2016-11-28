using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// IP validation
	/// </summary>
	public class IpValidation
	{
		/// <summary>
		/// Gets or sets the ip identifier.
		/// </summary>
		/// <value>
		/// The ip identifier.
		/// </value>
		[JsonProperty("id")]
		public long IpId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the IP is valid.
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
		public IpValidationResults ValidationResults { get; set; }
	}
}
