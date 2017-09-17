using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// An email attachment
	/// </summary>
	public class Attachment
	{
		/// <summary>
		/// Gets or sets the content.
		/// </summary>
		/// <value>
		/// The content.
		/// </value>
		[JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
		public string Content { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the name of the file.
		/// </summary>
		/// <value>
		/// The name of the file.
		/// </value>
		[JsonProperty("filename", NullValueHandling = NullValueHandling.Ignore)]
		public string FileName { get; set; }

		/// <summary>
		/// Gets or sets the disposition.
		/// </summary>
		/// <value>
		/// The disposition.
		/// </value>
		[JsonProperty("disposition", NullValueHandling = NullValueHandling.Ignore)]
		public string Disposition { get; set; }

		/// <summary>
		/// Gets or sets the content identifier.
		/// </summary>
		/// <value>
		/// The content identifier.
		/// </value>
		[JsonProperty("content_id", NullValueHandling = NullValueHandling.Ignore)]
		public string ContentId { get; set; }
	}
}
