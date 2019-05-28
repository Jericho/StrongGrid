using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;
using StrongGrid.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Diagnostic handler for requests dispatched to the SendGrid API.
	/// </summary>
	/// <seealso cref="Pathoschild.Http.Client.Extensibility.IHttpFilter" />
	internal class DiagnosticHandler : IHttpFilter
	{
		#region FIELDS

		internal const string DIAGNOSTIC_ID_HEADER_NAME = "StrongGrid-Diagnostic-Id";
		private static readonly ILog _logger = LogProvider.For<DiagnosticHandler>();
		private readonly LogBehavior _logBehavior;

		#endregion

		#region PROPERTIES

		internal static IDictionary<string, (WeakReference<HttpRequestMessage> RequestReference, StringBuilder Diagnostic, long RequestTimestamp, long ResponseTimeStamp)> DiagnosticsInfo { get; } = new Dictionary<string, (WeakReference<HttpRequestMessage>, StringBuilder, long, long)>();

		#endregion

		#region CTOR

		public DiagnosticHandler(LogBehavior logBehavior)
		{
			_logBehavior = logBehavior;
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

			// Log the request
			var httpRequest = request.Message;
			var diagnostic = new StringBuilder();

			diagnostic.AppendLine("REQUEST:");
			diagnostic.AppendLine($"  {httpRequest.Method.Method} {httpRequest.RequestUri}");
			LogHeaders(diagnostic, httpRequest.Headers);
			LogContent(diagnostic, httpRequest.Content);

			// Add the diagnotic info to our cache
			lock (DiagnosticsInfo)
			{
				DiagnosticsInfo.Add(diagnosticId, (new WeakReference<HttpRequestMessage>(request.Message), diagnostic, Stopwatch.GetTimestamp(), long.MinValue));
			}
		}

		/// <summary>Method invoked just after the HTTP response is received. This method can modify the incoming HTTP response.</summary>
		/// <param name="response">The HTTP response.</param>
		/// <param name="httpErrorAsException">Whether HTTP error responses should be raised as exceptions.</param>
		public void OnResponse(IResponse response, bool httpErrorAsException)
		{
			var responseTimestamp = Stopwatch.GetTimestamp();
			var httpResponse = response.Message;

			var diagnosticId = response.Message.RequestMessage.Headers.GetValue(DiagnosticHandler.DIAGNOSTIC_ID_HEADER_NAME);
			var diagnosticInfo = DiagnosticsInfo[diagnosticId];
			diagnosticInfo.ResponseTimeStamp = responseTimestamp;

			try
			{
				// Log the response
				diagnosticInfo.Diagnostic.AppendLine();
				diagnosticInfo.Diagnostic.AppendLine("RESPONSE:");
				diagnosticInfo.Diagnostic.AppendLine($"  HTTP/{httpResponse.Version} {(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase}");
				LogHeaders(diagnosticInfo.Diagnostic, httpResponse.Headers);
				LogContent(diagnosticInfo.Diagnostic, httpResponse.Content);

				// Calculate how much time elapsed between request and response
				var elapsed = TimeSpan.FromTicks(diagnosticInfo.ResponseTimeStamp - diagnosticInfo.RequestTimestamp);

				// Log diagnostic
				diagnosticInfo.Diagnostic.AppendLine();
				diagnosticInfo.Diagnostic.AppendLine("DIAGNOSTIC:");
				diagnosticInfo.Diagnostic.AppendLine($"  The request took {elapsed.ToDurationString()}");
			}
			catch (Exception e)
			{
				Debug.WriteLine("{0}\r\nAN EXCEPTION OCCURRED: {1}\r\n{0}", new string('=', 50), e.GetBaseException().Message);
				diagnosticInfo.Diagnostic.AppendLine($"AN EXCEPTION OCCURRED: {e.GetBaseException().Message}");

				if (_logger != null && _logger.IsErrorEnabled())
				{
					_logger.Error(e, "An exception occurred when inspecting the response from SendGrid");
				}
			}
			finally
			{
				var diagnosticMessage = diagnosticInfo.Diagnostic.ToString();

				if (!string.IsNullOrEmpty(diagnosticMessage))
				{
					Debug.WriteLine("{0}\r\n{1}{0}", new string('=', 50), diagnosticMessage);

					if (_logger != null && _logger.IsDebugEnabled())
					{
						var shouldLog = response.IsSuccessStatusCode && _logBehavior.HasFlag(LogBehavior.LogSuccessfulCalls);
						shouldLog |= !response.IsSuccessStatusCode && _logBehavior.HasFlag(LogBehavior.LogFailedCalls);

						if (shouldLog)
						{
							_logger.Debug(diagnosticMessage
								.Replace("{", "{{")
								.Replace("}", "}}"));
						}
					}
				}

				DiagnosticsInfo[diagnosticId] = diagnosticInfo;

				Cleanup();
			}
		}

		#endregion

		#region PRIVATE METHODS

		private void LogHeaders(StringBuilder diagnostic, HttpHeaders httpHeaders)
		{
			if (httpHeaders != null)
			{
				foreach (var header in httpHeaders)
				{
					if (header.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase))
					{
						diagnostic.AppendLine($"  {header.Key}: ...redacted for security reasons...");
					}
					else
					{
						diagnostic.AppendLine($"  {header.Key}: {string.Join(", ", header.Value)}");
					}
				}
			}
		}

		private void LogContent(StringBuilder diagnostic, HttpContent httpContent)
		{
			if (httpContent == null)
			{
				diagnostic.AppendLine("  Content-Length: 0");
			}
			else
			{
				LogHeaders(diagnostic, httpContent.Headers);

				var contentLength = httpContent.Headers?.ContentLength.GetValueOrDefault(0) ?? 0;
				if (!httpContent.Headers?.Contains("Content-Length") ?? false)
				{
					diagnostic.AppendLine($"  Content-Length: {contentLength}");
				}

				if (contentLength > 0)
				{
					diagnostic.AppendLine();
					diagnostic.AppendLine(httpContent.ReadAsStringAsync(null).GetAwaiter().GetResult() ?? "<NULL>");
				}
			}
		}

		private void Cleanup()
		{
			try
			{
				// Remove diagnostic information for requests that have been garbage collected
				foreach (string key in DiagnosticHandler.DiagnosticsInfo.Keys.ToArray())
				{
					var diagnosticInfo = DiagnosticHandler.DiagnosticsInfo[key];
					if (!diagnosticInfo.RequestReference.TryGetTarget(out HttpRequestMessage request))
					{
						DiagnosticHandler.DiagnosticsInfo.Remove(key);
						continue;
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
