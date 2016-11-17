using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class DomainValidation
	{
		[JsonProperty("id")]
		public long DomainId { get; set; }

		[JsonProperty("valid")]
		public bool IsValid { get; set; }

		[JsonProperty("validation_resuts")]
		public DomainValidationResults ValidationResults { get; set; }
	}
}
