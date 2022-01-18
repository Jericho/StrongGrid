using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// The result of importing a group of contacts.
	/// </summary>
	public class ImportResults
	{
		/// <summary>
		/// Gets or sets the requested count.
		/// </summary>
		/// <value>
		/// The requested count.
		/// </value>
		[JsonPropertyName("requested_count")]
		public long RequestedCount { get; set; }

		/// <summary>
		/// Gets or sets the created count.
		/// </summary>
		/// <value>
		/// The created count.
		/// </value>
		[JsonPropertyName("created_count")]
		public long CreatedCount { get; set; }

		/// <summary>
		/// Gets or sets the updated count.
		/// </summary>
		/// <value>
		/// The updated count.
		/// </value>
		[JsonPropertyName("updated_count")]
		public long UpdatedCount { get; set; }

		/// <summary>
		/// Gets or sets the deleted count.
		/// </summary>
		/// <value>
		/// The deleted count.
		/// </value>
		[JsonPropertyName("deleted_count")]
		public long DeletedCount { get; set; }

		/// <summary>
		/// Gets or sets the error count.
		/// </summary>
		/// <value>
		/// The error count.
		/// </value>
		[JsonPropertyName("errored_count")]
		public long ErroredCount { get; set; }

		/// <summary>
		/// Gets or sets the url to download the file which provides information about the errors.
		/// </summary>
		/// <value>
		/// The error file URL.
		/// </value>
		[JsonPropertyName("errors_url")]
		public string ErrorsUrl { get; set; }
	}
}
