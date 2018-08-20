using HttpMultipartParser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StrongGrid.Models;
using StrongGrid.Models.Webhooks;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StrongGrid
{
	/// <summary>
	/// Allows parsing of information posted from SendGrid.
	/// This parser supports both 'Events' and 'Inbound emails'.
	/// </summary>
	public class WebhookParser
	{
		#region PUBLIC METHODS

		/// <summary>
		/// Parses the webhook events asynchronously.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns>An array of <see cref="Event">events</see>.</returns>
		public async Task<Event[]> ParseWebhookEventsAsync(Stream stream)
		{
			string requestBody;
			using (var streamReader = new StreamReader(stream))
			{
				requestBody = await streamReader.ReadToEndAsync().ConfigureAwait(false);
			}

			var webHookEvents = ParseWebhookEvents(requestBody);
			return webHookEvents;
		}

		/// <summary>
		/// Parses the webhook events.
		/// </summary>
		/// <param name="requestBody">The content submitted by Sendgrid's WebHook.</param>
		/// <returns>An array of <see cref="Event">events</see>.</returns>
		public Event[] ParseWebhookEvents(string requestBody)
		{
			var webHookEvents = JsonConvert.DeserializeObject<List<Event>>(requestBody, new WebHookEventConverter());
			return webHookEvents.ToArray();
		}

		/// <summary>
		/// Parses the inbound email webhook.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns>The <see cref="InboundEmail"/>.</returns>
		public InboundEmail ParseInboundEmailWebhook(Stream stream)
		{
			// Parse the multipart content received from SendGrid
			var parser = new MultipartFormDataParser(stream, Encoding.UTF8);

			// Convert the 'headers' from a string into array of KeyValuePair
			var rawHeaders = parser
				.GetParameterValue("headers", string.Empty)
				.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
			var headers = rawHeaders
				.Select(header =>
				{
					var splitHeader = header.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
					var key = splitHeader[0];
					var value = splitHeader.Length >= 2 ? splitHeader[1] : null;
					return new KeyValuePair<string, string>(key, value);
				}).ToArray();

			// Combine the 'attachment-info' and Files into an array of Attachments
			var attachmentInfoAsJObject = JObject.Parse(parser.GetParameterValue("attachment-info", "{}"));
			var attachments = attachmentInfoAsJObject
				.Properties()
				.Select(prop =>
				{
					var attachment = prop.Value.ToObject<InboundEmailAttachment>();
					attachment.Id = prop.Name;

					var file = parser.Files.FirstOrDefault(f => f.Name == prop.Name);
					if (file != null)
					{
						attachment.Data = file.Data;
						if (string.IsNullOrEmpty(attachment.ContentType)) attachment.ContentType = file.ContentType;
						if (string.IsNullOrEmpty(attachment.FileName)) attachment.FileName = file.FileName;
					}

					return attachment;
				}).ToArray();

			// Convert the 'envelope' from a JSON string into a strongly typed object
			var envelope = JsonConvert.DeserializeObject<InboundEmailEnvelope>(parser.GetParameterValue("envelope", "{}"));

			// Convert the 'charset' from a string into array of KeyValuePair
			var charsetsAsJObject = JObject.Parse(parser.GetParameterValue("charsets", "{}"));
			var charsets = charsetsAsJObject
				.Properties()
				.Select(prop =>
				{
					var key = prop.Name;
					var value = Encoding.GetEncoding(prop.Value.ToString());
					return new KeyValuePair<string, Encoding>(key, value);
				}).ToArray();

			// Create a dictionary of parsers, one parser for each desired encoding.
			// This is necessary because MultipartFormDataParser can only handle one
			// encoding and SendGrid can use different encodings for parameters such
			// as "from", "to", "text" and "html".
			var encodedParsers = charsets
				.Where(c => c.Value != Encoding.UTF8)
				.Select(c => c.Value)
				.Distinct()
				.Select(encoding =>
				{
					stream.Position = 0;
					return new
					{
						Encoding = encoding,
						Parser = new MultipartFormDataParser(stream, encoding)
					};
				})
				.Union(new[]
				{
					new { Encoding = Encoding.UTF8, Parser = parser }
				})
				.ToDictionary(ep => ep.Encoding, ep => ep.Parser);

			// Convert the 'from' from a string into an email address
			var rawFrom = GetEncodedValue("from", charsets, encodedParsers, string.Empty);
			var from = ParseEmailAddress(rawFrom);

			// Convert the 'to' from a string into an array of email addresses
			var rawTo = GetEncodedValue("to", charsets, encodedParsers, string.Empty);
			var to = ParseEmailAddresses(rawTo);

			// Convert the 'cc' from a string into an array of email addresses
			var rawCc = GetEncodedValue("cc", charsets, encodedParsers, string.Empty);
			var cc = ParseEmailAddresses(rawCc);

			// Arrange the InboundEmail
			var inboundEmail = new InboundEmail
			{
				Attachments = attachments,
				Charsets = charsets,
				Dkim = GetEncodedValue("dkim", charsets, encodedParsers, null),
				Envelope = envelope,
				From = from,
				Headers = headers,
				Html = GetEncodedValue("html", charsets, encodedParsers, null),
				SenderIp = GetEncodedValue("sender_ip", charsets, encodedParsers, null),
				SpamReport = GetEncodedValue("spam_report", charsets, encodedParsers, null),
				SpamScore = GetEncodedValue("spam_score", charsets, encodedParsers, null),
				Spf = GetEncodedValue("SPF", charsets, encodedParsers, null),
				Subject = GetEncodedValue("subject", charsets, encodedParsers, null),
				Text = GetEncodedValue("text", charsets, encodedParsers, null),
				To = to,
				Cc = cc
			};

			return inboundEmail;
		}

		#endregion

		#region PRIVATE METHODS

		private static MailAddress[] ParseEmailAddresses(string rawEmailAddresses)
		{
			// Split on commas that have an even number of double-quotes following them
			const string SPLIT_EMAIL_ADDRESSES = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";

			/*
				When we stop supporting .NET 4.5.2 we will be able to use the following:
				if (string.IsNullOrEmpty(rawEmailAddresses)) return Array.Empty<MailAddress>();
			*/
			if (string.IsNullOrEmpty(rawEmailAddresses)) return Enumerable.Empty<MailAddress>().ToArray();

			var rawEmails = Regex.Split(rawEmailAddresses, SPLIT_EMAIL_ADDRESSES);
			var addresses = rawEmails
				.Select(rawEmail => ParseEmailAddress(rawEmail))
				.Where(address => address != null)
				.ToArray();
			return addresses;
		}

		private static MailAddress ParseEmailAddress(string rawEmailAddress)
		{
			if (string.IsNullOrEmpty(rawEmailAddress)) return null;

			var pieces = rawEmailAddress.Split(new[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
			if (pieces.Length == 0) return null;
			var email = pieces.Length == 2 ? pieces[1].Trim() : pieces[0].Trim();
			var name = pieces.Length == 2 ? pieces[0].Replace("\"", string.Empty).Trim() : string.Empty;
			return new MailAddress(email, name);
		}

		private static Encoding GetEncoding(string parameterName, IEnumerable<KeyValuePair<string, Encoding>> charsets)
		{
			var encoding = charsets.Where(c => c.Key == parameterName);
			if (encoding.Any()) return encoding.First().Value;
			else return Encoding.UTF8;
		}

		private static MultipartFormDataParser GetEncodedParser(string parameterName, IEnumerable<KeyValuePair<string, Encoding>> charsets, IDictionary<Encoding, MultipartFormDataParser> encodedParsers)
		{
			var encoding = GetEncoding(parameterName, charsets);
			var parser = encodedParsers[encoding];
			return parser;
		}

		private static string GetEncodedValue(string parameterName, IEnumerable<KeyValuePair<string, Encoding>> charsets, IDictionary<Encoding, MultipartFormDataParser> encodedParsers, string defaultValue = null)
		{
			var parser = GetEncodedParser(parameterName, charsets, encodedParsers);
			var value = parser.GetParameterValue(parameterName, defaultValue);
			return value;
		}

		#endregion
	}
}
