using System;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Engagement quality score.
	/// </summary>
	public class EngagementQualityScore
	{
		/// <summary>
		/// Gets or sets the date (UTC) for which this score was calculated.
		/// </summary>
		[JsonPropertyName("date")]
		public DateTime Date { get; set; }

		/// <summary>
		/// Gets or sets the user identifier associated with this score.
		/// </summary>
		[JsonPropertyName("user_id")]
		public string UserId { get; set; } // user_id is always returned as a long in the SendGrid API except in this instance where it's a string

		/// <summary>
		/// Gets or sets the user name associated with this score.
		/// </summary>
		[JsonPropertyName("username")]
		public string UserName { get; set; }

		/// <summary>
		/// Gets or sets the SendGrid Engagment Quality Score.
		/// </summary>
		/// <remarks>
		/// maximum: 5; minimum: 1.
		/// </remarks>
		[JsonPropertyName("score")]
		public int? Score { get; set; }

		/// <summary>
		/// Gets or sets a collection of metrics representing your engagment score across multiple indicators such as your open rate, spam rate, and bounce rate.
		/// </summary>
		/// <remarks>
		/// Each metric is assigned a score from 1 to 5 with a higher score representing better engagement quality for that particular metric.
		/// </remarks>
		[JsonPropertyName("metrics")]
		public EngagementQualityMetric[] Metrics { get; set; }
	}
}
