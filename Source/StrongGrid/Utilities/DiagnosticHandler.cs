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
using System.Text;
using System.Text.RegularExpressions;

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

			public string GetLoggingTemplate()
			{
				RequestReference.TryGetTarget(out HttpRequestMessage request);
				ResponseReference.TryGetTarget(out HttpResponseMessage response);

				var logTemplate = new StringBuilder();

				if (request != null)
				{
					logTemplate.AppendLine("REQUEST SENT BY STRONGGRID: {Request_HttpMethod} {Request_Uri} HTTP/{Request_HttpVersion}");
					logTemplate.AppendLine("REQUEST HEADERS:");
					request.Headers
						.Where(kvp => !kvp.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase))
						.OrderBy(kvp => kvp.Key)
						.Select(kvp => kvp.Key)
						.ToList()
						.ForEach(key => logTemplate.AppendLine("  " + key + ": {Request_Header_" + key + "}"));
					logTemplate.AppendLine("REQUEST: {Request_Content}");
					logTemplate.AppendLine();
				}

				if (response != null)
				{
					logTemplate.AppendLine("RESPONSE FROM SENDGRID: HTTP/{Response_HttpVersion} {Response_StatusCode} {Response_ReasonPhrase}");
					logTemplate.AppendLine("RESPONSE HEADERS:");
					response.Headers
						.Where(kvp => !kvp.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase))
						.OrderBy(kvp => kvp.Key)
						.Select(kvp => kvp.Key)
						.ToList()
						.ForEach(key => logTemplate.AppendLine("  " + key + ": {Response_Header_" + key + "}"));
					logTemplate.AppendLine("RESPONSE: {Response_Content}");
					logTemplate.AppendLine();
				}

				logTemplate.AppendLine("DIAGNOSTIC: The request took {Diagnostic_Elapsed:N} milliseconds");

				return logTemplate.ToString();
			}

			public object[] GetLoggingParameters()
			{
				RequestReference.TryGetTarget(out HttpRequestMessage request);
				ResponseReference.TryGetTarget(out HttpResponseMessage response);

				// Calculate the size of the content
				var requestContentLength = request?.Content?.Headers?.ContentLength.GetValueOrDefault(0) ?? 0;
				var responseContentLength = response?.Content?.Headers?.ContentLength.GetValueOrDefault(0) ?? 0;

				// Get the content to the request/response and calculate how long it took to get the response
				var elapsed = TimeSpan.FromTicks(ResponseTimestamp - RequestTimestamp);
				var requestContent = (request?.Content?.ReadAsStringAsync(null).GetAwaiter().GetResult() ?? "<NULL>").TrimEnd('\r', '\n');
				var responseContent = (response?.Content?.ReadAsStringAsync(null).GetAwaiter().GetResult() ?? "<NULL>").TrimEnd('\r', '\n');

				// The order of these values must match the order in which they appear in the logging template
				var logParams = new List<object>();

				if (request != null)
				{
					logParams.AddRange([request.Method.Method, request.RequestUri, request.Version]);
					logParams.AddRange(request.Headers
							.Where(kvp => !kvp.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase))
							.OrderBy(kvp => kvp.Key)
							.Select(kvp => string.Join(", ", kvp.Value))
							.ToArray());
					logParams.Add(requestContent);
				}

				if (response != null)
				{
					logParams.AddRange([response.Version, (int)response.StatusCode, response.ReasonPhrase]);
					logParams.AddRange(response.Headers
							.Where(kvp => !kvp.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase))
							.OrderBy(kvp => kvp.Key)
							.Select(kvp => string.Join(", ", kvp.Value))
							.ToList());
					logParams.Add(responseContent);
				}

				logParams.Add(elapsed.TotalMilliseconds);

				return logParams.ToArray();
			}

			public string GetFormattedLog()
			{
				var formattedLog = GetLoggingTemplate();
				var args = GetLoggingParameters();

				var pattern = @"(.*?{)(\w+?.+?)(}.*)";
				for (var i = 0; i < args.Length; i++)
				{
					formattedLog = Regex.Replace(formattedLog, pattern, $"$1 {i} $3", RegexOptions.None);
				}

				formattedLog = formattedLog.Replace("{ ", "{").Replace(" }", "}");

				return string.Format(formattedLog, args);
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
				// Update the cached diagnostic info
				diagnosticInfo.ResponseReference = new WeakReference<HttpResponseMessage>(response.Message);
				DiagnosticsInfo[diagnosticId] = diagnosticInfo;

				// Log
				var logLevel = response.IsSuccessStatusCode ? _logLevelSuccessfulCalls : _logLevelFailedCalls;
				if (_logger.IsEnabled(logLevel))
				{
					var template = diagnosticInfo.GetLoggingTemplate();
					var parameters = diagnosticInfo.GetLoggingParameters();

					_logger.Log(logLevel, template, parameters);
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
