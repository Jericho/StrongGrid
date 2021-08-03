using System.Text.Json.Serialization;

namespace StrongGrid.Models
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
		/// Gets or sets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		[JsonPropertyName("description")]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is default.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is default; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("is_default")]
		public bool IsDefault { get; set; }

		/// <summary>
		/// Gets or sets a value indicating the number of email addresses in this suppression group.
		/// </summary>
		/// <value>
		/// The number of email addresses in this suppression group.
		/// </value>
		[JsonPropertyName("unsubscribes")]
		public long UnsubscribesCount { get; set; }
	}
}
