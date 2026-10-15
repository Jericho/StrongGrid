using HttpMultipartParser;
using Pathoschild.Http.Client;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
		/// <param name="options">Options to control behavior Converter during parsing.</param>
		/// <returns>Returns the strongly typed object.</returns>
		/// <exception cref="SendGridException">An error occurred processing the response.</exception>
		internal static Task<T> AsObject<T>(this IResponse response, string propertyName = null, bool throwIfPropertyIsMissing = true, JsonSerializerOptions options = null)
		{
			return response.Message.Content.AsObject<T>(propertyName, throwIfPropertyIsMissing, options);
		}

		/// <summary>Asynchronously retrieve the JSON encoded response body and convert it to an object of the desired type.</summary>
		/// <typeparam name="T">The response model to deserialize into.</typeparam>
		/// <param name="request">The request.</param>
		/// <param name="propertyName">The name of the JSON property (or null if not applicable) where the desired data is stored.</param>
		/// <param name="throwIfPropertyIsMissing">Indicates if an exception should be thrown when the specified JSON property is missing from the response.</param>
		/// <param name="options">Options to control behavior Converter during parsing.</param>
		/// <returns>Returns the strongly typed object.</returns>
		/// <exception cref="SendGridException">An error occurred processing the response.</exception>
		internal static async Task<T> AsObject<T>(this IRequest request, string propertyName = null, bool throwIfPropertyIsMissing = true, JsonSerializerOptions options = null)
		{
			var response = await request.AsResponse().ConfigureAwait(false);
			return await response.AsObject<T>(propertyName, throwIfPropertyIsMissing, options).ConfigureAwait(false);
		}

		/// <summary>Asynchronously retrieve the JSON encoded content and convert it to a 'PaginatedResponse' object.</summary>
		/// <typeparam name="T">The response model to deserialize into.</typeparam>
		/// <param name="response">The response.</param>
		/// <param name="propertyName">The name of the JSON property (or null if not applicable) where the desired data is stored.</param>
		/// <param name="options">Options to control behavior Converter during parsing.</param>
		/// <returns>Returns the paginated response.</returns>
		/// <exception cref="SendGridException">An error occurred processing the response.</exception>
		internal static Task<PaginatedResponse<T>> AsPaginatedResponse<T>(this IResponse response, string propertyName = null, JsonSerializerOptions options = null)
		{
			return response.Message.Content.AsPaginatedResponse<T>(propertyName, options);
		}

		/// <summary>Asynchronously retrieve the JSON encoded content and convert it to a 'PaginatedResponse' object.</summary>
		/// <typeparam name="T">The response model to deserialize into.</typeparam>
		/// <param name="request">The request.</param>
		/// <param name="propertyName">The name of the JSON property (or null if not applicable) where the desired data is stored.</param>
		/// <param name="options">Options to control behavior Converter during parsing.</param>
		/// <returns>Returns the paginated response.</returns>
		/// <exception cref="SendGridException">An error occurred processing the response.</exception>
		internal static async Task<PaginatedResponse<T>> AsPaginatedResponse<T>(this IRequest request, string propertyName = null, JsonSerializerOptions options = null)
		{
			var response = await request.AsResponse().ConfigureAwait(false);
			return await response.AsPaginatedResponse<T>(propertyName, options).ConfigureAwait(false);
		}

		/// <summary>Get a raw JSON document representation of the response.</summary>
		/// <exception cref="ApiException">An error occurred processing the response.</exception>
		internal static Task<JsonDocument> AsRawJsonDocument(this IResponse response, string propertyName = null, bool throwIfPropertyIsMissing = true)
		{
			return response.Message.Content.AsRawJsonDocument(propertyName, throwIfPropertyIsMissing);
		}

		/// <summary>Get a raw JSON document representation of the response.</summary>
		/// <exception cref="ApiException">An error occurred processing the response.</exception>
		internal static async Task<JsonDocument> AsRawJsonDocument(this IRequest request, string propertyName = null, bool throwIfPropertyIsMissing = true)
		{
			var response = await request.AsResponse().ConfigureAwait(false);
			return await response.AsRawJsonDocument(propertyName, throwIfPropertyIsMissing).ConfigureAwait(false);
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
				.AsObject<string[]>("scopes")
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

		internal static DiagnosticInfo GetDiagnosticInfo(this IResponse response)
		{
			var diagnosticId = response.Message.RequestMessage.Headers.GetValue(DiagnosticHandler.DIAGNOSTIC_ID_HEADER_NAME);
			DiagnosticHandler.DiagnosticsInfo.TryGetValue(diagnosticId, out DiagnosticInfo diagnosticInfo);
			return diagnosticInfo;
		}

		internal static async Task<(bool IsError, string Message)> GetErrorMessageAsync(this HttpResponseMessage message)
		{
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
					var rootJsonElement = JsonDocument.Parse(responseContent).RootElement;

					if (rootJsonElement.ValueKind == JsonValueKind.Object)
					{
						var foundErrors = rootJsonElement.TryGetProperty("errors", out JsonElement jsonErrors);
						var foundError = rootJsonElement.TryGetProperty("error", out JsonElement jsonError);

						// Check for the presence of property called 'errors'
						if (foundErrors && jsonErrors.ValueKind == JsonValueKind.Array)
						{
							var errors = jsonErrors.EnumerateArray()
								.Select(jsonElement => jsonElement.GetProperty("message").GetString())
								.ToArray();

							errorMessage = string.Join(Environment.NewLine, errors);
							return (true, errorMessage);
						}

						// Check for the presence of property called 'error'
						else if (foundError)
						{
							errorMessage = jsonError.GetString();
							return (true, errorMessage);
						}
					}
				}
				catch
				{
					// Intentionally ignore parsing errors
				}
			}

			return (!message.IsSuccessStatusCode, errorMessage);
		}

		internal static T ToObject<T>(this JsonElement element, JsonSerializerOptions options = null)
		{
			return JsonSerializer.Deserialize<T>(element.GetRawText(), options ?? JsonFormatter.DeserializerOptions);
		}

		internal static StrongGridJsonObject ToStrongGridJsonObject<T>(this T source, bool ignoreDefault = true)
		{
			var jsonObject = new StrongGridJsonObject();
			foreach (var property in source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				var propertyName = ((JsonPropertyNameAttribute)property.GetCustomAttribute(typeof(JsonPropertyNameAttribute))).Name;
				var propertyValue = property.GetValue(source);

				jsonObject.AddProperty(propertyName, propertyValue, ignoreDefault);
			}

			return jsonObject;
		}

		/// <summary>Asynchronously converts the JSON encoded content and convert it to an object of the desired type.</summary>
		/// <typeparam name="T">The response model to deserialize into.</typeparam>
		/// <param name="httpContent">The content.</param>
		/// <param name="propertyName">The name of the JSON property (or null if not applicable) where the desired data is stored.</param>
		/// <param name="throwIfPropertyIsMissing">Indicates if an exception should be thrown when the specified JSON property is missing from the response.</param>
		/// <param name="options">Options to control behavior Converter during parsing.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>Returns the strongly typed object.</returns>
		/// <exception cref="SendGridException">An error occurred processing the response.</exception>
		private static async Task<T> AsObject<T>(this HttpContent httpContent, string propertyName = null, bool throwIfPropertyIsMissing = true, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
		{
			var responseContent = await httpContent.ReadAsStringAsync(null, cancellationToken).ConfigureAwait(false);

			if (string.IsNullOrEmpty(propertyName))
			{
				return JsonSerializer.Deserialize<T>(responseContent, options ?? JsonFormatter.DeserializerOptions);
			}

			var jsonDoc = JsonDocument.Parse(responseContent, (JsonDocumentOptions)default);
			if (jsonDoc.RootElement.TryGetProperty(propertyName, out JsonElement property))
			{
				return property.ToObject<T>(options);
			}
			else if (throwIfPropertyIsMissing)
			{
				throw new ArgumentException($"The response does not contain a field called '{propertyName}'", nameof(propertyName));
			}
			else
			{
				return default;
			}
		}

		/// <summary>Get a raw JSON object representation of the response.</summary>
		/// <param name="httpContent">The content.</param>
		/// <param name="propertyName">The name of the JSON property (or null if not applicable) where the desired data is stored.</param>
		/// <param name="throwIfPropertyIsMissing">Indicates if an exception should be thrown when the specified JSON property is missing from the response.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>Returns the response body, or <c>null</c> if the response has no body.</returns>
		/// <exception cref="SendGridException">An error occurred processing the response.</exception>
		private static async Task<JsonDocument> AsRawJsonDocument(this HttpContent httpContent, string propertyName = null, bool throwIfPropertyIsMissing = true, CancellationToken cancellationToken = default)
		{
			var responseContent = await httpContent.ReadAsStringAsync(null, cancellationToken).ConfigureAwait(false);

			var jsonDoc = JsonDocument.Parse(responseContent, (JsonDocumentOptions)default);

			if (string.IsNullOrEmpty(propertyName))
			{
				return jsonDoc;
			}

			if (jsonDoc.RootElement.TryGetProperty(propertyName, out JsonElement property))
			{
				var propertyContent = property.GetRawText();
				return JsonDocument.Parse(propertyContent, (JsonDocumentOptions)default);
			}
			else if (throwIfPropertyIsMissing)
			{
				throw new ArgumentException($"The response does not contain a field called '{propertyName}'", nameof(propertyName));
			}
			else
			{
				return default;
			}
		}

		/// <summary>Asynchronously retrieve the JSON encoded content and convert it to a 'PaginatedResponse' object.</summary>
		/// <typeparam name="T">The response model to deserialize into.</typeparam>
		/// <param name="httpContent">The content.</param>
		/// <param name="propertyName">The name of the JSON property (or null if not applicable) where the desired data is stored.</param>
		/// <param name="options">Options to control behavior Converter during parsing.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>Returns the response body, or <c>null</c> if the response has no body.</returns>
		/// <exception cref="SendGridException">An error occurred processing the response.</exception>
		private static async Task<PaginatedResponse<T>> AsPaginatedResponse<T>(this HttpContent httpContent, string propertyName, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
		{
			var jsonDocument = await httpContent.AsRawJsonDocument(null, false, cancellationToken).ConfigureAwait(false);
			var metadataProperty = jsonDocument.RootElement.GetProperty("_metadata");
			var metadata = metadataProperty.ToObject<PaginationMetadata>(options);

			if (!jsonDocument.RootElement.TryGetProperty(propertyName, out JsonElement jProperty))
			{
				throw new ArgumentException($"The response does not contain a field called '{propertyName}'", nameof(propertyName));
			}

			var result = new PaginatedResponse<T>()
			{
				PreviousPageToken = metadata.PrevToken,
				CurrentPageToken = metadata.SelfToken,
				NextPageToken = metadata.NextToken,
				TotalRecords = metadata.Count,
				Records = jProperty.ToObject<T[]>(options) ?? Array.Empty<T>()
			};

			return result;
		}

	}
}
