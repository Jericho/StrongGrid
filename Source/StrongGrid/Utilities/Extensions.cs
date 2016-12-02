using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
				content = response.Content.ReadAsStringAsync(null).Result;
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

		/// <summary>
		/// Reads the content of the HTTP response as string asynchronously.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="encoding">The encoding. You can leave this parameter null and the encoding will be
		/// automatically calculated based on the charset in the response. Also, UTF-8
		/// encoding will be used if the charset is absent from the response, is blank
		/// or contains an invalid value.</param>
		/// <returns></returns>
		/// <remarks>
		/// This method is an improvement over the built-in ReadAsStringAsync method
		/// because it can handle invalid charset returned in the response. For example
		/// you may be sending a request to an API that returns a blank charset or a
		/// mispelled one like 'utf8' instead of the correctly spelled 'utf-8'. The
		/// built-in method throws an exception if an invalid charset is specified
		/// while this method uses the UTF-8 encoding in that situation.
		/// 
		/// My motivation for writing this extension method was to work around a situation
		/// where the 3rd party API I was sending requests to would sometimes return 'utf8'
		/// as the charset and an exception would be thrown when I called the ReadAsStringAsync
		/// method to get the content of the response into a string because the .Net HttpClient
		/// would attempt to determine the proper encoding to use but it would fail due to
		/// the fact that the charset was misspelled. I contacted the vendor, asking them
		/// to either omit the charset or fix the misspelling but they didn't feel the need
		/// to fix this issue because:
		/// "in some programming languages, you can use the syntax utf8 instead of utf-8".
		/// In other words, they are happy to continue using the misspelled value which is
		/// supported by "some" programming languages instead of using the properly spelled
		/// value which is supported by all programming languages!
		/// </remarks>
		/// <example>
		/// <code>
		/// var httpRequest = new HttpRequestMessage
		/// {
		///		Method = HttpMethod.Get,
		///		RequestUri = new Uri("https://api.vendor.com/v1/endpoint")
		///	};
		///	var httpClient = new HttpClient();
		///	var response = await httpClient.SendAsync(httpRequest, CancellationToken.None).ConfigureAwait(false);
		///	var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
		/// </code>
		/// </example>
		public static async Task<string> ReadAsStringAsync(this HttpContent content, Encoding encoding)
		{
			var responseStream = await content.ReadAsStreamAsync().ConfigureAwait(false);
			var responseContent = string.Empty;

			if (encoding == null) encoding = content.GetEncoding(Encoding.UTF8);

			using (var sr = new StreamReader(responseStream, encoding))
			{
				responseContent = await sr.ReadToEndAsync().ConfigureAwait(false);
			}

			return responseContent;
		}

		/// <summary>
		/// Gets the encoding.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="defaultEncoding">The default encoding.</param>
		/// <returns>
		/// The encoding
		/// </returns>
		/// <remarks>
		/// This method tries to get the encoding based on the charset or uses the
		/// 'defaultEncoding' if the charset is empty or contains an invalid value.
		/// </remarks>
		/// <example>
		///   <code>
		/// var httpRequest = new HttpRequestMessage
		/// {
		/// Method = HttpMethod.Get,
		/// RequestUri = new Uri("https://my.api.com/v1/myendpoint")
		/// };
		/// var httpClient = new HttpClient();
		/// var response = await httpClient.SendAsync(httpRequest, CancellationToken.None).ConfigureAwait(false);
		/// var encoding = response.Content.GetEncoding(Encoding.UTF8);
		/// </code>
		/// </example>
		public static Encoding GetEncoding(this HttpContent content, Encoding defaultEncoding)
		{

			var encoding = defaultEncoding;
			var charset = content.Headers.ContentType.CharSet;
			if (!string.IsNullOrEmpty(charset))
			{
				try
				{
					encoding = Encoding.GetEncoding(charset);
				}
				catch
				{
					encoding = defaultEncoding;
				}
			}

			return encoding;
		}
	}
}
