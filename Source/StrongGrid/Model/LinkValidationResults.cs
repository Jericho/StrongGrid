using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class LinkValidationResults
	{
		[JsonProperty("domain_cname")]
		public ValidationResult Domain { get; set; }

		[JsonProperty("owner_cname")]
		public ValidationResult Owner { get; set; }
	}
}
