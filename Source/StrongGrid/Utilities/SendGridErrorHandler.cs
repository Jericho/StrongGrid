using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;
using System;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Error handler for requests dispatched to the SendGrid API.
	/// </summary>
	/// <seealso cref="Pathoschild.Http.Client.Extensibility.IHttpFilter" />
	internal class SendGridErrorHandler : IHttpFilter
	{
		private readonly bool _logRequestHeaders;
		private readonly bool _logRequestContent;
		private readonly bool _logResponseHeaders;
		private readonly bool _logResponseContent;

		/// <summary>Method invoked just before the HTTP request is submitted. This method can modify the outgoing HTTP request.</summary>
		/// <param name="request">The HTTP request.</param>
		public void OnRequest(IRequest request) { }

		public SendGridErrorHandler(bool logRequestHeaders, bool logRequestContent, bool logResponseHeaders, bool logResponseContent)
		{
			_logRequestHeaders = logRequestHeaders;
			_logRequestContent = logRequestContent;
			_logResponseHeaders = logResponseHeaders;
			_logResponseContent = logResponseContent;
		}

		/// <summary>Method invoked just after the HTTP response is received. This method can modify the incoming HTTP response.</summary>
		/// <param name="response">The HTTP response.</param>
		/// <param name="httpErrorAsException">Whether HTTP error responses should be raised as exceptions.</param>
		public void OnResponse(IResponse response, bool httpErrorAsException)
		{
			var (isError, errorMessage) = response.Message.GetErrorMessageAsync().GetAwaiter().GetResult();
			if (!isError) return;

			var diagnosticLog = response.GetDiagnosticInfo()?.GetFormattedLog(_logRequestHeaders, _logRequestContent, _logResponseHeaders, _logResponseContent) ?? "Diagnostic log unavailable";
			throw new SendGridException(errorMessage, response.Message, diagnosticLog);
		}
	}
}
