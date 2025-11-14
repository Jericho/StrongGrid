using Microsoft.Extensions.Logging;
using System;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Options for the StrongGrid client.
	/// </summary>
	public class StrongGridClientOptions
	{
		private const string SENDGRID_V3_BASE_URI = "https://api.sendgrid.com/v3";

		/// <summary>
		/// Gets or sets the log levels for successful calls (HTTP status code in the 200-299 range).
		/// </summary>
		public LogLevel LogLevelSuccessfulCalls { get; set; } = LogLevel.Debug;

		/// <summary>
		/// Gets or sets the log levels for failed calls (HTTP status code outside of the 200-299 range).
		/// </summary>
		public LogLevel LogLevelFailedCalls { get; set; } = LogLevel.Error;

		/// <summary>
		/// Gets or sets the base URI of the SendGrid API endpoint.
		/// </summary>
		public Uri ApiEndPoint { get; set; } = new Uri(SENDGRID_V3_BASE_URI);
	}
}
