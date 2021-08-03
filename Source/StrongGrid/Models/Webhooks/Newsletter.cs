using System.Text.Json.Serialization;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// A newsletter.
	/// </summary>
	public class Newsletter
	{
		/// <summary>
		/// Gets or sets the user list identifier.
		/// </summary>
		/// <value>
		/// The user list identifier.
		/// </value>
		[JsonPropertyName("newsletter_user_list_id")]
		public string UserListId { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonPropertyName("newsletter_id")]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the send identifier.
		/// </summary>
		/// <value>
		/// The send identifier.
		/// </value>
		[JsonPropertyName("newsletter_send_id")]
		public string SendId { get; set; }
	}
}
