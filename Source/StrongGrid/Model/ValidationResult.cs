using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class ValidationResult
	{
		[JsonProperty("valid")]
		public bool IsValid { get; set; }

		[JsonProperty("reason")]
		public string Reason { get; set; }
	}
}
