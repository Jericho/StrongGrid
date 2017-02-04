using HttpMultipartParser;
using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
		/// Reads the content of the HTTP response as string asynchronously.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="encoding">The encoding. You can leave this parameter null and the encoding will be
		/// automatically calculated based on the charset in the response. Also, UTF-8
		/// encoding will be used if the charset is absent from the response, is blank
		/// or contains an invalid value.</param>
		/// <returns>The string content of the response</returns>
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
		/// 	Method = HttpMethod.Get,
		/// 	RequestUri = new Uri("https://api.vendor.com/v1/endpoint")
		/// };
		/// var httpClient = new HttpClient();
		/// var response = await httpClient.SendAsync(httpRequest, CancellationToken.None).ConfigureAwait(false);
		/// var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
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

		/// <summary>
		/// Returns the value of a parameter or the default value if it doesn't exist.
		/// </summary>
		/// <param name="parser">The parser.</param>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>The value of the parameter</returns>
		public static string GetParameterValue(this MultipartFormDataParser parser, string name, string defaultValue)
		{
			if (parser.HasParameter(name)) return parser.GetParameterValue(name);
			else return defaultValue;
		}

		/// <summary>Asynchronously retrieve the JSON encoded response body and convert it to a 'SendGrid' object of the desired type.</summary>
		/// <typeparam name="T">The response model to deserialize into.</typeparam>
		/// <param name="response">The response</param>
		/// <param name="propertyName">The name of the JSON property (or null if not applicable) where the desired data is stored</param>
		/// <returns>Returns the response body, or <c>null</c> if the response has no body.</returns>
		/// <exception cref="ApiException">An error occurred processing the response.</exception>
		public static Task<T> AsSendGridObject<T>(this IResponse response, string propertyName = null)
		{
			return response.Message.Content.AsSendGridObject<T>(propertyName);
		}

		/// <summary>Asynchronously retrieve the JSON encoded response body and convert it to a 'SendGrid' object of the desired type.</summary>
		/// <typeparam name="T">The response model to deserialize into.</typeparam>
		/// <param name="request">The request</param>
		/// <param name="propertyName">The name of the JSON property (or null if not applicable) where the desired data is stored</param>
		/// <returns>Returns the response body, or <c>null</c> if the response has no body.</returns>
		/// <exception cref="ApiException">An error occurred processing the response.</exception>
		public static async Task<T> AsSendGridObject<T>(this IRequest request, string propertyName = null)
		{
			var response = await request.AsMessage().ConfigureAwait(false);
			return await response.Content.AsSendGridObject<T>(propertyName).ConfigureAwait(false);
		}

		/// <summary>Set the body content of the HTTP request.</summary>
		/// <typeparam name="T">The type of object to serialize into a JSON string.</typeparam>
		/// <param name="request">The request.</param>
		/// <param name="body">The value to serialize into the HTTP body content.</param>
		/// <returns>Returns the request builder for chaining.</returns>
		public static IRequest WithJsonBody<T>(this IRequest request, T body)
		{
			return request.WithBody(body, new MediaTypeHeaderValue("application/json"));
		}

		/// <summary>Asynchronously retrieve the response body as a <see cref="string"/>.</summary>
		/// <param name="response">The response</param>
		/// <param name="encoding">The encoding. You can leave this parameter null and the encoding will be
		/// automatically calculated based on the charset in the response. Also, UTF-8
		/// encoding will be used if the charset is absent from the response, is blank
		/// or contains an invalid value.</param>
		/// <returns>Returns the response body, or <c>null</c> if the response has no body.</returns>
		/// <exception cref="ApiException">An error occurred processing the response.</exception>
		public static Task<string> AsString(this IResponse response, Encoding encoding)
		{
			return response.Message.Content.ReadAsStringAsync(encoding);
		}

		/// <summary>Asynchronously retrieve the response body as a <see cref="string"/>.</summary>
		/// <param name="request">The request</param>
		/// <param name="encoding">The encoding. You can leave this parameter null and the encoding will be
		/// automatically calculated based on the charset in the response. Also, UTF-8
		/// encoding will be used if the charset is absent from the response, is blank
		/// or contains an invalid value.</param>
		/// <returns>Returns the response body, or <c>null</c> if the response has no body.</returns>
		/// <exception cref="ApiException">An error occurred processing the response.</exception>
		public static async Task<string> AsString(this IRequest request, Encoding encoding)
		{
			IResponse response = await request.AsResponse().ConfigureAwait(false);
			return await response.AsString(encoding).ConfigureAwait(false);
		}

		/// <summary>Asynchronously converts the JSON encoded content aand convert it to a 'SendGrid' object of the desired type.</summary>
		/// <typeparam name="T">The response model to deserialize into.</typeparam>
		/// <param name="httpContent">The content</param>
		/// <param name="propertyName">The name of the JSON property (or null if not applicable) where the desired data is stored</param>
		/// <returns>Returns the response body, or <c>null</c> if the response has no body.</returns>
		/// <exception cref="ApiException">An error occurred processing the response.</exception>
		private static async Task<T> AsSendGridObject<T>(this HttpContent httpContent, string propertyName = null)
		{
			var responseContent = await httpContent.ReadAsStringAsync(null).ConfigureAwait(false);

			if (!string.IsNullOrEmpty(propertyName))
			{
				var jObject = JObject.Parse(responseContent);
				return jObject[propertyName].ToObject<T>();
			}
			else if (typeof(T).IsArray)
			{
				return JArray.Parse(responseContent).ToObject<T>();
			}
			else
			{
				return JObject.Parse(responseContent).ToObject<T>();
			}
		}
	}
}
