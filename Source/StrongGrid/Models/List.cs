using Newtonsoft.Json;

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
		/// Gets or sets the contact count.
		/// </summary>
		/// <value>
		/// The contact count.
		/// </value>
		[JsonProperty("contact_count", NullValueHandling = NullValueHandling.Ignore)]
		public long ContactCount { get; set; }
	}
}
