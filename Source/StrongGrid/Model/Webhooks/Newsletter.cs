using Newtonsoft.Json;
using StrongGrid.Utilities;

namespace StrongGrid.Model.Webhooks
{
	/// <summary>
	/// A newsletter
	/// </summary>
	public class Newsletter
	{
		[JsonProperty("newsletter_user_list_id", NullValueHandling = NullValueHandling.Ignore)]
		public string UserListId { get; set; }

		[JsonProperty("newsletter_id", NullValueHandling = NullValueHandling.Ignore)]
		public string Id { get; set; }

		[JsonProperty("newsletter_send_id", NullValueHandling = NullValueHandling.Ignore)]
		public string SendId { get; set; }
	}
}
