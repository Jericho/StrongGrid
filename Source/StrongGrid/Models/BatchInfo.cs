using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Information about a batch.
	/// </summary>
	public class BatchInfo
	{
		/// <summary>
		/// Gets or sets the batch identifier.
		/// </summary>
		/// <value>
		/// The batch identifier.
		/// </value>
		[JsonProperty("batch_id", NullValueHandling = NullValueHandling.Ignore)]
		public string BatchId { get; set; }

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		[JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
		public BatchStatus Status { get; set; }
	}
}
