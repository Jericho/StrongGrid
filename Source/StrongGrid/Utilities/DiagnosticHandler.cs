using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;
using System;
using System.Diagnostics;
using System.Net.Http;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Diagnostic handler for requests dispatched to the SendGrid API.
	/// </summary>
	/// <seealso cref="IHttpFilter" />
	internal class DiagnosticHandler : IHttpFilter
	{
		#region FIELDS

		internal const string DIAGNOSTIC_ID_HEADER_NAME = "StrongGrid-Diagnostic-Id";
		private readonly ILogger _logger;
		private readonly IDiagnosticStore _diagnosticStore;
		private readonly StrongGridClientOptions _clientOptions;

		#endregion

		#region CTOR

		public DiagnosticHandler(IDiagnosticStore diagnosticStore, StrongGridClientOptions clientOptions = null, ILogger logger = null)
		{
			_diagnosticStore = diagnosticStore ?? throw new ArgumentNullException(nameof(diagnosticStore));
			_clientOptions = clientOptions ?? new StrongGridClientOptions();
			_logger = logger ?? NullLogger.Instance;
		}

		#endregion

		#region PUBLIC METHODS

		/// <summary>Method invoked just before the HTTP request is submitted. This method can modify the outgoing HTTP request.</summary>
		/// <param name="request">The HTTP request.</param>
		public void OnRequest(IRequest request)
		{
			// Add a unique ID to the request header
			var diagnosticId = Guid.NewGuid().ToString("N");
			request.WithHeader(DIAGNOSTIC_ID_HEADER_NAME, diagnosticId);

			// Add the diagnostic info to our cache
			_diagnosticStore.TryAdd(diagnosticId, new DiagnosticInfo(new WeakReference<HttpRequestMessage>(request.Message), Stopwatch.GetTimestamp(), null, long.MinValue, request.Options));
		}

		/// <summary>Method invoked just after the HTTP response is received. This method can modify the incoming HTTP response.</summary>
		/// <param name="response">The HTTP response.</param>
		/// <param name="httpErrorAsException">Whether HTTP error responses should be raised as exceptions.</param>
		public void OnResponse(IResponse response, bool httpErrorAsException)
		{
			var responseTimestamp = Stopwatch.GetTimestamp();

			var diagnosticId = response.Message.RequestMessage.Headers.GetValue(DIAGNOSTIC_ID_HEADER_NAME);
			if (_diagnosticStore.TryGetValue(diagnosticId, out DiagnosticInfo diagnosticInfo))
			{
				// Update the cached diagnostic info
				diagnosticInfo.ResponseReference = new WeakReference<HttpResponseMessage>(response.Message);
				diagnosticInfo.ResponseTimestamp = responseTimestamp;
				_diagnosticStore.AddOrUpdate(diagnosticId, diagnosticInfo);

				// Log
				var logLevel = response.IsSuccessStatusCode ? _clientOptions.LogLevelSuccessfulCalls : _clientOptions.LogLevelFailedCalls;
				if (_logger?.IsEnabled(logLevel) ?? false)
				{
					var template = diagnosticInfo.GetLoggingTemplate(true, _clientOptions.LogRequestHeaders, _clientOptions.LogRequestContent, _clientOptions.LogResponseHeaders, _clientOptions.LogResponseContent);
					var parameters = diagnosticInfo.GetLoggingParameters(_clientOptions.LogRequestHeaders, _clientOptions.LogRequestContent, _clientOptions.LogResponseHeaders, _clientOptions.LogResponseContent);

					// CA2254 warns when the message template varies; allow it here because we need to pass
					// the template to structured logging providers exactly as produced by DiagnosticInfo.
#pragma warning disable CA2254 // Template is intentionally dynamic for structured loggers
					_logger.Log(logLevel, template, parameters);
#pragma warning restore CA2254
				}
			}
		}

		#endregion
	}
}
