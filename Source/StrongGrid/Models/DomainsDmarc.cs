using Newtonsoft.Json;

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
		[JsonProperty("soft_failures", NullValueHandling = NullValueHandling.Ignore)]
		public string[] SoftFailures { get; set; }

		/// <summary>
		/// Gets or sets the domains that will not deliver mail when used as a sender identity due to the domain's DMARC policy settings.
		/// </summary>
		/// <value>
		/// The aray of domains.
		/// </value>
		[JsonProperty("hard_failures", NullValueHandling = NullValueHandling.Ignore)]
		public string[] HardFailures { get; set; }
	}
}
