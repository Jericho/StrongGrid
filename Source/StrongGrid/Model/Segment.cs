using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class Segment
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonProperty("id")]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the list identifier.
		/// </summary>
		/// <value>
		/// The list identifier.
		/// </value>
		[JsonProperty("list_id")]
		public long ListId { get; set; }

		/// <summary>
		/// Gets or sets the conditions.
		/// </summary>
		/// <value>
		/// The conditions.
		/// </value>
		[JsonProperty("conditions")]
		public SearchCondition[] Conditions { get; set; }
	}
}
