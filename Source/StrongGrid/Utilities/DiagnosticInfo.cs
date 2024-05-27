using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace StrongGrid.Utilities
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

				var requestHeaders = response?.RequestMessage?.Headers ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>();
				if (!requestHeaders.Any(kvp => string.Equals(kvp.Key, "Content-Length", StringComparison.OrdinalIgnoreCase)))
				{
					requestHeaders = requestHeaders.Append(new KeyValuePair<string, IEnumerable<string>>("Content-Length", new[] { "0" }));
				}

				foreach (var header in requestHeaders.OrderBy(kvp => kvp.Key))
				{
					logTemplate.AppendLine("  " + header.Key + ": {Request_Header_" + header.Key + "}");
				}

				logTemplate.AppendLine("REQUEST: {Request_Content}");
				logTemplate.AppendLine();
			}

			if (response != null)
			{
				logTemplate.AppendLine("RESPONSE FROM SENDGRID: HTTP/{Response_HttpVersion} {Response_StatusCode} {Response_ReasonPhrase}");
				logTemplate.AppendLine("RESPONSE HEADERS:");

				var responseHeaders = response?.Headers ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>();
				if (!responseHeaders.Any(kvp => string.Equals(kvp.Key, "Content-Length", StringComparison.OrdinalIgnoreCase)))
				{
					responseHeaders = responseHeaders.Append(new KeyValuePair<string, IEnumerable<string>>("Content-Length", new[] { "0" }));
				}

				foreach (var header in responseHeaders.OrderBy(kvp => kvp.Key))
				{
					logTemplate.AppendLine("  " + header.Key + ": {Response_Header_" + header.Key + "}");
				}

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

			// Get the content to the request/response and calculate how long it took to get the response
			var elapsed = TimeSpan.FromTicks(ResponseTimestamp - RequestTimestamp);
			var requestContent = request?.Content?.ReadAsStringAsync(null).GetAwaiter().GetResult();
			var responseContent = response?.Content?.ReadAsStringAsync(null).GetAwaiter().GetResult();

			// Calculate the content size
			var requestContentLength = requestContent?.Length ?? 0;
			var responseContentLength = responseContent?.Length ?? 0;

			// Get the request headers (please note: intentionally getting headers from "response.RequestMessage" rather than "request")
			var requestHeaders = response?.RequestMessage?.Headers ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>();
			if (!requestHeaders.Any(kvp => string.Equals(kvp.Key, "Content-Length", StringComparison.OrdinalIgnoreCase)))
			{
				requestHeaders = requestHeaders.Append(new KeyValuePair<string, IEnumerable<string>>("Content-Length", new[] { requestContentLength.ToString() }));
			}

			// Get the response headers
			var responseHeaders = response?.Headers ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>();
			if (!responseHeaders.Any(kvp => string.Equals(kvp.Key, "Content-Length", StringComparison.OrdinalIgnoreCase)))
			{
				responseHeaders = responseHeaders.Append(new KeyValuePair<string, IEnumerable<string>>("Content-Length", new[] { responseContentLength.ToString() }));
			}

			// The order of these values must match the order in which they appear in the logging template
			var logParams = new List<object>();

			if (request != null)
			{
				logParams.AddRange([request.Method.Method, request.RequestUri, request.Version]);
				logParams.AddRange(requestHeaders
						.OrderBy(kvp => kvp.Key)
						.Select(kvp => kvp.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase) ? "... omitted for security reasons ..." : string.Join(", ", kvp.Value))
						.ToArray());
				logParams.Add(requestContent?.TrimEnd('\r', '\n'));
			}

			if (response != null)
			{
				logParams.AddRange([response.Version, (int)response.StatusCode, response.ReasonPhrase]);
				logParams.AddRange(responseHeaders
						.OrderBy(kvp => kvp.Key)
						.Select(kvp => kvp.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase) ? "... omitted for security reasons ..." : string.Join(", ", kvp.Value))
						.ToList());
				logParams.Add(responseContent?.TrimEnd('\r', '\n'));
			}

			logParams.Add(elapsed.TotalMilliseconds);

			return logParams.ToArray();
		}

		public string GetFormattedLog()
		{
			var template = GetLoggingTemplate();
			var args = GetLoggingParameters();

			var formater = new LogValuesFormatter(template);
			var formattedLog = formater.Format(args);

			return formattedLog;
		}
	}
}
