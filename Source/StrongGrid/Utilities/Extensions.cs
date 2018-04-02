﻿using HttpMultipartParser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Extension methods
	/// </summary>
	internal static class Extensions
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
		/// Converts a .Net <see cref="DateTime" /> into a 'Unix time' (which is expressed as the number
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
		///     Method = HttpMethod.Get,
		///     RequestUri = new Uri("https://api.vendor.com/v1/endpoint")
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

			// This is important: we must make a copy of the response stream otherwise we would get an
			// exception on subsequent attempts to read the content of the stream
			using (var ms = new MemoryStream())
			{
				await content.CopyToAsync(ms).ConfigureAwait(false);
				ms.Position = 0;
				using (var sr = new StreamReader(ms, encoding))
				{
					responseContent = await sr.ReadToEndAsync().ConfigureAwait(false);
				}
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
		/// <remarks>
		/// This method is equivalent to IRequest.AsBody&lt;T&gt;(T body) because omitting the media type
		/// causes the first formatter in MediaTypeFormatterCollection to be used by default and the first
		/// formatter happens to be the JSON formatter. However, I don't feel good about relying on the
		/// default ordering of the items in the MediaTypeFormatterCollection.
		/// </remarks>
		public static IRequest WithJsonBody<T>(this IRequest request, T body)
		{
			return request.WithBody(body, new MediaTypeHeaderValue("application/json"));
		}

		/// <summary>
		/// Impersonate a user when making a call to the SendGrid API
		/// </summary>
		/// <param name="request">The request</param>
		/// <param name="username">The user to impersonate</param>
		/// <returns>Returns the request builder for chaining.</returns>
		public static IRequest OnBehalfOf(this IRequest request, string username)
		{
			return string.IsNullOrEmpty(username) ? request : request.WithHeader("on-behalf-of", username);
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

		/// <summary>
		///  Converts the value of the current System.TimeSpan object to its equivalent string
		///  representation by using a human readable format.
		/// </summary>
		/// <param name="timeSpan">The time span.</param>
		/// <returns>Returns the human readable representation of the TimeSpan</returns>
		public static string ToDurationString(this TimeSpan timeSpan)
		{
			void AppendFormatIfNecessary(StringBuilder stringBuilder, string timePart, int value)
			{
				if (value <= 0) return;
				stringBuilder.AppendFormat($" {value} {timePart}{(value > 1 ? "s" : string.Empty)}");
			}

			// In case the TimeSpan is extremely short
			if (timeSpan.TotalMilliseconds <= 1) return "1 millisecond";

			var result = new StringBuilder();
			AppendFormatIfNecessary(result, "day", timeSpan.Days);
			AppendFormatIfNecessary(result, "hour", timeSpan.Hours);
			AppendFormatIfNecessary(result, "minute", timeSpan.Days);
			AppendFormatIfNecessary(result, "second", timeSpan.Days);
			AppendFormatIfNecessary(result, "millisecond", timeSpan.Days);
			return result.ToString().Trim();
		}

		/// <summary>
		/// Ensure that a string starts with a given prefix
		/// </summary>
		/// <param name="value">The value</param>
		/// <param name="prefix">The prefix</param>
		/// <returns>The value including the prefix</returns>
		public static string EnsureStartsWith(this string value, string prefix)
		{
			return !string.IsNullOrEmpty(value) && value.StartsWith(prefix) ? value : string.Concat(prefix, value);
		}

		/// <summary>
		/// Ensure that a string ends with a given suffix
		/// </summary>
		/// <param name="value">The value</param>
		/// <param name="suffix">The sufix</param>
		/// <returns>The value including the suffix</returns>
		public static string EnsureEndsWith(this string value, string suffix)
		{
			return !string.IsNullOrEmpty(value) && value.EndsWith(suffix) ? value : string.Concat(value, suffix);
		}

		/// <summary>
		/// Retrieve the permissions (AKA "scopes") assigned to the current user
		/// </summary>
		/// <param name="client">The client</param>
		/// <param name="excludeBillingScopes">Indicates if billing permissions should be excluded from the result</param>
		/// <param name="cancellationToken">The cancellation token</param>
		/// <returns>An array of permisisons assigned to the current user</returns>
		public static async Task<string[]> GetCurrentScopes(this Pathoschild.Http.Client.IClient client, bool excludeBillingScopes, CancellationToken cancellationToken = default(CancellationToken))
		{
			// Get the current user's permissions
			var scopes = await client
				.GetAsync("scopes")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<string[]>("scopes")
				.ConfigureAwait(false);

			if (excludeBillingScopes)
			{
				// In most cases it's important to exclude billing scopes
				// because they are mutually exclusive from all others.
				scopes = scopes
					.Where(p => !p.StartsWith("billing.", StringComparison.OrdinalIgnoreCase))
					.ToArray();
			}

			return scopes;
		}

		public static void AddPropertyIfValue(this JObject jsonObject, string propertyName, string value)
		{
			if (string.IsNullOrEmpty(value)) return;
			jsonObject.Add(propertyName, value);
		}

		public static void AddPropertyIfValue<T>(this JObject jsonObject, string propertyName, T value)
		{
			if (EqualityComparer<T>.Default.Equals(value, default(T))) return;
			jsonObject.Add(propertyName, JToken.FromObject(value));
		}

		public static void AddPropertyIfValue<T>(this JObject jsonObject, string propertyName, IEnumerable<T> value)
		{
			if (value == null || !value.Any()) return;
			jsonObject.Add(propertyName, JArray.FromObject(value.ToArray()));
		}

		public static void AddPropertyIfValue<T>(this JObject jsonObject, string propertyName, Parameter<T> parameter)
		{
			AddPropertyIfValue(jsonObject, propertyName, parameter, value => JToken.FromObject(parameter.Value));
		}

		public static void AddPropertyIfValue<T>(this JObject jsonObject, string propertyName, Parameter<IEnumerable<T>> parameter)
		{
			AddPropertyIfValue(jsonObject, propertyName, parameter, value => JArray.FromObject(value.ToArray()));
		}

		public static void AddPropertyIfEnumValue<T>(this JObject jsonObject, string propertyName, Parameter<T> parameter)
		{
			AddPropertyIfValue(jsonObject, propertyName, parameter, value => JToken.Parse(JsonConvert.SerializeObject(value)).ToString());
		}

		public static void AddPropertyIfValue<T>(this JObject jsonObject, string propertyName, Parameter<T> parameter, Func<T, JToken> convertValueToJsonToken)
		{
			if (convertValueToJsonToken == null) throw new ArgumentNullException(nameof(convertValueToJsonToken));
			if (!parameter.HasValue) return;

			if (parameter.Value == null)
			{
				jsonObject.Add(propertyName, null);
			}
			else
			{
				jsonObject.Add(propertyName, convertValueToJsonToken(parameter.Value));
			}
		}

		public static T GetPropertyValue<T>(this JToken item, string name)
		{
			if (item[name] == null) return default(T);
			return item[name].Value<T>();
		}

		/// <summary>Asynchronously converts the JSON encoded content and converts it to a 'SendGrid' object of the desired type.</summary>
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
				var jProperty = jObject.Property(propertyName);
				if (jProperty == null)
				{
					throw new ArgumentException($"The response does not contain a field called '{propertyName}'", nameof(propertyName));
				}

				return jProperty.Value.ToObject<T>();
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
