using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;
using System.Net.Http;
using System.Threading.Tasks;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Error handler for requests dispatched to the SendGrid API.
	/// </summary>
	/// <seealso cref="Pathoschild.Http.Client.Extensibility.IHttpFilter" />
	internal class SendGridErrorHandler : IHttpFilter
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

			var errorMessage = GetErrorMessage(response.Message).Result;
			throw new SendGridException(errorMessage, response.Message.StatusCode);
		}

		private static async Task<string> GetErrorMessage(HttpResponseMessage message)
		{
			// Default error message
			var errorMessage = $"{(int)message.StatusCode}: {message.ReasonPhrase}";

			if (message.Content != null)
			{
				/*
					In case of an error, the SendGrid API returns a JSON string that looks like this:
					{
						"errors": [
							{
								"message": "An error has occured",
								"field": null,
								"help": null
							}
						]
					}

					I have also seen cases where the JSON string looks like this:
					{
						"error": "Name already exists"
					}
				*/

				var responseContent = await message.Content.ReadAsStringAsync(null).ConfigureAwait(false);

				if (!string.IsNullOrEmpty(responseContent))
				{
					try
					{
						// Check for the presence of property called 'errors'
						var jObject = JObject.Parse(responseContent);
						var errorsArray = (JArray)jObject["errors"];
						if (errorsArray != null && errorsArray.Count > 0)
						{
							// Get the first error message
							errorMessage = errorsArray[0]["message"].Value<string>();
						}
						else
						{
							// Check for the presence of property called 'error'
							var errorProperty = jObject["error"];
							if (errorProperty != null)
							{
								errorMessage = errorProperty.Value<string>();
							}
						}
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
					}
					catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
					{
						// Intentionally ignore parsing errors
					}
				}
			}

			return errorMessage;
		}

		#endregion
	}
}
