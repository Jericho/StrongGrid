using Newtonsoft.Json;
using StrongGrid.Utilities;

namespace StrongGrid.Model
{
	public class BatchInfo
	{
		[JsonProperty("batch_id")]
		public string BatchId { get; set; }

		[JsonProperty("status")]
		[JsonConverter(typeof(EnumDescriptionConverter))]
		public BatchStatus Status { get; set; }
	}
}
