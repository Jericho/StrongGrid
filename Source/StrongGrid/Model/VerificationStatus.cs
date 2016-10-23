using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class VerificationStatus
	{
		[JsonProperty("status")]
		public bool IsCompleted { get; set; }

		[JsonProperty("reason")]
		public string Reason { get; set; }
	}
}
