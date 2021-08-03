using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Information about a newly created import job.
	/// </summary>
	public class ImportJobRequest
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonPropertyName("id")]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the URI for putting the upload file.
		/// </summary>
		/// <value>
		/// The upload file URI.
		/// </value>
		[JsonPropertyName("upload_uri")]
		public string UploadUri { get; set; }

		/// <summary>
		/// Gets or sets the headers that must be included in PUT request.
		/// </summary>
		/// <value>
		/// The headers.
		/// </value>
		[JsonPropertyName("upload_headers")]
		public KeyValuePair<string, string>[] Headers { get; set; }
	}
}
