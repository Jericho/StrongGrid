using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Diagnostic handler for requests dispatched to the SendGrid API.
	/// </summary>
	/// <seealso cref="Pathoschild.Http.Client.Extensibility.IHttpFilter" />
	internal class DiagnosticHandler : IHttpFilter
	{
		internal class DiagnosticInfo
		{
			public WeakReference<HttpRequestMessage> RequestReference { get; set; }

			public long RequestTimestamp { get; set; }

			public WeakReference<HttpResponseMessage> ResponseReference { get; set; }

			public long ResponseTimestamp { get; set; }

			public DiagnosticInfo(WeakReference<HttpRequestMessage> requestReference, long requestTimestamp, WeakReference<HttpResponseMessage> responseReference, long responseTimestamp)
			{
				RequestReference = requestReference;
				RequestTimestamp = requestTimestamp;
				ResponseReference = responseReference;
				ResponseTimestamp = responseTimestamp;
			}
		}

		#region FIELDS

		internal const string DIAGNOSTIC_ID_HEADER_NAME = "StrongGrid-Diagnostic-Id";
		private readonly ILogger _logger;
		private readonly LogLevel _logLevelSuccessfulCalls;
		private readonly LogLevel _logLevelFailedCalls;

		#endregion

		#region PROPERTIES

		internal static ConcurrentDictionary<string, DiagnosticInfo> DiagnosticsInfo { get; } = new();

		#endregion

		#region CTOR

		public DiagnosticHandler(LogLevel logLevelSuccessfulCalls, LogLevel logLevelFailedCalls, ILogger logger = null)
		{
			_logLevelSuccessfulCalls = logLevelSuccessfulCalls;
			_logLevelFailedCalls = logLevelFailedCalls;
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
			DiagnosticsInfo.TryAdd(diagnosticId, new DiagnosticInfo(new WeakReference<HttpRequestMessage>(request.Message), Stopwatch.GetTimestamp(), null, long.MinValue));
		}

		/// <summary>Method invoked just after the HTTP response is received. This method can modify the incoming HTTP response.</summary>
		/// <param name="response">The HTTP response.</param>
		/// <param name="httpErrorAsException">Whether HTTP error responses should be raised as exceptions.</param>
		public void OnResponse(IResponse response, bool httpErrorAsException)
		{
			var responseTimestamp = Stopwatch.GetTimestamp();

			var diagnosticId = response.Message.RequestMessage.Headers.GetValue(DIAGNOSTIC_ID_HEADER_NAME);
			if (DiagnosticsInfo.TryGetValue(diagnosticId, out DiagnosticInfo diagnosticInfo))
			{
				if (!diagnosticInfo.RequestReference.TryGetTarget(out HttpRequestMessage request))
				{
					_logger.LogDebug("The HTTP request was garbage-collected before StrongGrid could log debugging information about this request");
				}
				else
				{
					var logLevel = response.IsSuccessStatusCode ? _logLevelSuccessfulCalls : _logLevelFailedCalls;
					if (_logger.IsEnabled(logLevel))
					{
						// Calculate the size of the content
						var requestContentLength = request.Content?.Headers?.ContentLength.GetValueOrDefault(0) ?? 0;
						var responseContentLength = response.Message?.Content?.Headers?.ContentLength.GetValueOrDefault(0) ?? 0;

						// Scrub sensitive information from headers
						var safeRequestHeaders = request.Headers?.Where(kvp => !kvp.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase)).Select(kvp => new KeyValuePair<string, string>(kvp.Key, string.Join(", ", kvp.Value)))
							.Union(request.Headers?.Where(kvp => kvp.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase)).Select(kvp => new KeyValuePair<string, string>(kvp.Key, "...redacted for security reasons...")))
							.ToList();

						var safeResponseHeaders = response.Message?.Headers?.Where(kvp => !kvp.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase)).Select(kvp => new KeyValuePair<string, string>(kvp.Key, string.Join(", ", kvp.Value)))
							.Union(response.Message?.Headers?.Where(kvp => kvp.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase)).Select(kvp => new KeyValuePair<string, string>(kvp.Key, "...redacted for security reasons...")))
							.ToList();

						// Make sure the Content-Length header is present
						if (!safeRequestHeaders.Any(kvp => kvp.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase)))
						{
							safeRequestHeaders.Add(new KeyValuePair<string, string>("Content-Length", requestContentLength.ToString()));
						}

						if (!safeResponseHeaders.Any(kvp => kvp.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase)))
						{
							safeResponseHeaders.Add(new KeyValuePair<string, string>("Content-Length", responseContentLength.ToString()));
						}

						// Log the request and response
						using (_logger.BeginScope("{RequestHeaders}", safeRequestHeaders))
						{
							var requestContent = ((requestContentLength > 0 ? request.Content.ReadAsStringAsync(null).GetAwaiter().GetResult() : null) ?? "<NULL>").TrimEnd('\r', '\n'); ;
							_logger.Log(logLevel, "REQUEST SENT BY STRONGGRID: {HttpMethod} {RequestUri} HTTP/{HttpVersion}\r\nCONTENT OF THE REQUEST: {Content}", request.Method.Method, request.RequestUri, request.Version, requestContent);

							using (_logger.BeginScope("{ResponseHeaders}", safeResponseHeaders))
							{
								var elapsed = TimeSpan.FromTicks(responseTimestamp - diagnosticInfo.RequestTimestamp);
								var responseContent = ((responseContentLength > 0 ? response.Message.Content.ReadAsStringAsync(null).GetAwaiter().GetResult() : null) ?? "<NULL>").TrimEnd('\r', '\n');
								_logger.Log(logLevel, "RESPONSE FROM SENDGRID: HTTP/{HttpVersion} {StatusCode} {ReasonPhrase}\r\nCONTENT OF THE RESPONSE: {Content}\r\nDIAGNOSTIC: The request took {Elapsed}", response.Message.Version, (int)response.Message.StatusCode, response.Message.ReasonPhrase, responseContent, elapsed.ToDurationString());
							}
						}

						// Update the cached diagnostic info
						DiagnosticsInfo.TryUpdate(
							diagnosticId,
							new DiagnosticInfo(diagnosticInfo.RequestReference, diagnosticInfo.RequestTimestamp, new WeakReference<HttpResponseMessage>(response.Message), responseTimestamp),
							diagnosticInfo);
					}
				}

				Cleanup();
			}
		}

		#endregion

		#region PRIVATE METHODS

		private static void Cleanup()
		{
			try
			{
				// Remove diagnostic information for requests that have been garbage collected
				foreach (string key in DiagnosticHandler.DiagnosticsInfo.Keys.ToArray())
				{
					if (DiagnosticHandler.DiagnosticsInfo.TryGetValue(key, out DiagnosticInfo diagnosticInfo))
					{
						if (!diagnosticInfo.RequestReference.TryGetTarget(out HttpRequestMessage request))
						{
							DiagnosticsInfo.TryRemove(key, out _);
						}
					}
				}
			}
			catch
			{
				// Intentionally left empty
			}
		}

		#endregion
	}
}
