using HttpMultipartParser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid
{
	/// <summary>
	/// Internal extension methods.
	/// </summary>
	internal static class Internal
	{
		/// <summary>
		/// Converts a 'unix time' (which is expressed as the number of seconds since midnight on
		/// January 1st 1970) to a .Net <see cref="DateTime" />.
		/// </summary>
		/// <param name="unixTime">The unix time.</param>
		/// <returns>
		/// The <see cref="DateTime" />.
		/// </returns>
		internal static DateTime FromUnixTime(this long unixTime)
		{
			return Utils.Epoch.AddSeconds(unixTime);
		}

		/// <summary>
		/// Converts a .Net <see cref="DateTime" /> into a 'Unix time' (which is expressed as the number
		/// of seconds since midnight on January 1st 1970).
		/// </summary>
		/// <param name="date">The date.</param>
		/// <returns>
		/// The numer of seconds since midnight on January 1st 1970.
		/// </returns>
		internal static long ToUnixTime(this DateTime date)
		{
			return Convert.ToInt64((date.ToUniversalTime() - Utils.Epoch).TotalSeconds);
		}

		/// <summary>
		/// Reads the content of the HTTP response as string asynchronously.
		/// </summary>
		/// <param name="httpContent">The content.</param>
		/// <param name="encoding">The encoding. You can leave this parameter null and the encoding will be
		/// automatically calculated based on the charset in the response. Also, UTF-8
		/// encoding will be used if the charset is absent from the response, is blank
		/// or contains an invalid value.</param>
		/// <returns>The string content of the response.</returns>
		/// <remarks>
		/// This method is an improvement over the built-in ReadAsStringAsync method
		/// because it can handle invalid charset returned in the response. For example
		/// you may be sending a request to an API that returns a blank charset or a
		/// misspelled one like 'utf8' instead of the correctly spelled 'utf-8'. The
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
		/// value which is supported by all programming languages.
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
		internal static async Task<string> ReadAsStringAsync(this HttpContent httpContent, Encoding encoding)
		{
			var content = string.Empty;

			if (httpContent != null)
			{
				var contentStream = await httpContent.ReadAsStreamAsync().ConfigureAwait(false);

				if (encoding == null) encoding = httpContent.GetEncoding(Encoding.UTF8);

				// This is important: we must make a copy of the response stream otherwise we would get an
				// exception on subsequent attempts to read the content of the stream
				using (var ms = Utils.MemoryStreamManager.GetStream())
				{
					await contentStream.CopyToAsync(ms).ConfigureAwait(false);
					ms.Position = 0;
					using (var sr = new StreamReader(ms, encoding))
					{
						content = await sr.ReadToEndAsync().ConfigureAwait(false);
					}

					// It's important to rewind the stream
					if (contentStream.CanSeek) contentStream.Position = 0;
				}
			}

			return content;
		}

		/// <summary>
		/// Gets the encoding.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="defaultEncoding">The default encoding.</param>
		/// <returns>
		/// The encoding.
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
		internal static Encoding GetEncoding(this HttpContent content, Encoding defaultEncoding)
		{
			var encoding = defaultEncoding;
			try
			{
				var charset = content?.Headers?.ContentType?.CharSet;
				if (!string.IsNullOrEmpty(charset))
				{
					encoding = Encoding.GetEncoding(charset);
				}
			}
			catch
			{
				encoding = defaultEncoding;
			}

			return encoding;
		}

		/// <summary>
		/// Returns the value of a parameter or the default value if it doesn't exist.
		/// </summary>
		/// <param name="parser">The parser.</param>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>The value of the parameter.</returns>
		internal static string GetParameterValue(this MultipartFormDataParser parser, string name, string defaultValue)
		{
			if (parser.HasParameter(name)) return parser.GetParameterValue(name);
			else return defaultValue;
		}

		/// <summary>Asynchronously retrieve the JSON encoded response body and convert it to an object of the desired type.</summary>
		/// <typeparam name="T">The response model to deserialize into.</typeparam>
		/// <param name="response">The response.</param>
		/// <param name="propertyName">The name of the JSON property (or null if not applicable) where the desired data is stored.</param>
		/// <param name="throwIfPropertyIsMissing">Indicates if an exception should be thrown when the specified JSON property is missing from the response.</param>
		/// <param name="jsonConverter">Converter that will be used during deserialization.</param>
		/// <returns>Returns the strongly typed object.</returns>
		/// <exception cref="SendGridException">An error occurred processing the response.</exception>
		internal static Task<T> AsSendGridObject<T>(this IResponse response, string propertyName = null, bool throwIfPropertyIsMissing = true, JsonConverter jsonConverter = null)
		{
			return response.Message.Content.AsSendGridObject<T>(propertyName, throwIfPropertyIsMissing, jsonConverter);
		}

		/// <summary>Asynchronously retrieve the JSON encoded response body and convert it to an object of the desired type.</summary>
		/// <typeparam name="T">The response model to deserialize into.</typeparam>
		/// <param name="request">The request.</param>
		/// <param name="propertyName">The name of the JSON property (or null if not applicable) where the desired data is stored.</param>
		/// <param name="throwIfPropertyIsMissing">Indicates if an exception should be thrown when the specified JSON property is missing from the response.</param>
		/// <param name="jsonConverter">Converter that will be used during deserialization.</param>
		/// <returns>Returns the strongly typed object.</returns>
		/// <exception cref="SendGridException">An error occurred processing the response.</exception>
		internal static async Task<T> AsSendGridObject<T>(this IRequest request, string propertyName = null, bool throwIfPropertyIsMissing = true, JsonConverter jsonConverter = null)
		{
			var response = await request.AsMessage().ConfigureAwait(false);
			return await response.Content.AsSendGridObject<T>(propertyName, throwIfPropertyIsMissing, jsonConverter).ConfigureAwait(false);
		}

		/// <summary>Asynchronously retrieve the JSON encoded content and converts it to a 'PaginatedResponse' object.</summary>
		/// <typeparam name="T">The response model to deserialize into.</typeparam>
		/// <param name="response">The response.</param>
		/// <param name="propertyName">The name of the JSON property (or null if not applicable) where the desired data is stored.</param>
		/// <param name="jsonConverter">Converter that will be used during deserialization.</param>
		/// <returns>Returns the paginated response.</returns>
		/// <exception cref="SendGridException">An error occurred processing the response.</exception>
		internal static Task<PaginatedResponse<T>> AsPaginatedResponse<T>(this IResponse response, string propertyName = null, JsonConverter jsonConverter = null)
		{
			return response.Message.Content.AsPaginatedResponse<T>(propertyName, jsonConverter);
		}

		/// <summary>Asynchronously retrieve the JSON encoded content and converts it to a 'PaginatedResponse' object.</summary>
		/// <typeparam name="T">The response model to deserialize into.</typeparam>
		/// <param name="request">The request.</param>
		/// <param name="propertyName">The name of the JSON property (or null if not applicable) where the desired data is stored.</param>
		/// <param name="jsonConverter">Converter that will be used during deserialization.</param>
		/// <returns>Returns the paginated response.</returns>
		/// <exception cref="SendGridException">An error occurred processing the response.</exception>
		internal static async Task<PaginatedResponse<T>> AsPaginatedResponse<T>(this IRequest request, string propertyName = null, JsonConverter jsonConverter = null)
		{
			var response = await request.AsMessage().ConfigureAwait(false);
			return await response.Content.AsPaginatedResponse<T>(propertyName, jsonConverter).ConfigureAwait(false);
		}

		/// <summary>Get a raw JSON object representation of the response, which can also be accessed as a <c>dynamic</c> value.</summary>
		/// <exception cref="SendGridException">An error occurred processing the response.</exception>
		internal static Task<JObject> AsRawJsonObject(this IResponse response, string propertyName = null, bool throwIfPropertyIsMissing = true)
		{
			return response.Message.Content.AsRawJsonObject(propertyName, throwIfPropertyIsMissing);
		}

		/// <summary>Get a raw JSON object representation of the response, which can also be accessed as a <c>dynamic</c> value.</summary>
		/// <exception cref="SendGridException">An error occurred processing the response.</exception>
		internal static async Task<JObject> AsRawJsonObject(this IRequest request, string propertyName = null, bool throwIfPropertyIsMissing = true)
		{
			var response = await request.AsMessage().ConfigureAwait(false);
			return await response.Content.AsRawJsonObject(propertyName, throwIfPropertyIsMissing).ConfigureAwait(false);
		}

		/// <summary>Set the body content of the HTTP request.</summary>
		/// <typeparam name="T">The type of object to serialize into a JSON string.</typeparam>
		/// <param name="request">The request.</param>
		/// <param name="body">The value to serialize into the HTTP body content.</param>
		/// <param name="omitCharSet">
		/// Indicates if the charset should be omitted from the 'Content-Type' request header.
		/// The vast majority of SendGrid's endpoints require this parameter to be false but one
		/// notable exception is 'Contacts.Upsert' in the new marketing campaigns API.
		/// SendGrid has not documented when it should be true/false, I only figured it out when
		/// getting a "invalid content-type: application/json; charset=utf-8" exception which was
		/// solved by omitting the "charset".
		/// </param>
		/// <returns>Returns the request builder for chaining.</returns>
		/// <remarks>
		/// This method is equivalent to IRequest.AsBody&lt;T&gt;(T body) because omitting the media type
		/// causes the first formatter in MediaTypeFormatterCollection to be used by default and the first
		/// formatter happens to be the JSON formatter. However, I don't feel good about relying on the
		/// default ordering of the items in the MediaTypeFormatterCollection.
		/// </remarks>
		internal static IRequest WithJsonBody<T>(this IRequest request, T body, bool omitCharSet = false)
		{
			return request.WithBody(bodyBuilder =>
			{
				var httpContent = bodyBuilder.Model(body, new MediaTypeHeaderValue("application/json"));

				if (omitCharSet && !string.IsNullOrEmpty(httpContent.Headers.ContentType.CharSet))
				{
					httpContent.Headers.ContentType.CharSet = string.Empty;
				}

				return httpContent;
			});
		}

		/// <summary>
		/// Impersonate a user when making a call to the SendGrid API.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="username">The user to impersonate.</param>
		/// <returns>Returns the request builder for chaining.</returns>
		internal static IRequest OnBehalfOf(this IRequest request, string username)
		{
			return string.IsNullOrEmpty(username) ? request : request.WithHeader("on-behalf-of", username);
		}

		/// <summary>Asynchronously retrieve the response body as a <see cref="string"/>.</summary>
		/// <param name="response">The response.</param>
		/// <param name="encoding">The encoding. You can leave this parameter null and the encoding will be
		/// automatically calculated based on the charset in the response. Also, UTF-8
		/// encoding will be used if the charset is absent from the response, is blank
		/// or contains an invalid value.</param>
		/// <returns>Returns the response body, or <c>null</c> if the response has no body.</returns>
		/// <exception cref="SendGridException">An error occurred processing the response.</exception>
		internal static Task<string> AsString(this IResponse response, Encoding encoding)
		{
			return response.Message.Content.ReadAsStringAsync(encoding);
		}

		/// <summary>Asynchronously retrieve the response body as a <see cref="string"/>.</summary>
		/// <param name="request">The request.</param>
		/// <param name="encoding">The encoding. You can leave this parameter null and the encoding will be
		/// automatically calculated based on the charset in the response. Also, UTF-8
		/// encoding will be used if the charset is absent from the response, is blank
		/// or contains an invalid value.</param>
		/// <returns>Returns the response body, or <c>null</c> if the response has no body.</returns>
		/// <exception cref="SendGridException">An error occurred processing the response.</exception>
		internal static async Task<string> AsString(this IRequest request, Encoding encoding)
		{
			IResponse response = await request.AsResponse().ConfigureAwait(false);
			return await response.AsString(encoding).ConfigureAwait(false);
		}

		/// <summary>
		///  Converts the value of the current System.TimeSpan object to its equivalent string
		///  representation by using a human readable format.
		/// </summary>
		/// <param name="timeSpan">The time span.</param>
		/// <returns>Returns the human readable representation of the TimeSpan.</returns>
		internal static string ToDurationString(this TimeSpan timeSpan)
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
			AppendFormatIfNecessary(result, "minute", timeSpan.Minutes);
			AppendFormatIfNecessary(result, "second", timeSpan.Seconds);
			AppendFormatIfNecessary(result, "millisecond", timeSpan.Milliseconds);
			return result.ToString().Trim();
		}

		/// <summary>
		/// Ensure that a string starts with a given prefix.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="prefix">The prefix.</param>
		/// <returns>The value including the prefix.</returns>
		internal static string EnsureStartsWith(this string value, string prefix)
		{
			return !string.IsNullOrEmpty(value) && value.StartsWith(prefix) ? value : string.Concat(prefix, value);
		}

		/// <summary>
		/// Ensure that a string ends with a given suffix.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="suffix">The sufix.</param>
		/// <returns>The value including the suffix.</returns>
		internal static string EnsureEndsWith(this string value, string suffix)
		{
			return !string.IsNullOrEmpty(value) && value.EndsWith(suffix) ? value : string.Concat(value, suffix);
		}

		/// <summary>
		/// Retrieve the permissions (AKA "scopes") assigned to the current user.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="excludeBillingScopes">Indicates if billing permissions should be excluded from the result.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An array of permisisons assigned to the current user.</returns>
		internal static async Task<string[]> GetCurrentScopes(this Pathoschild.Http.Client.IClient client, bool excludeBillingScopes, CancellationToken cancellationToken = default)
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

		internal static void AddPropertyIfValue(this JObject jsonObject, string propertyName, string value)
		{
			if (string.IsNullOrEmpty(value)) return;
			jsonObject.AddDeepProperty(propertyName, value);
		}

		internal static void AddPropertyIfValue<T>(this JObject jsonObject, string propertyName, T value, JsonConverter converter = null)
		{
			if (EqualityComparer<T>.Default.Equals(value, default)) return;

			var jsonSerializer = new JsonSerializer();
			if (converter != null)
			{
				jsonSerializer.Converters.Add(converter);
			}

			jsonObject.AddDeepProperty(propertyName, JToken.FromObject(value, jsonSerializer));
		}

		internal static void AddPropertyIfValue<T>(this JObject jsonObject, string propertyName, IEnumerable<T> value, JsonConverter converter = null)
		{
			if (value == null || !value.Any()) return;

			var jsonSerializer = new JsonSerializer();
			if (converter != null)
			{
				jsonSerializer.Converters.Add(converter);
			}

			jsonObject.AddDeepProperty(propertyName, JArray.FromObject(value.ToArray(), jsonSerializer));
		}

		internal static void AddPropertyIfValue<T>(this JObject jsonObject, string propertyName, Parameter<T> parameter, JsonConverter converter = null)
		{
			var jsonSerializer = new JsonSerializer();
			if (converter != null)
			{
				jsonSerializer.Converters.Add(converter);
			}

			AddPropertyIfValue(jsonObject, propertyName, parameter, value => JToken.FromObject(value, jsonSerializer));
		}

		internal static void AddPropertyIfValue<T>(this JObject jsonObject, string propertyName, Parameter<IEnumerable<T>> parameter, JsonConverter converter = null)
		{
			var jsonSerializer = new JsonSerializer();
			if (converter != null)
			{
				jsonSerializer.Converters.Add(converter);
			}

			AddPropertyIfValue(jsonObject, propertyName, parameter, value => JArray.FromObject(value.ToArray()));
		}

		internal static void AddPropertyIfEnumValue<T>(this JObject jsonObject, string propertyName, Parameter<T> parameter, JsonConverter converter = null)
		{
			var jsonSerializer = new JsonSerializer();
			if (converter != null)
			{
				jsonSerializer.Converters.Add(converter);
			}

			AddPropertyIfValue(jsonObject, propertyName, parameter, value => JToken.Parse(JsonConvert.SerializeObject(value)).ToString());
		}

		internal static void AddPropertyIfValue<T>(this JObject jsonObject, string propertyName, Parameter<T> parameter, Func<T, JToken> convertValueToJsonToken)
		{
			if (convertValueToJsonToken == null) throw new ArgumentNullException(nameof(convertValueToJsonToken));
			if (!parameter.HasValue) return;

			jsonObject.AddDeepProperty(propertyName, parameter.Value == null ? null : convertValueToJsonToken(parameter.Value));
		}

		internal static T GetPropertyValue<T>(this JToken item, string name, T defaultValue = default)
		{
			if (item[name] == null) return defaultValue;
			return item[name].Value<T>();
		}

		internal static async Task<TResult[]> ForEachAsync<T, TResult>(this IEnumerable<T> items, Func<T, Task<TResult>> action, int maxDegreeOfParalellism)
		{
			var allTasks = new List<Task<TResult>>();
			using (var throttler = new SemaphoreSlim(initialCount: maxDegreeOfParalellism))
			{
				foreach (var item in items)
				{
					await throttler.WaitAsync();
					allTasks.Add(
						Task.Run(async () =>
						{
							try
							{
								return await action(item).ConfigureAwait(false);
							}
							finally
							{
								throttler.Release();
							}
						}));
				}

				var results = await Task.WhenAll(allTasks).ConfigureAwait(false);
				return results;
			}
		}

		internal static async Task ForEachAsync<T>(this IEnumerable<T> items, Func<T, Task> action, int maxDegreeOfParalellism)
		{
			var allTasks = new List<Task>();
			using (var throttler = new SemaphoreSlim(initialCount: maxDegreeOfParalellism))
			{
				foreach (var item in items)
				{
					await throttler.WaitAsync();
					allTasks.Add(
						Task.Run(async () =>
						{
							try
							{
								await action(item).ConfigureAwait(false);
							}
							finally
							{
								throttler.Release();
							}
						}));
				}

				await Task.WhenAll(allTasks).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Gets the attribute of the specified type.
		/// </summary>
		/// <typeparam name="T">The type of the desired attribute.</typeparam>
		/// <param name="enumVal">The enum value.</param>
		/// <returns>The attribute.</returns>
		internal static T GetAttributeOfType<T>(this Enum enumVal)
			where T : Attribute
		{
			return enumVal.GetType()
				.GetTypeInfo()
				.DeclaredMembers
				.SingleOrDefault(x => x.Name == enumVal.ToString())
				?.GetCustomAttribute<T>(false);
		}

		/// <summary>
		/// Indicates if an object contain a numerical value.
		/// </summary>
		/// <param name="value">The object.</param>
		/// <returns>A boolean indicating if the object contains a numerical value.</returns>
		internal static bool IsNumber(this object value)
		{
			return value is sbyte
				   || value is byte
				   || value is short
				   || value is ushort
				   || value is int
				   || value is uint
				   || value is long
				   || value is ulong
				   || value is float
				   || value is double
				   || value is decimal;
		}

		/// <summary>
		/// Returns the first value for a specified header stored in the System.Net.Http.Headers.HttpHeaderscollection.
		/// </summary>
		/// <param name="headers">The HTTP headers.</param>
		/// <param name="name">The specified header to return value for.</param>
		/// <returns>A string.</returns>
		internal static string GetValue(this HttpHeaders headers, string name)
		{
			if (headers == null) return null;

			if (headers.TryGetValues(name, out IEnumerable<string> values))
			{
				return values.FirstOrDefault();
			}

			return null;
		}

		internal static void CheckForSendGridErrors(this IResponse response)
		{
			var (isError, errorMessage) = GetErrorMessage(response.Message).GetAwaiter().GetResult();
			if (!isError) return;

			var diagnosticId = response.Message.RequestMessage.Headers.GetValue(DiagnosticHandler.DIAGNOSTIC_ID_HEADER_NAME);
			if (DiagnosticHandler.DiagnosticsInfo.TryGetValue(diagnosticId, out (WeakReference<HttpRequestMessage> RequestReference, string Diagnostic, long RequestTimeStamp, long ResponseTimestamp) diagnosticInfo))
			{
				throw new SendGridException(errorMessage, response.Message, diagnosticInfo.Diagnostic);
			}
			else
			{
				throw new SendGridException(errorMessage, response.Message, "Diagnostic log unavailable");
			}
		}

		internal static IEnumerable<KeyValuePair<string, string>> ParseQuerystring(this Uri uri)
		{
			var querystringParameters = uri
				.Query.TrimStart('?')
				.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(value => value.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
				.Select(splitValue =>
				{
					if (splitValue.Length == 1)
					{
						return new KeyValuePair<string, string>(splitValue[0].Trim(), null);
					}
					else
					{
						return new KeyValuePair<string, string>(splitValue[0].Trim(), splitValue[1].Trim());
					}
				});

			return querystringParameters;
		}

		internal static void AddDeepProperty(this JObject jsonObject, string propertyName, JToken value)
		{
			var separatorLocation = propertyName.IndexOf('/');

			if (separatorLocation == -1)
			{
				jsonObject.Add(propertyName, value);
			}
			else
			{
				var name = propertyName.Substring(0, separatorLocation);
				var childrenName = propertyName.Substring(separatorLocation + 1);

				var property = jsonObject.Value<JObject>(name);
				if (property == null)
				{
					property = new JObject();
					jsonObject.Add(name, property);
				}

				property.AddDeepProperty(childrenName, value);
			}
		}

		/// <summary>Asynchronously converts the JSON encoded content and converts it to an object of the desired type.</summary>
		/// <typeparam name="T">The response model to deserialize into.</typeparam>
		/// <param name="httpContent">The content.</param>
		/// <param name="propertyName">The name of the JSON property (or null if not applicable) where the desired data is stored.</param>
		/// <param name="throwIfPropertyIsMissing">Indicates if an exception should be thrown when the specified JSON property is missing from the response.</param>
		/// <param name="jsonConverter">Converter that will be used during deserialization.</param>
		/// <returns>Returns the strongly typed object.</returns>
		/// <exception cref="SendGridException">An error occurred processing the response.</exception>
		internal static async Task<T> AsSendGridObject<T>(this HttpContent httpContent, string propertyName = null, bool throwIfPropertyIsMissing = true, JsonConverter jsonConverter = null)
		{
			var responseContent = await httpContent.ReadAsStringAsync(null).ConfigureAwait(false);

			var serializer = new JsonSerializer();
			if (jsonConverter != null) serializer.Converters.Add(jsonConverter);

			if (!string.IsNullOrEmpty(propertyName))
			{
				var jObject = JObject.Parse(responseContent);
				var jProperty = jObject.Property(propertyName);
				if (jProperty == null)
				{
					if (throwIfPropertyIsMissing)
					{
						throw new ArgumentException($"The response does not contain a field called '{propertyName}'", nameof(propertyName));
					}
					else
					{
						return default;
					}
				}

				return jProperty.Value.ToObject<T>(serializer);
			}
			else if (typeof(T).IsArray)
			{
				return JArray.Parse(responseContent).ToObject<T>(serializer);
			}
			else
			{
				return JObject.Parse(responseContent).ToObject<T>(serializer);
			}
		}

		/// <summary>Asynchronously retrieve the JSON encoded content and converts it to a 'PaginatedResponse' object.</summary>
		/// <typeparam name="T">The response model to deserialize into.</typeparam>
		/// <param name="httpContent">The content.</param>
		/// <param name="propertyName">The name of the JSON property (or null if not applicable) where the desired data is stored.</param>
		/// <param name="jsonConverter">Converter that will be used during deserialization.</param>
		/// <returns>Returns the response body, or <c>null</c> if the response has no body.</returns>
		/// <exception cref="SendGridException">An error occurred processing the response.</exception>
		internal static async Task<PaginatedResponse<T>> AsPaginatedResponse<T>(this HttpContent httpContent, string propertyName, JsonConverter jsonConverter = null)
		{
			var responseContent = await httpContent.ReadAsStringAsync(null).ConfigureAwait(false);
			var jObject = JObject.Parse(responseContent);

			var metadata = jObject.Property("_metadata").Value.ToObject<PaginationMetadata>();

			var serializer = new JsonSerializer();
			if (jsonConverter != null) serializer.Converters.Add(jsonConverter);

			var jProperty = jObject.Property(propertyName);
			if (jProperty == null)
			{
				throw new ArgumentException($"The response does not contain a field called '{propertyName}'", nameof(propertyName));
			}

			var result = new PaginatedResponse<T>()
			{
				PreviousPageToken = metadata.PrevToken,
				CurrentPageToken = metadata.SelfToken,
				NextPageToken = metadata.NextToken,
				TotalRecords = metadata.Count,
				Records = jProperty.Value.ToObject<T[]>(serializer)
			};

			return result;
		}

		/// <summary>Get a raw JSON object representation of the response, which can also be accessed as a <c>dynamic</c> value.</summary>
		/// <param name="httpContent">The content.</param>
		/// <param name="propertyName">The name of the JSON property (or null if not applicable) where the desired data is stored.</param>
		/// <param name="throwIfPropertyIsMissing">Indicates if an exception should be thrown when the specified JSON property is missing from the response.</param>
		/// <returns>Returns the response body, or <c>null</c> if the response has no body.</returns>
		/// <exception cref="SendGridException">An error occurred processing the response.</exception>
		internal static async Task<JObject> AsRawJsonObject(this HttpContent httpContent, string propertyName = null, bool throwIfPropertyIsMissing = true)
		{
			var responseContent = await httpContent.ReadAsStringAsync(null).ConfigureAwait(false);

			if (!string.IsNullOrEmpty(propertyName))
			{
				var jObject = JObject.Parse(responseContent);
				var jProperty = jObject.Property(propertyName);
				if (jProperty == null)
				{
					if (throwIfPropertyIsMissing)
					{
						throw new ArgumentException($"The response does not contain a field called '{propertyName}'", nameof(propertyName));
					}
					else
					{
						return default;
					}
				}

				return (JObject)jProperty.Value;
			}
			else
			{
				return JObject.Parse(responseContent);
			}
		}

		private static async Task<(bool, string)> GetErrorMessage(HttpResponseMessage message)
		{
			// Assume there is no error
			var isError = false;

			// Default error message
			var errorMessage = $"{(int)message.StatusCode}: {message.ReasonPhrase}";

			/*
				In case of an error, the SendGrid API returns a JSON string that looks like this:
				{
					"errors": [
						{
							"message": "An error has occurred",
							"field": null,
							"help": null
						}
					]
				}

				The documentation says that it should look like this:
				{
					"errors": [
						{
							"message": <string>,
							"field": <string>,
							"error_id": <string>
						}
					]
				}

				The documentation for "Add or Update a Contact" under the "New Marketing Campaigns" section says that it looks like this:
				{
					"errors": [
						{
							"message": <string>,
							"field": <string>,
							"error_id": <string>,
							"parameter": <string>
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
						errorMessage = string.Join(Environment.NewLine, errorsArray.Select(error => error["message"].Value<string>()));
						isError = true;
					}
					else
					{
						// Check for the presence of property called 'error'
						var errorProperty = jObject["error"];
						if (errorProperty != null)
						{
							errorMessage = errorProperty.Value<string>();
							isError = true;
						}
					}
				}
				catch
				{
					// Intentionally ignore parsing errors
				}
			}

			return (isError, errorMessage);
		}
	}
}
