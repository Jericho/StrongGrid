using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;
using StrongGrid.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
			var diagnosticMessage = string.Empty;
			var responseTimestamp = Stopwatch.GetTimestamp();

			try
			{
				var diagnosticStringBuilder = new StringBuilder();

				var httpRequest = response.Message.RequestMessage;
				diagnosticStringBuilder.AppendLine($"Request: {httpRequest}");
				diagnosticStringBuilder.AppendLine($"Request Content: {httpRequest.Content?.ReadAsStringAsync(null).GetAwaiter().GetResult() ?? "<NULL>"}");

				var httpResponse = response.Message;
				diagnosticStringBuilder.AppendLine($"Response: {httpResponse}");
				diagnosticStringBuilder.AppendLine($"Response.Content: {httpResponse.Content?.ReadAsStringAsync(null).GetAwaiter().GetResult() ?? "<NULL>"}");

				if (httpRequest.Headers.TryGetValues(DIAGNOSTIC_TIMESTAMP_HEADER_NAME, out IEnumerable<string> timestampHeaderValues))
				{
					var diagnosticTimestamp = timestampHeaderValues.FirstOrDefault();
					if (long.TryParse(diagnosticTimestamp, out long requestTimestamp))
					{
						var elapsed = TimeSpan.FromTicks(responseTimestamp - requestTimestamp);
						diagnosticStringBuilder.AppendLine($"The request took {elapsed.ToDurationString()}");
					}
				}

				diagnosticMessage = diagnosticStringBuilder.ToString();
			}
			catch (Exception e)
			{
				Debug.WriteLine("{0}\r\nAN EXCEPTION OCCURED: {1}\r\n{0}", new string('=', 25), e.GetBaseException().Message);

				if (_logger != null && _logger.IsErrorEnabled())
				{
					_logger.Error(e, "An exception occured when inspecting the response from SendGrid");
				}
			}
			finally
			{
				if (!string.IsNullOrEmpty(diagnosticMessage))
				{
					Debug.WriteLine("{0}\r\n{1}{0}", new string('=', 25), diagnosticMessage);

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
	}
}
