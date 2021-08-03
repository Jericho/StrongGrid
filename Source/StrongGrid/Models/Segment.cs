using System;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Segment.
	/// </summary>
	public class Segment
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
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonPropertyName("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the list identifier.
		/// </summary>
		/// <value>
		/// The list identifier.
		/// </value>
		[JsonPropertyName("parent_list_id")]
		public string ListId { get; set; }

		/// <summary>
		/// Gets or sets the query DSL.
		/// </summary>
		/// <value>
		/// The query DSL.
		/// </value>
		[JsonPropertyName("query_dsl")]
		public string QueryDsl { get; set; }

		/// <summary>
		/// Gets or sets the recipient count.
		/// </summary>
		/// <value>
		/// The recipient count.
		/// </value>
		[JsonPropertyName("contacts_count")]
		public long ContactsCount { get; set; }

		/// <summary>
		/// Gets or sets the created on.
		/// </summary>
		/// <value>
		/// The created on.
		/// </value>
		[JsonPropertyName("created_at")]
		public DateTime CreatedOn { get; set; }

		/// <summary>
		/// Gets or sets the updated on.
		/// </summary>
		/// <value>
		/// The updated on.
		/// </value>
		[JsonPropertyName("updated_at")]
		public DateTime UpdatedOn { get; set; }

		/// <summary>
		/// Gets or sets the date the sample data was updated on.
		/// </summary>
		/// <remarks>
		/// There is delay between when you create or update a segment and when the sample data is refreshed.
		/// </remarks>
		/// <value>
		/// The date the sample data was most recently updated.
		/// </value>
		[JsonPropertyName("sample_updated_at")]
		public DateTime? SampleRefreshedOn { get; set; }

		/// <summary>
		/// Gets or sets the sample contacts.
		/// </summary>
		/// <value>
		/// An array of <see cref="Contact">Contacts</see> that match the segmenting criteria.
		/// </value>
		[JsonPropertyName("contacts_sample")]
		public Contact[] SampleContacts { get; set; }
	}
}
