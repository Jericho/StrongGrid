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

		private const string DIAGNOSTIC_ID_HEADER_NAME = "StrongGrid-Diagnostic-Id";
		private const string DIAGNOSTIC_TIMESTAMP_HEADER_NAME = "StrongGrid-Diagnostic-Timestamp";
		private static readonly ILog _logger = LogProvider.For<DiagnosticHandler>();

		#endregion

		#region PUBLIC METHODS

		/// <summary>Method invoked just before the HTTP request is submitted. This method can modify the outgoing HTTP request.</summary>
		/// <param name="request">The HTTP request.</param>
		public void OnRequest(IRequest request)
		{
			// Add a unique ID to the request header
			request.WithHeader(DIAGNOSTIC_ID_HEADER_NAME, Guid.NewGuid().ToString("N"));

			// Add a timestamp to the request header
			request.WithHeader(DIAGNOSTIC_TIMESTAMP_HEADER_NAME, Stopwatch.GetTimestamp().ToString());
		}

		/// <summary>Method invoked just after the HTTP response is received. This method can modify the incoming HTTP response.</summary>
		/// <param name="response">The HTTP response.</param>
		/// <param name="httpErrorAsException">Whether HTTP error responses should be raised as exceptions.</param>
		public void OnResponse(IResponse response, bool httpErrorAsException)
		{
			var responseTimestamp = Stopwatch.GetTimestamp();
			var diagnostic = new StringBuilder();

			try
			{
				var httpRequest = response.Message.RequestMessage;
				var httpResponse = response.Message;

				// Log the request
				diagnostic.AppendLine("REQUEST:");
				diagnostic.AppendLine($"  {httpRequest.Method.Method} {httpRequest.RequestUri}");
				LogHeaders(diagnostic, httpRequest.Headers);
				LogContent(diagnostic, httpRequest.Content);

				// Log the response
				diagnostic.AppendLine();
				diagnostic.AppendLine("RESPONSE:");
				diagnostic.AppendLine($"  HTTP/{httpResponse.Version} {(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase}");
				LogHeaders(diagnostic, httpResponse.Headers);
				LogContent(diagnostic, httpResponse.Content);

				// Log diagnostic
				if (httpRequest.Headers.TryGetValues(DIAGNOSTIC_TIMESTAMP_HEADER_NAME, out IEnumerable<string> timestampHeaderValues))
				{
					var diagnosticTimestamp = timestampHeaderValues.FirstOrDefault();
					if (long.TryParse(diagnosticTimestamp, out long requestTimestamp))
					{
						var elapsed = TimeSpan.FromTicks(responseTimestamp - requestTimestamp);

						diagnostic.AppendLine();
						diagnostic.AppendLine("DIAGNOSTIC:");
						diagnostic.AppendLine($"  The request took {elapsed.ToDurationString()}");
					}
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("{0}\r\nAN EXCEPTION OCCURED: {1}\r\n{0}", new string('=', 50), e.GetBaseException().Message);
				diagnostic.AppendLine($"AN EXCEPTION OCCURED: {e.GetBaseException().Message}");

				if (_logger != null && _logger.IsErrorEnabled())
				{
					_logger.Error(e, "An exception occured when inspecting the response from SendGrid");
				}
			}
			finally
			{
				var diagnosticMessage = diagnostic.ToString();

				if (!string.IsNullOrEmpty(diagnosticMessage))
				{
					Debug.WriteLine("{0}\r\n{1}{0}", new string('=', 50), diagnosticMessage);

					if (_logger != null && _logger.IsDebugEnabled())
					{
						_logger.Debug(diagnosticMessage
							.Replace("{", "{{")
							.Replace("}", "}}"));
					}
				}
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
			if (httpContent != null)
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

		#endregion
	}
}
