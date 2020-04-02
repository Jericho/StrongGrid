using Newtonsoft.Json;
using System;

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
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the list identifier.
		/// </summary>
		/// <value>
		/// The list identifier.
		/// </value>
		[JsonProperty("parent_list_id", NullValueHandling = NullValueHandling.Ignore)]
		public string ListId { get; set; }

		/// <summary>
		/// Gets or sets the query DSL.
		/// </summary>
		/// <value>
		/// The query DSL.
		/// </value>
		[JsonProperty("query_dsl", NullValueHandling = NullValueHandling.Ignore)]
		public string QueryDsl { get; set; }

		/// <summary>
		/// Gets or sets the recipient count.
		/// </summary>
		/// <value>
		/// The recipient count.
		/// </value>
		[JsonProperty("contacts_count", NullValueHandling = NullValueHandling.Ignore)]
		public long ContactsCount { get; set; }

		/// <summary>
		/// Gets or sets the created on.
		/// </summary>
		/// <value>
		/// The created on.
		/// </value>
		[JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime CreatedOn { get; set; }

		/// <summary>
		/// Gets or sets the updated on.
		/// </summary>
		/// <value>
		/// The updated on.
		/// </value>
		[JsonProperty("updated_at", NullValueHandling = NullValueHandling.Ignore)]
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
		[JsonProperty("sample_updated_at", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime SampleRefreshedOn { get; set; }

		/// <summary>
		/// Gets or sets the sample contacts.
		/// </summary>
		/// <value>
		/// An array of <see cref="Contact">Contacts</see> that match the segmenting criteria.
		/// </value>
		[JsonProperty("contacts_sample", NullValueHandling = NullValueHandling.Ignore)]
		public Contact[] SampleContacts { get; set; }
	}
}
