using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class BatchInfo
	{
		[JsonProperty("batch_id")]
		public string BatchId { get; set; }

		[JsonProperty("status")]
		public BatchStatus Status { get; set; }
	}
}
