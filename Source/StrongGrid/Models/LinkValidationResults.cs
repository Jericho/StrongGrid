using System.Text.Json.Serialization;

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
		[JsonPropertyName("domain_cname")]
		public ValidationResult Domain { get; set; }

		/// <summary>
		/// Gets or sets the owner.
		/// </summary>
		/// <value>
		/// The owner.
		/// </value>
		[JsonPropertyName("owner_cname")]
		public ValidationResult Owner { get; set; }
	}
}
