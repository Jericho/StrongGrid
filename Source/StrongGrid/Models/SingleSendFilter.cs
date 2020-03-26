using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Single Send filter.
	/// </summary>
	public class SingleSendFilter
	{
		/// <summary>
		/// Gets or sets the lists.
		/// </summary>
		/// <value>
		/// The lists.
		/// </value>
		[JsonProperty("list_ids", NullValueHandling = NullValueHandling.Ignore)]
		public string[] Lists { get; set; }

		/// <summary>
		/// Gets or sets the segments.
		/// </summary>
		/// <value>
		/// The segments.
		/// </value>
		[JsonProperty("segment_ids", NullValueHandling = NullValueHandling.Ignore)]
		public string[] Segments { get; set; }
	}
}
