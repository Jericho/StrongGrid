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

		/// <summary>
		/// Gets a reference to the diagnostic store.
		/// </summary>
		public IDiagnosticStore DiagnosticStore { get; }

		/// <summary>Method invoked just before the HTTP request is submitted. This method can modify the outgoing HTTP request.</summary>
		/// <param name="request">The HTTP request.</param>
		public void OnRequest(IRequest request) { }

		public SendGridErrorHandler(IDiagnosticStore diagnosticStore, bool logRequestHeaders, bool logRequestContent, bool logResponseHeaders, bool logResponseContent)
		{
			DiagnosticStore = diagnosticStore ?? throw new ArgumentNullException(nameof(diagnosticStore));
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

			var diagnosticLog = "Diagnostic log unavailable";
			var diagnosticId = response.Message.Headers.GetValue(DiagnosticHandler.DIAGNOSTIC_ID_HEADER_NAME);
			if (string.IsNullOrEmpty(diagnosticId)) diagnosticId = response.Message.RequestMessage.Headers.GetValue(DiagnosticHandler.DIAGNOSTIC_ID_HEADER_NAME);
			if (!string.IsNullOrEmpty(diagnosticId) && DiagnosticStore.TryGetValue(diagnosticId, out var diagnosticInfo))
			{
				diagnosticLog = diagnosticInfo.GetFormattedLog(_logRequestHeaders, _logRequestContent, _logResponseHeaders, _logResponseContent);
			}

			throw new SendGridException(errorMessage, response.Message, diagnosticLog);
		}
	}
}
