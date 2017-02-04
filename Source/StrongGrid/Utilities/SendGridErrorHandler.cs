using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;
using System;
using System.IO;
using System.Net.Http;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Implements IRetryConfig with back off based on a wait time derived from the
	/// "X-RateLimit-Reset" response header. The value in this header contains the
	/// date and time (expressed as the number of seconds since midnight on January
	/// 1st 1970) when the next attempt can take place.
	/// </summary>
	/// <seealso cref="Pathoschild.Http.Client.Extensibility.IHttpFilter" />
	public class SendGridErrorHandler : IHttpFilter
	{
		#region PUBLIC METHODS

		/// <summary>Method invoked just before the HTTP request is submitted. This method can modify the outgoing HTTP request.</summary>
		/// <param name="request">The HTTP request.</param>
		public void OnRequest(IRequest request) { }

		/// <summary>Method invoked just after the HTTP response is received. This method can modify the incoming HTTP response.</summary>
		/// <param name="response">The HTTP response.</param>
		/// <param name="httpErrorAsException">Whether HTTP error responses should be raised as exceptions.</param>
		public void OnResponse(IResponse response, bool httpErrorAsException)
		{
			if (response.Message.IsSuccessStatusCode) return;

			var errorMessage = GetErrorMessage(response.Message);
			if (string.IsNullOrEmpty(errorMessage))
			{
				errorMessage = $"{(int)response.Message.StatusCode}: {response.Message.ReasonPhrase}";
			}

			throw new Exception(errorMessage);
		}

		private static string GetErrorMessage(HttpResponseMessage message)
		{
			// In case of an error, the SendGrid API returns a JSON string that looks like this:
			// {
			//   "errors": [
			//     {
			//       "message": "An error has occured",
			//       "field": null,
			//       "help": null
			//     }
			//  ]
			// }

			if (message.Content == null) return null;

			var errorMessage = string.Empty;
			var responseContent = string.Empty;

			// This is important: we must make a copy of the response stream otherwise we won't be able to read it outside of this error handler
			using (var ms = new MemoryStream())
			{
				message.Content.CopyToAsync(ms).Wait();
				ms.Position = 0;
				var sr = new StreamReader(ms);
				responseContent = sr.ReadToEnd();
			}

			// We assume no error occured if the content is empty
			if (string.IsNullOrEmpty(responseContent)) return null;

			try
			{
				// Check for the presence of property called 'errors' to determine if an error occured
				var jObject = JObject.Parse(responseContent);
				var errorsArray = (JArray)jObject["errors"];

				if (errorsArray == null || errorsArray.Count == 0) return null;

				// Get the first error message
				errorMessage = errorsArray[0]["message"].ToString();
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
			}
			catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
			{
				// Intentionally ignore parsing errors
			}

			return errorMessage;
		}

		#endregion
	}
}
