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
	/// Diagnostic handler for requests dispatched to the SendGrid API.
	/// </summary>
	/// <seealso cref="Pathoschild.Http.Client.Extensibility.IHttpFilter" />
	internal class DiagnosticHandler : IHttpFilter
	{
		#region FIELDS

		private const string DIAGNOSTIC_ID_HEADER_NAME = "StrongGrid-DiagnosticId";
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
			diagnosticMessage.AppendLine($"Request Content: {httpRequest.Content?.ReadAsStringAsync(null).GetAwaiter().GetResult() ?? "<NULL>"}");

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
			var diagnosticMessage = string.Empty;

			try
			{
				var diagnosticInfo = GetDiagnosticInfo(response.Message.RequestMessage);
				var diagnosticStringBuilder = diagnosticInfo.Item1;
				var diagnosticTimer = diagnosticInfo.Item2;
				if (diagnosticTimer != null) diagnosticTimer?.Stop();

				var httpResponse = response.Message;

				try
				{
					diagnosticStringBuilder.AppendLine($"Response: {httpResponse}");
					diagnosticStringBuilder.AppendLine($"Content is null: {httpResponse.Content == null}");
					if (httpResponse.Content != null)
					{
						diagnosticStringBuilder.AppendLine($"Content.Headers is null: {httpResponse.Content.Headers == null}");
						if (httpResponse.Content?.Headers != null)
						{
							diagnosticStringBuilder.AppendLine($"Content.Headers.ContentType is null: {httpResponse.Content.Headers.ContentType == null}");
							if (httpResponse.Content?.Headers?.ContentType != null)
							{
								diagnosticStringBuilder.AppendLine($"Content.Headers.ContentType.CharSet: {httpResponse.Content.Headers.ContentType.CharSet}");
							}
						}
					}

					diagnosticStringBuilder.AppendLine($"Response Content: {httpResponse.Content?.ReadAsStringAsync(null).GetAwaiter().GetResult() ?? "<NULL>"}");
				}
				catch
				{
					// Intentionally ignore errors that may occur when attempting to log the content of the response
				}

				if (diagnosticTimer != null)
				{
					diagnosticStringBuilder.AppendLine($"The request took {diagnosticTimer.Elapsed.ToDurationString()}");
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

		#region PRIVATE METHODS

		private Tuple<StringBuilder, Stopwatch> GetDiagnosticInfo(HttpRequestMessage requestMessage)
		{
			lock (_diagnostics)
			{
				var diagnosticId = requestMessage.Headers.GetValues(DIAGNOSTIC_ID_HEADER_NAME).FirstOrDefault();

				foreach (WeakReference<HttpRequestMessage> key in _diagnostics.Keys.ToArray())
				{
					// Check if garbage collected
					if (!key.TryGetTarget(out HttpRequestMessage request))
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
