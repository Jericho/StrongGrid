using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// A list of contacts.
	/// </summary>
	public class List
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonPropertyName("id")]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonPropertyName("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the contact count.
		/// </summary>
		/// <value>
		/// The contact count.
		/// </value>
		[JsonPropertyName("contact_count")]
		public long ContactCount { get; set; }

		/// <summary>
		/// Gets or sets the sample contacts.
		/// </summary>
		/// <value>
		/// An array of <see cref="Contact">Contacts</see>.
		/// </value>
		[JsonPropertyName("contact_sample")]
		public Contact[] SampleContacts { get; set; }
	}
}
