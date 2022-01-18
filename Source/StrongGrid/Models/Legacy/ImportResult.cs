using System.Text.Json.Serialization;

namespace StrongGrid.Models.Legacy
{
	/// <summary>
	/// The result of importing a group of contacts.
	/// </summary>
	public class ImportResult
	{
		/// <summary>
		/// Gets or sets the error count.
		/// </summary>
		/// <value>
		/// The error count.
		/// </value>
		[JsonPropertyName("error_count")]
		public int ErrorCount { get; set; }

		/// <summary>
		/// Gets or sets the error indices.
		/// </summary>
		/// <value>
		/// The error indices.
		/// </value>
		[JsonPropertyName("error_indices")]
		public int[] ErrorIndices { get; set; }

		/// <summary>
		/// Gets or sets the new count.
		/// </summary>
		/// <value>
		/// The new count.
		/// </value>
		[JsonPropertyName("new_count")]
		public int NewCount { get; set; }

		/// <summary>
		/// Gets or sets the persisted recipients.
		/// </summary>
		/// <value>
		/// The persisted recipients.
		/// </value>
		[JsonPropertyName("persisted_recipients")]
		public string[] PersistedRecipients { get; set; }

		/// <summary>
		/// Gets or sets the updated count.
		/// </summary>
		/// <value>
		/// The updated count.
		/// </value>
		[JsonPropertyName("updated_count")]
		public int UpdatedCount { get; set; }

		/// <summary>
		/// Gets or sets the errors.
		/// </summary>
		/// <value>
		/// The errors.
		/// </value>
		[JsonPropertyName("errors")]
		public Error[] Errors { get; set; }
	}
}
