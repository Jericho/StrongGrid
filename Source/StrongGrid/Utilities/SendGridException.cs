using System;
using System.Net;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Exception that includes both the formatted message and the status code.
	/// </summary>
	public class SendGridException : Exception
	{
		/// <summary>
		/// The status code of the non-successful call.
		/// </summary>
		public HttpStatusCode StatusCode { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SendGridException"/> class.
		/// </summary>
		/// <param name="message">The exception message.</param>
		/// <param name="statusCode">The status code of the non-successful call.</param>
		public SendGridException(string message, HttpStatusCode statusCode)
			: base(message)
		{
			StatusCode = statusCode;
		}
	}
}
