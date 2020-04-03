using Microsoft.Extensions.Logging;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Options for the StrongGrid client.
	/// </summary>
	public class StrongGridClientOptions
	{
		/// <summary>
		/// Gets or sets the log levels for successful calls (HTTP status code in the 200-299 range).
		/// </summary>
		public LogLevel LogLevelSuccessfulCalls { get; set; }

		/// <summary>
		/// Gets or sets the log levels for failed calls (HTTP status code outside of the 200-299 range).
		/// </summary>
		public LogLevel LogLevelFailedCalls { get; set; }
	}
}
