using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;
using StrongGrid.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;

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
		private readonly IDictionary<WeakReference<HttpRequestMessage>, Stopwatch> _timers = new Dictionary<WeakReference<HttpRequestMessage>, Stopwatch>();

		#endregion

		#region PUBLIC METHODS

		/// <summary>Method invoked just before the HTTP request is submitted. This method can modify the outgoing HTTP request.</summary>
		/// <param name="request">The HTTP request.</param>
		public void OnRequest(IRequest request)
		{
			request.WithHeader(DIAGNOSTIC_ID_HEADER_NAME, Guid.NewGuid().ToString("N"));

			if (_logger != null && _logger.IsDebugEnabled())
			{
				var httpRequest = request.Message;
				var msg = $"Request: {httpRequest}"
					.Replace("{", "{{")
					.Replace("}", "}}");
				_logger.Debug(msg);
				if (httpRequest.Content != null)
				{
					var content = httpRequest.Content.ReadAsStringAsync(null).Result
						.Replace("{", "{{")
						.Replace("}", "}}");
					_logger.Debug($"Request Content: {content}");
				}
			}

			lock (_timers)
			{
				_timers.Add(new WeakReference<HttpRequestMessage>(request.Message), Stopwatch.StartNew());
			}
		}

		/// <summary>Method invoked just after the HTTP response is received. This method can modify the incoming HTTP response.</summary>
		/// <param name="response">The HTTP response.</param>
		/// <param name="httpErrorAsException">Whether HTTP error responses should be raised as exceptions.</param>
		public void OnResponse(IResponse response, bool httpErrorAsException)
		{
			var elapsed = GetElapsedExecutionTime(response.Message.RequestMessage);

			if (_logger != null && _logger.IsDebugEnabled())
			{
				var httpResponse = response.Message;
				var msg = $"Response: {httpResponse}"
					.Replace("{", "{{")
					.Replace("}", "}}");
				_logger.Debug(msg);
				if (httpResponse.Content != null)
				{
					var content = httpResponse.Content.ReadAsStringAsync(null).Result
						.Replace("{", "{{")
						.Replace("}", "}}");
					_logger.Debug($"Response Content: {content}");
				}

				if (elapsed > TimeSpan.MinValue)
				{
					var elapsedAsString = elapsed.ToDurationString();
					_logger.Debug($"The request took {elapsedAsString}");
				}
			}
		}

		#endregion

		#region PRIVATE METHODS

		private TimeSpan GetElapsedExecutionTime(HttpRequestMessage requestMessage)
		{
			lock (_timers)
			{
				foreach (WeakReference<HttpRequestMessage> key in _timers.Keys.ToArray())
				{
					// Get request
					HttpRequestMessage request;

					// Check if garbage collected
					if (!key.TryGetTarget(out request))
					{
						_timers.Remove(key);
						continue;
					}

					// Check if different request
					var requestDiagnosticId = request.Headers.GetValues(DIAGNOSTIC_ID_HEADER_NAME).FirstOrDefault();
					var requestMessageDiagnosticId = requestMessage.Headers.GetValues(DIAGNOSTIC_ID_HEADER_NAME).FirstOrDefault();
					if (requestDiagnosticId != requestMessageDiagnosticId)
					{
						continue;
					}

					// Measure elapsed time
					var timer = _timers[key];
					timer.Stop();
					var elapsed = timer.Elapsed;

					// Remove the time from dictionary
					_timers.Remove(key);

					return elapsed;
				}
			}

			return TimeSpan.MinValue;
		}

		#endregion
	}
}
