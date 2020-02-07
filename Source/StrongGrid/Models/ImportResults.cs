using Newtonsoft.Json;

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
		[JsonProperty("requested_count", NullValueHandling = NullValueHandling.Ignore)]
		public long RequestedCount { get; set; }

		/// <summary>
		/// Gets or sets the created count.
		/// </summary>
		/// <value>
		/// The created count.
		/// </value>
		[JsonProperty("created_count", NullValueHandling = NullValueHandling.Ignore)]
		public long CreatedCount { get; set; }

		/// <summary>
		/// Gets or sets the updated count.
		/// </summary>
		/// <value>
		/// The updated count.
		/// </value>
		[JsonProperty("updated_count", NullValueHandling = NullValueHandling.Ignore)]
		public long UpdatedCount { get; set; }

		/// <summary>
		/// Gets or sets the deleted count.
		/// </summary>
		/// <value>
		/// The deleted count.
		/// </value>
		[JsonProperty("deleted_count", NullValueHandling = NullValueHandling.Ignore)]
		public long DeletedCount { get; set; }

		/// <summary>
		/// Gets or sets the error count.
		/// </summary>
		/// <value>
		/// The error count.
		/// </value>
		[JsonProperty("errored_count", NullValueHandling = NullValueHandling.Ignore)]
		public long ErroredCount { get; set; }

		/// <summary>
		/// Gets or sets the url to download the file which provides information about the errors.
		/// </summary>
		/// <value>
		/// The error file URL.
		/// </value>
		[JsonProperty("errors_url", NullValueHandling = NullValueHandling.Ignore)]
		public string ErrorsUrl { get; set; }
	}
}
