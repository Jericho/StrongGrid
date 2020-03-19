using Newtonsoft.Json;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Recipient has opened the HTML message.
	/// You need to enable Open Tracking for getting this type of event.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.Webhooks.EngagementEvent" />
	public class OpenEvent : EngagementEvent
	{
		/// <summary>
		/// Gets or sets the user agent.
		/// </summary>
		/// <value>
		/// The user agent.
		/// </value>
		[JsonProperty("useragent", NullValueHandling = NullValueHandling.Ignore)]
		public string UserAgent { get; set; }

		/// <summary>
		/// Gets or sets the newsletter.
		/// </summary>
		/// <value>
		/// The newsletter.
		/// </value>
		[JsonProperty("newsletter", NullValueHandling = NullValueHandling.Ignore)]
		public Newsletter Newsletter { get; set; }

		/// <summary>
		/// Gets or sets the content type.
		/// </summary>
		/// <remarks>
		/// Possible values: "html" or "amp".
		/// </remarks>
		/// <value>
		/// The content type.
		/// </value>
		[JsonProperty("sg_content_type", NullValueHandling = NullValueHandling.Ignore)]
		public string ContentType { get; set; }
	}
}
