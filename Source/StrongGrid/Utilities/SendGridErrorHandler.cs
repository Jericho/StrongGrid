using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;
using System;
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
		public void OnResponse(IResponse response)
		{
			if (response.Message.IsSuccessStatusCode) return;

			var content = string.Empty;
			if (response.Message.Content != null)
			{
				content = response.Message.Content.ReadAsStringAsync(null).Result;
				response.Message.Content.Dispose();

				try
				{
					// Response looks like this:
					// {
					//   "errors": [
					//     {
					//       "message": "An error has occured",
					//       "field": null,
					//       "help": null
					//     }
					//  ]
					// }
					// We use a dynamic object to get rid of the 'errors' property and get the first error message
					dynamic dynamicObject = JObject.Parse(content);
					dynamic dynamicArray = dynamicObject.errors;
					dynamic firstError = dynamicArray.First;

					content = firstError.message.ToString();
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
				}
				catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
				{
					// Intentionally ignore parsing errors
				}
			}

			if (string.IsNullOrEmpty(content))
			{
				content = $"StatusCode: {response.Message.StatusCode}";
			}

			throw new Exception(content);
		}

		#endregion
	}
}
