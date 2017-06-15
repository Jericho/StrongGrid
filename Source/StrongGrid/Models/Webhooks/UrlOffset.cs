using Newtonsoft.Json;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// UrlOffset gives you more information about the link that was clicked.
	/// </summary>
	public class UrlOffset
	{
		/// <summary>
		/// Gets or sets the index.
		/// Links are indexed beginning at 0.
		/// Index indicates which link was clicked based on that index.
		/// </summary>
		/// <value>
		/// The index.
		/// </value>
		[JsonProperty("index", NullValueHandling = NullValueHandling.Ignore)]
		public int Index { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// The type of link can be either text, HTML, or a header.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
		public UrlType Type { get; set; }
	}
}
