using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class DomainValidationResults
	{
		[JsonProperty("mail_cname")]
		public ValidationResult Mail { get; set; }

		[JsonProperty("dkim1")]
		public ValidationResult Dkim1 { get; set; }

		[JsonProperty("dkim2")]
		public ValidationResult Dkim2 { get; set; }

		[JsonProperty("spf")]
		public ValidationResult Spf { get; set; }
	}
}
