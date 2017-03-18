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
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		[JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is default.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is default; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("is_default", NullValueHandling = NullValueHandling.Ignore)]
		public bool IsDefault { get; set; }

		/// <summary>
		/// Gets or sets a value indicating the number of email addresses in this suppression group.
		/// </summary>
		/// <value>
		/// The number of email addresses in this suppression group.
		/// </value>
		[JsonProperty("unsubscribes", NullValueHandling = NullValueHandling.Ignore)]
		public long UnsubscribesCount { get; set; }
	}
}
