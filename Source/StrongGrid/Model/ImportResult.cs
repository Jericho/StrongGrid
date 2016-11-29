using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// The result of importing a group of contacts
	/// </summary>
	public class ImportResult
	{
		/// <summary>
		/// Gets or sets the error count.
		/// </summary>
		/// <value>
		/// The error count.
		/// </value>
		[JsonProperty("error_count", NullValueHandling = NullValueHandling.Ignore)]
		public int ErrorCount { get; set; }

		/// <summary>
		/// Gets or sets the error indices.
		/// </summary>
		/// <value>
		/// The error indices.
		/// </value>
		[JsonProperty("error_indices", NullValueHandling = NullValueHandling.Ignore)]
		public int[] ErrorIndices { get; set; }

		/// <summary>
		/// Gets or sets the new count.
		/// </summary>
		/// <value>
		/// The new count.
		/// </value>
		[JsonProperty("new_count", NullValueHandling = NullValueHandling.Ignore)]
		public int NewCount { get; set; }

		/// <summary>
		/// Gets or sets the persisted recipients.
		/// </summary>
		/// <value>
		/// The persisted recipients.
		/// </value>
		[JsonProperty("persisted_recipients", NullValueHandling = NullValueHandling.Ignore)]
		public string[] PersistedRecipients { get; set; }

		/// <summary>
		/// Gets or sets the updated count.
		/// </summary>
		/// <value>
		/// The updated count.
		/// </value>
		[JsonProperty("updated_count", NullValueHandling = NullValueHandling.Ignore)]
		public int UpdatedCount { get; set; }

		/// <summary>
		/// Gets or sets the errors.
		/// </summary>
		/// <value>
		/// The errors.
		/// </value>
		[JsonProperty("errors", NullValueHandling = NullValueHandling.Ignore)]
		public Error[] Errors { get; set; }
	}
}
