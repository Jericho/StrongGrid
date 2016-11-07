using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class IpValidationResults
	{
		[JsonProperty("a_record")]
		public ValidationResult ARecord { get; set; }
	}
}
