using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// Suppression Groups allow you to segment your email by a grouping which is most often defined
	/// by the types of email. Example: Receipts, Deals emails, and notification.
	/// </summary>
	public class SuppressionGroup
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonProperty("id")]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		[JsonProperty("description")]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is default.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is default; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("is_default")]
		public bool IsDefault { get; set; }
	}
}
