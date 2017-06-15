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
		/// <returns>The <see cref="InboundEmail"/></returns>
		public InboundEmail ParseInboundEmailWebhook(Stream stream)
		{
			// Parse the multipart content received from SendGrid
			var parser = new MultipartFormDataParser(stream);

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

			// Conbine the 'attachment-info' and Files into a single array or Attachments
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
						attachment.ContentType = file.ContentType;
						attachment.Data = file.Data;
					}
					return attachment;
				}).ToArray();

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

			// Convert the 'envelope' from a JSON string into a strongly typed object
			var envelope = JsonConvert.DeserializeObject<InboundEmailEnvelope>(parser.GetParameterValue("envelope", "{}"));

			// Convert the 'from' from a string into an email address
			var rawFrom = parser.GetParameterValue("from", string.Empty);
			var from = ParseEmailAddress(rawFrom);

			// Convert the 'to' from a string into an array of email addresses
			var rawTo = parser.GetParameterValue("to", string.Empty);
			var to = ParseEmailAddresses(rawTo);

			// Convert the 'cc' from a string into an array of email addresses
			var rawCc = parser.GetParameterValue("cc", string.Empty);
			var cc = ParseEmailAddresses(rawCc);

			// Arrange the InboundEmail
			var inboundEmail = new InboundEmail
			{
				Attachments = attachments,
				Charsets = charsets,
				Dkim = parser.GetParameterValue("dkim", null),
				Envelope = envelope,
				From = from,
				Headers = headers,
				Html = parser.GetParameterValue("html", null),
				SenderIp = parser.GetParameterValue("sender_ip", null),
				SpamReport = parser.GetParameterValue("spam_report", null),
				SpamScore = parser.GetParameterValue("spam_score", null),
				Spf = parser.GetParameterValue("SPF", null),
				Subject = parser.GetParameterValue("subject", null),
				Text = parser.GetParameterValue("text", null),
				To = to,
				Cc = cc
			};

			return inboundEmail;
		}

		#endregion

		#region PRIVATE METHODS

		private MailAddress[] ParseEmailAddresses(string rawEmailAddresses)
		{
			var rawEmails = rawEmailAddresses.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			var addresses = rawEmails.Select(rawEmail => ParseEmailAddress(rawEmail)).ToArray();
			return addresses;
		}

		private MailAddress ParseEmailAddress(string rawEmailAddress)
		{
			var pieces = rawEmailAddress.Split(new[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
			var email = pieces.Length == 2 ? pieces[1].Trim() : pieces[0].Trim();
			var name = pieces.Length == 2 ? pieces[0].Replace("\"", string.Empty).Trim() : string.Empty;
			return new MailAddress(email, name);
		}

		#endregion
	}
}
