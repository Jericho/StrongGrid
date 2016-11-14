using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class LinkValidation
	{
		[JsonProperty("id")]
		public long LinkId { get; set; }

		[JsonProperty("valid")]
		public bool IsValid { get; set; }

		[JsonProperty("validation_results")]
		public LinkValidationResults ValidationResults { get; set; }
	}
}
