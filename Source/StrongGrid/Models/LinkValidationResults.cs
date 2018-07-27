using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Link validation results.
	/// </summary>
	public class LinkValidationResults
	{
		/// <summary>
		/// Gets or sets the domain.
		/// </summary>
		/// <value>
		/// The domain.
		/// </value>
		[JsonProperty("domain_cname", NullValueHandling = NullValueHandling.Ignore)]
		public ValidationResult Domain { get; set; }

		/// <summary>
		/// Gets or sets the owner.
		/// </summary>
		/// <value>
		/// The owner.
		/// </value>
		[JsonProperty("owner_cname", NullValueHandling = NullValueHandling.Ignore)]
		public ValidationResult Owner { get; set; }
	}
}
