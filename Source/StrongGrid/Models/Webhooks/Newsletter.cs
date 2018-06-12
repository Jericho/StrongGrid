using Newtonsoft.Json;

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
		[JsonProperty("newsletter_user_list_id", NullValueHandling = NullValueHandling.Ignore)]
		public string UserListId { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonProperty("newsletter_id", NullValueHandling = NullValueHandling.Ignore)]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the send identifier.
		/// </summary>
		/// <value>
		/// The send identifier.
		/// </value>
		[JsonProperty("newsletter_send_id", NullValueHandling = NullValueHandling.Ignore)]
		public string SendId { get; set; }
	}
}
