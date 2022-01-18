using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// List of domains known to implement DMARC and categorizes them by failure type — hard failure or soft failure.
	/// </summary>
	public class DomainsDmarc
	{
		/// <summary>
		/// Gets or sets the domains that will ??? when used as a sender identity due to the domain's DMARC policy settings.
		/// </summary>
		/// <value>
		/// The array of domains.
		/// </value>
		[JsonPropertyName("soft_failures")]
		public string[] SoftFailures { get; set; }

		/// <summary>
		/// Gets or sets the domains that will not deliver mail when used as a sender identity due to the domain's DMARC policy settings.
		/// </summary>
		/// <value>
		/// The aray of domains.
		/// </value>
		[JsonPropertyName("hard_failures")]
		public string[] HardFailures { get; set; }
	}
}
