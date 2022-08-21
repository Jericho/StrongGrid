using System;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// An import job allows you to import multiple contacts from a file (either csv or json).
	/// A given export job can produce multiple files if the amount of data exceeds the maximum MB per file.
	/// </summary>
	public class ImportJob
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
		public ImportJobStatus Status { get; set; }

		/// <summary>
		/// Gets or sets the job type.
		/// </summary>
		/// <value>
		/// The job type.
		/// </value>
		[JsonPropertyName("job_type")]
		public ImportType Type { get; set; }

		/// <summary>
		/// Gets or sets the information about the number of records created, deleted, updated, etc.
		/// </summary>
		/// <value>
		/// The results.
		/// </value>
		[JsonPropertyName("results")]
		public ImportResults Results { get; set; }

		/// <summary>
		/// Gets or sets the date when the import was started.
		/// </summary>
		/// <value>
		/// The creation date.
		/// </value>
		[JsonPropertyName("started_at")]
		public DateTime StartedOn { get; set; }

		/// <summary>
		/// Gets or sets the date when the import completed.
		/// </summary>
		/// <value>
		/// The completion date.
		/// </value>
		[JsonPropertyName("finished_at")]
		public DateTime CompletedOn { get; set; }
	}
}
