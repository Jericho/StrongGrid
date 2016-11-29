using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// Segment
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
		/// Gets or sets the list identifier.
		/// </summary>
		/// <value>
		/// The list identifier.
		/// </value>
		[JsonProperty("list_id", NullValueHandling = NullValueHandling.Ignore)]
		public long ListId { get; set; }

		/// <summary>
		/// Gets or sets the conditions.
		/// </summary>
		/// <value>
		/// The conditions.
		/// </value>
		[JsonProperty("conditions", NullValueHandling = NullValueHandling.Ignore)]
		public SearchCondition[] Conditions { get; set; }
	}
}
