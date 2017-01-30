using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;
using System;
using System.IO;

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
		public void OnResponse(IResponse response)
		{
			// The default FluentHttpClient error filter handles HTTP problems
			if (!response.Message.IsSuccessStatusCode) return;

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

			// We assume no error occured if there is no content
			if (response.Message.Content == null) return;

			var isError = false;
			var errorMessage = string.Empty;
			var responseContent = string.Empty;

			// This is important: we must make a copy of the response stream otherwise we won't be able to read it outside of this error handler
			using (var ms = new MemoryStream())
			{
				response.Message.Content.CopyToAsync(ms).Wait();
				ms.Position = 0;
				var sr = new StreamReader(ms);
				responseContent = sr.ReadToEnd();
			}

			// We assume no error occured if the content is empty
			if (string.IsNullOrEmpty(responseContent)) return;

			try
			{
				// Check for the presence of property called 'errors' to determine if an error occured
				var jObject = JObject.Parse(responseContent);
				var errorsArray = (JArray)jObject["errors"];

				isError = errorsArray != null;

				// Get the first error message
				if (isError && errorsArray.Count >= 1)
				{
					errorMessage = errorsArray[0]["message"].ToString();
				}
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
			}
			catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
			{
				// Intentionally ignore parsing errors
			}

			if (isError)
			{
				if (string.IsNullOrEmpty(errorMessage))
				{
					errorMessage = $"StatusCode: {response.Message.StatusCode}";
				}

				throw new Exception(errorMessage);
			}
		}

		#endregion
	}
}
