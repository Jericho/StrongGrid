using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class LinkValidationResults
	{
		/// <summary>
		/// Gets or sets the domain.
		/// </summary>
		/// <value>
		/// The domain.
		/// </value>
		[JsonProperty("domain_cname")]
		public ValidationResult Domain { get; set; }

		/// <summary>
		/// Gets or sets the owner.
		/// </summary>
		/// <value>
		/// The owner.
		/// </value>
		[JsonProperty("owner_cname")]
		public ValidationResult Owner { get; set; }
	}
}
