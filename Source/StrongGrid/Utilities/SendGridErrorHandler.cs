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
		/// <summary>Options used to configure the StrongGrid client.</summary>
		public StrongGridClientOptions ClientOptions { get; set; }

		/// <summary>Gets a reference to the diagnostic store.</summary>
		public IDiagnosticStore DiagnosticStore { get; set; }

		public SendGridErrorHandler(IDiagnosticStore diagnosticStore, StrongGridClientOptions clientOptions)
		{
			DiagnosticStore = diagnosticStore ?? throw new ArgumentNullException(nameof(diagnosticStore));
			ClientOptions = clientOptions ?? new StrongGridClientOptions();
		}

		/// <summary>Method invoked just before the HTTP request is submitted. This method can modify the outgoing HTTP request.</summary>
		/// <param name="request">The HTTP request.</param>
		public void OnRequest(IRequest request) { }

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
				diagnosticLog = diagnosticInfo.GetFormattedLog(ClientOptions.LogRequestHeaders, ClientOptions.LogRequestContent, ClientOptions.LogResponseHeaders, ClientOptions.LogResponseContent);
			}

			throw new SendGridException(errorMessage, response.Message, diagnosticLog);
		}
	}
}
