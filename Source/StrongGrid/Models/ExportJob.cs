using Newtonsoft.Json;
using System;

namespace StrongGrid.Models
{
	/// <summary>
	/// An export job allows you to export all contacts to a file (either csv or json) and to download this file.
	/// A given export job can produce multiple files if the amount of data exceeds the maximum MB per file.
	/// </summary>
	public class ExportJob
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
		/// Gets or sets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		[JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
		public JobStatus Status { get; set; }

		/// <summary>
		/// Gets or sets the date when the job was created.
		/// </summary>
		/// <value>
		/// The creation date.
		/// </value>
		[JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime CreatedOn { get; set; }

		/// <summary>
		/// Gets or sets the date when the job completed.
		/// </summary>
		/// <value>
		/// The completion date.
		/// </value>
		[JsonProperty("completed_at", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime CompletedOn { get; set; }

		/// <summary>
		/// Gets or sets the date when the job expires.
		/// </summary>
		/// <value>
		/// The expiration date.
		/// </value>
		[JsonProperty("expires_at", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime ExpiresOn { get; set; }

		/// <summary>
		/// Gets or sets the URLs of the files.
		/// </summary>
		/// <value>
		/// The URLs.
		/// </value>
		[JsonProperty("urls", NullValueHandling = NullValueHandling.Ignore)]
		public string[] FileUrls { get; set; }

		/// <summary>
		/// Gets or sets a human readable message if the status is 'failure'.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		[JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the user id.
		/// </summary>
		/// <value>
		/// The user identifier.
		/// </value>
		[JsonProperty("user_id", NullValueHandling = NullValueHandling.Ignore)]
		public string UserId { get; set; }

		/// <summary>
		/// Gets or sets the export type.
		/// </summary>
		/// <value>
		/// The export type.
		/// </value>
		[JsonProperty("export_type", NullValueHandling = NullValueHandling.Ignore)]
		public ExportType ExportType { get; set; }

		/// <summary>
		/// Gets or sets the lists.
		/// </summary>
		/// <value>
		/// The lists.
		/// </value>
		[JsonProperty("lists", NullValueHandling = NullValueHandling.Ignore)]
		public List[] Lists { get; set; }

		/// <summary>
		/// Gets or sets the segments.
		/// </summary>
		/// <value>
		/// The lists.
		/// </value>
		[JsonProperty("segments", NullValueHandling = NullValueHandling.Ignore)]
		public Segment[] Segments { get; set; }

		/// <summary>
		/// Gets or sets the count of exported contacts.
		/// </summary>
		/// <value>
		/// The count.
		/// </value>
		[JsonProperty("contact_count", NullValueHandling = NullValueHandling.Ignore)]
		public long ContactCount { get; set; }
	}
}
