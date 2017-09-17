using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// A list of contacts
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
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the recipient count.
		/// </summary>
		/// <value>
		/// The recipient count.
		/// </value>
		[JsonProperty("recipient_count", NullValueHandling = NullValueHandling.Ignore)]
		public long RecipientCount { get; set; }
	}
}
