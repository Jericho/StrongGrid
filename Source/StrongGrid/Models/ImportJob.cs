using Newtonsoft.Json;
using System;

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
		/// Gets or sets the job type.
		/// </summary>
		/// <value>
		/// The job type.
		/// </value>
		[JsonProperty("job_type", NullValueHandling = NullValueHandling.Ignore)]
		public ImportType Type { get; set; }

		/// <summary>
		/// Gets or sets the information about the nu,ber of records created, deleted, updated, etc.
		/// </summary>
		/// <value>
		/// The results.
		/// </value>
		[JsonProperty("results", NullValueHandling = NullValueHandling.Ignore)]
		public ImportResults Results { get; set; }

		/// <summary>
		/// Gets or sets the date when the import was started.
		/// </summary>
		/// <value>
		/// The creation date.
		/// </value>
		[JsonProperty("started_at", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime StartedOn { get; set; }

		/// <summary>
		/// Gets or sets the date when the import completed.
		/// </summary>
		/// <value>
		/// The completion date.
		/// </value>
		[JsonProperty("finished_at", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime CompletedOn { get; set; }
	}
}
