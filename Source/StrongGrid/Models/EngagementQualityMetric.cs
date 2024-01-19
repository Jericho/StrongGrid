using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Engagement quality metric.
	/// </summary>
	public class EngagementQualityMetric
	{
		/// <summary>
		/// Gets or sets a value indicating whether or not you are sending to an engaged audience.
		/// </summary>
		/// <remarks>
		///  Engagment recency is determined by factors such as how regularly your messages are being opened and clicked.
		///  Your score will range from 1 to 5 with a higher score representing better engagement quality.
		///  </remarks>
		[JsonPropertyName("engagement_recency")]
		public int Recency { get; set; }

		/// <summary>
		/// Gets or sets a value indicating the degree to which your audience is opening your messages.
		/// </summary>
		/// <remarks>
		/// Your score will range from 1 to 5 with a higher score representing a better open rate and better engagement quality.
		/// </remarks>
		[JsonPropertyName("open_rate")]
		public int OpenRate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating the degree to which mailbox providers are rejecting your mail due to reputation issues or content that looks like spam.
		/// </summary>
		/// <remarks>
		/// Your score is calculated based on a ratio of these specific types of bounces to your total processed email.
		/// Your score will range from 1 to 5 with a higher score representing a lower percentage of bounces and better engagement quality.
		/// </remarks>
		[JsonPropertyName("bounce_classification")]
		public int BounceClassification { get; set; }

		/// <summary>
		/// Gets or sets a value indicating if you are attempting to send too many messages to addresses that don't exist.
		/// </summary>
		/// <remarks>
		/// This score is determined by calculating your permanent bounces (bounces to invalid mailboxes).
		/// Your score will range from 1 to 5 with a higher score representing fewer bounces and better engagement quality.
		/// </remarks>
		[JsonPropertyName("bounce_rate")]
		public int BounceRate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating if your messages are generating excessive spam complaints from recipients.
		/// </summary>
		/// <remarks>
		/// This score is determined by calculating the number of recipients who open your mail and then mark it as spam.
		/// Your score will range from 1 to 5 with a higher score representing fewer spam reports and better engagement quality.
		/// </remarks>
		[JsonPropertyName("spam_rate")]
		public int SpamRate { get; set; }
	}
}
