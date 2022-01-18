using System.Text.Json.Serialization;

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
		[JsonPropertyName("batch_id")]
		public string BatchId { get; set; }

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		[JsonPropertyName("status")]
		public BatchStatus Status { get; set; }
	}
}
