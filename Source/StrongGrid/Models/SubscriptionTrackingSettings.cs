using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Subscription tracking settings.
	/// </summary>
	public class SubscriptionTrackingSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SubscriptionTrackingSettings"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("enable")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>
		/// The text.
		/// </value>
		[JsonPropertyName("text")]
		public string Text { get; set; }

		/// <summary>
		/// Gets or sets the HTML.
		/// </summary>
		/// <value>
		/// The HTML.
		/// </value>
		[JsonPropertyName("html")]
		public string Html { get; set; }

		/// <summary>
		/// Gets or sets the substitution tag.
		/// </summary>
		/// <value>
		/// The substitution tag.
		/// </value>
		[JsonPropertyName("substitution_tag")]
		public string SubstitutionTag { get; set; }
	}
}
