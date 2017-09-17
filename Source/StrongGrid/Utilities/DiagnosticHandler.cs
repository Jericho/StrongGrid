using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;
using StrongGrid.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Diagnostic handler for requests dispatched to the SendGrid API
	/// </summary>
	/// <seealso cref="Pathoschild.Http.Client.Extensibility.IHttpFilter" />
	internal class DiagnosticHandler : IHttpFilter
	{
		#region FIELDS

		private const string DIAGNOSTIC_ID_HEADER_NAME = "StrongGridDiagnosticId";
		private static readonly ILog _logger = LogProvider.For<DiagnosticHandler>();
		private readonly IDictionary<WeakReference<HttpRequestMessage>, Tuple<StringBuilder, Stopwatch>> _diagnostics = new Dictionary<WeakReference<HttpRequestMessage>, Tuple<StringBuilder, Stopwatch>>();

		#endregion

		#region PUBLIC METHODS

		/// <summary>Method invoked just before the HTTP request is submitted. This method can modify the outgoing HTTP request.</summary>
		/// <param name="request">The HTTP request.</param>
		public void OnRequest(IRequest request)
		{
			request.WithHeader(DIAGNOSTIC_ID_HEADER_NAME, Guid.NewGuid().ToString("N"));

			var httpRequest = request.Message;

			var diagnosticMessage = new StringBuilder();
			diagnosticMessage.AppendLine($"Request: {httpRequest}");
			diagnosticMessage.AppendLine($"Request Content: {httpRequest.Content?.ReadAsStringAsync(null).Result ?? "<NULL>"}");

			lock (_diagnostics)
			{
				_diagnostics.Add(new WeakReference<HttpRequestMessage>(request.Message), new Tuple<StringBuilder, Stopwatch>(diagnosticMessage, Stopwatch.StartNew()));
			}
		}

		/// <summary>Method invoked just after the HTTP response is received. This method can modify the incoming HTTP response.</summary>
		/// <param name="response">The HTTP response.</param>
		/// <param name="httpErrorAsException">Whether HTTP error responses should be raised as exceptions.</param>
		public void OnResponse(IResponse response, bool httpErrorAsException)
		{
			var diagnosticInfo = GetDiagnosticInfo(response.Message.RequestMessage);
			var diagnosticMessage = diagnosticInfo.Item1;
			var timer = diagnosticInfo.Item2;
			if (timer != null) timer.Stop();

			var httpResponse = response.Message;

			diagnosticMessage.AppendLine($"Response: {httpResponse}");
			diagnosticMessage.AppendLine($"Response Content: {httpResponse.Content?.ReadAsStringAsync(null).Result ?? "<NULL>"}");

			if (timer != null)
			{
				diagnosticMessage.AppendLine($"The request took {timer.Elapsed.ToDurationString()}");
			}

			Debug.WriteLine("{0}\r\n{1}{0}", new string('=', 25), diagnosticMessage.ToString());

			if (_logger != null && _logger.IsDebugEnabled())
			{
				_logger.Debug(diagnosticMessage.ToString()
					.Replace("{", "{{")
					.Replace("}", "}}"));
			}
		}

		#endregion

		#region PRIVATE METHODS

		private Tuple<StringBuilder, Stopwatch> GetDiagnosticInfo(HttpRequestMessage requestMessage)
		{
			lock (_diagnostics)
			{
				var diagnosticId = requestMessage.Headers.GetValues(DIAGNOSTIC_ID_HEADER_NAME).FirstOrDefault();

				foreach (WeakReference<HttpRequestMessage> key in _diagnostics.Keys.ToArray())
				{
					// Get request
					HttpRequestMessage request;

					// Check if garbage collected
					if (!key.TryGetTarget(out request))
					{
						_diagnostics.Remove(key);
						continue;
					}

					// Check if different request
					var requestDiagnosticId = request.Headers.GetValues(DIAGNOSTIC_ID_HEADER_NAME).FirstOrDefault();
					if (requestDiagnosticId != diagnosticId)
					{
						continue;
					}

					// Get the diagnostic info from dictionary
					var diagnosticInfo = _diagnostics[key];

					// Remove the diagnostic info from dictionary
					_diagnostics.Remove(key);

					return diagnosticInfo;
				}
			}

			return new Tuple<StringBuilder, Stopwatch>(new StringBuilder(), null);
		}

		#endregion
	}
}
