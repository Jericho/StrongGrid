using Newtonsoft.Json;
using System.Collections.Generic;

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
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the URI for putting the upload file.
		/// </summary>
		/// <value>
		/// The upload file URI.
		/// </value>
		[JsonProperty("upload_uri", NullValueHandling = NullValueHandling.Ignore)]
		public string UploadUri { get; set; }

		/// <summary>
		/// Gets or sets the headers that must be included in PUT request.
		/// </summary>
		/// <value>
		/// The headers.
		/// </value>
		[JsonProperty("upload_headers", NullValueHandling = NullValueHandling.Ignore)]
		public KeyValuePair<string, string>[] Headers { get; set; }
	}
}
