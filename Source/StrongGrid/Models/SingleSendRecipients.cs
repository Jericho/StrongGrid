using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// The recipients of a Single Send.
	/// </summary>
	public class SingleSendRecipients
	{
		/// <summary>
		/// Gets or sets the lists.
		/// </summary>
		/// <value>
		/// The lists.
		/// </value>
		[JsonPropertyName("list_ids")]
		public string[] Lists { get; set; }

		/// <summary>
		/// Gets or sets the segments.
		/// </summary>
		/// <value>
		/// The segments.
		/// </value>
		[JsonPropertyName("segment_ids")]
		public string[] Segments { get; set; }
	}
}
