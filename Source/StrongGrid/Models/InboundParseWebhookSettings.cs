using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Inbound Parse Webhook settings.
	/// </summary>
	public class InboundParseWebhookSettings
	{
		/// <summary>
		/// Gets or sets the hostname of the URL where parsed emails are POSTed.
		/// </summary>
		/// <value>
		/// The hostname.
		/// </value>
		[JsonProperty("hostname", NullValueHandling = NullValueHandling.Ignore)]
		public string HostName { get; set; }

		/// <summary>
		/// Gets or sets the URL where parsed emails are POSTed.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		[JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the parsed content will be checked for spam.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("spam_check", NullValueHandling = NullValueHandling.Ignore)]
		public bool SpamCheck { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the the raw content that was parsed will be posted.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("send_raw", NullValueHandling = NullValueHandling.Ignore)]
		public bool SendRaw { get; set; }
	}
}
