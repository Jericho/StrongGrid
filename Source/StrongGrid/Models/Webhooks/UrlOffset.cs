using StrongGrid.Utilities;
using System.Text.Json.Serialization;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// UrlOffset gives you more information about the link that was clicked.
	/// </summary>
	public class UrlOffset
	{
		/// <summary>
		/// Gets or sets the index.
		/// </summary>
		/// <remarks>
		/// Links are indexed beginning at 0.
		/// Index indicates which link was clicked based on that index.
		/// </remarks>
		/// <value>
		/// The index.
		/// </value>
		[JsonPropertyName("index")]
		public int Index { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// The type of link can be either text, HTML, or a header.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonPropertyName("type")]
		[JsonConverter(typeof(StringEnumConverter<UrlType>))]
		public UrlType Type { get; set; }
	}
}
