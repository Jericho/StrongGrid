using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class IpValidation
	{
		[JsonProperty("id")]
		public long IpId { get; set; }

		[JsonProperty("valid")]
		public bool IsValid { get; set; }

		[JsonProperty("validation_results")]
		public IpValidationResults ValidationResults { get; set; }
	}
}
