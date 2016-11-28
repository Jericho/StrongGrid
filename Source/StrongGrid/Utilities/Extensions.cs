using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Extension methods
	/// </summary>
	public static class Extensions
	{
		private static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		/// <summary>
		/// Converts a 'unix time' (which is expressed as the number of seconds since midnight on
		/// January 1st 1970) to a .Net <see cref="DateTime" />.
		/// </summary>
		/// <param name="unixTime">The unix time.</param>
		/// <returns>
		/// The <see cref="DateTime" />.
		/// </returns>
		public static DateTime FromUnixTime(this long unixTime)
		{
			return EPOCH.AddSeconds(unixTime);
		}

		/// <summary>
		/// Converts a .Net <see cref="DateTime" /> into a 'unit time' (which is expressed as the number
		/// of seconds since midnight on January 1st 1970).
		/// </summary>
		/// <param name="date">The date.</param>
		/// <returns>
		/// The numer of seconds since midnight on January 1st 1970.
		/// </returns>
		public static long ToUnixTime(this DateTime date)
		{
			return Convert.ToInt64((date.ToUniversalTime() - EPOCH).TotalSeconds);
		}

		/// <summary>
		/// Ensures that the response was a success. Throws an <see cref="Exception" /> otherwise.
		/// </summary>
		/// <param name="response">The response.</param>
		/// <exception cref="System.Exception">Thrown when the response indicates that something went wrong.</exception>
		public static void EnsureSuccess(this HttpResponseMessage response)
		{
			if (response.IsSuccessStatusCode) return;

			var content = string.Empty;
			if (response.Content != null)
			{
				content = response.Content.ReadAsStringAsync().Result;
				response.Content.Dispose();

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
				content = string.Format("StatusCode: {0}", response.StatusCode);
			}

			throw new Exception(content);
		}
	}
}
