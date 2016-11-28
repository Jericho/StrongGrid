using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class BatchInfo
	{
		/// <summary>
		/// Gets or sets the batch identifier.
		/// </summary>
		/// <value>
		/// The batch identifier.
		/// </value>
		[JsonProperty("batch_id")]
		public string BatchId { get; set; }

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		[JsonProperty("status")]
		public BatchStatus Status { get; set; }
	}
}
