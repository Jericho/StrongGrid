using StrongGrid.Json;
using System;
using System.Text.Json.Serialization;

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
		[JsonPropertyName("id")]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		[JsonPropertyName("status")]
		[JsonConverter(typeof(StringEnumConverter<ExportJobStatus>))]
		public ExportJobStatus Status { get; set; }

		/// <summary>
		/// Gets or sets the date when the job was created.
		/// </summary>
		/// <value>
		/// The creation date.
		/// </value>
		[JsonPropertyName("created_at")]
		public DateTime CreatedOn { get; set; }

		/// <summary>
		/// Gets or sets the date when the job completed.
		/// </summary>
		/// <value>
		/// The completion date.
		/// </value>
		[JsonPropertyName("completed_at")]
		public DateTime CompletedOn { get; set; }

		/// <summary>
		/// Gets or sets the date when the job expires.
		/// </summary>
		/// <value>
		/// The expiration date.
		/// </value>
		[JsonPropertyName("expires_at")]
		public DateTime ExpiresOn { get; set; }

		/// <summary>
		/// Gets or sets the URLs of the files.
		/// </summary>
		/// <value>
		/// The URLs.
		/// </value>
		[JsonPropertyName("urls")]
		public string[] FileUrls { get; set; }

		/// <summary>
		/// Gets or sets a human readable message if the status is 'failure'.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		[JsonPropertyName("message")]
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the user id.
		/// </summary>
		/// <value>
		/// The user identifier.
		/// </value>
		[JsonPropertyName("user_id")]
		public long UserId { get; set; }

		/// <summary>
		/// Gets or sets the export type.
		/// </summary>
		/// <value>
		/// The export type.
		/// </value>
		[JsonPropertyName("export_type")]
		public ExportType ExportType { get; set; }

		/// <summary>
		/// Gets or sets the lists.
		/// </summary>
		/// <value>
		/// The lists.
		/// </value>
		[JsonPropertyName("lists")]
		public List[] Lists { get; set; }

		/// <summary>
		/// Gets or sets the segments.
		/// </summary>
		/// <value>
		/// The lists.
		/// </value>
		[JsonPropertyName("segments")]
		public Segment[] Segments { get; set; }

		/// <summary>
		/// Gets or sets the count of exported contacts.
		/// </summary>
		/// <value>
		/// The count.
		/// </value>
		[JsonPropertyName("contact_count")]
		public long ContactCount { get; set; }
	}
}
