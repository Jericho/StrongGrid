using System;
using System.Net;
using System.Net.Http;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Exception that includes both the formatted message and the status code.
	/// </summary>
	public class SendGridException : Exception
	{
		/// <summary>
		/// Gets the status code of the non-successful call.
		/// </summary>
		public HttpStatusCode StatusCode => ResponseMessage.StatusCode;

		/// <summary>
		/// Gets the HTTP response message which caused the exception.
		/// </summary>
		public HttpResponseMessage ResponseMessage { get; }

		/// <summary>
		/// Gets the human readable representation of the request/response.
		/// </summary>
		public string RequestResponseLog { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SendGridException"/> class.
		/// </summary>
		/// <param name="message">The exception message.</param>
		/// <param name="responseMessage">The response message of the non-successful call.</param>
		/// <param name="requestResponseLog">The human readable representation of the request/response.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
		public SendGridException(string message, HttpResponseMessage responseMessage, string requestResponseLog, Exception innerException = null)
			: base(message, innerException)
		{
			ResponseMessage = responseMessage;
			RequestResponseLog = requestResponseLog;
		}
	}
}
