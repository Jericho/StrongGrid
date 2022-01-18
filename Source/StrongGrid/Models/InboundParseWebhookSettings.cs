using System.Text.Json.Serialization;

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
		[JsonPropertyName("hostname")]
		public string HostName { get; set; }

		/// <summary>
		/// Gets or sets the URL where parsed emails are POSTed.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		[JsonPropertyName("url")]
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the parsed content will be checked for spam.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("spam_check")]
		public bool SpamCheck { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the the raw content that was parsed will be posted.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("send_raw")]
		public bool SendRaw { get; set; }
	}
}
