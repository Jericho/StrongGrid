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
		private const string SENDGRID_EUROPE_V3_BASE_URI = "https://api.eu.sendgrid.com/v3";

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
		[Obsolete("Use ApiBaseUrl instead.")]
		public Uri ApiEndPoint
		{
			get { return this.ApiBaseUrl; }
			set { this.ApiBaseUrl = value; }
		}

		/// <summary>
		/// Gets or sets the base URI used for API requests.
		/// </summary>
		public Uri ApiBaseUrl { get; set; } = new Uri(SENDGRID_V3_BASE_URI);

		/// <summary>
		/// Configures the client options to use the European Union SendGrid API endpoint.
		/// </summary>
		/// <remarks>Use this method to direct API requests to SendGrid's European Union infrastructure, which may be
		/// required for compliance with regional data regulations.</remarks>
		/// <returns>The same <see cref="StrongGridClientOptions"/> instance with the API endpoint set to the European Union endpoint.</returns>
		public StrongGridClientOptions WithEuropeanUnionApiBaseUrl()
		{
			this.ApiBaseUrl = new Uri(SENDGRID_EUROPE_V3_BASE_URI);
			return this;
		}

	}
}
