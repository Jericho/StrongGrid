using HttpMultipartParser;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using StrongGrid.Model.Webhooks;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Text;
using StrongGrid.Model;

namespace StrongGrid
{
	/// <summary>
	/// Allows parsing of information posted from SendGrid.
	/// This parser supports both 'Events' and 'Inbound emails'.
	/// </summary>
	public class WebhookParser
	{
		#region FIELDS

		#endregion

		#region PROPERTIES

		#endregion

		#region PUBLIC METHODS

		/// <summary>
		/// Parses the webhook events asynchronously.
		/// </summary>
		/// <param name="httpRequest">The HTTP request.</param>
		/// <returns>An array of <see cref="Event">events</see>.</returns>
		public Task<Event[]> ParseWebhookEventsAsync(HttpRequest httpRequest)
		{
			return ParseWebhookEventsAsync(httpRequest.Body);
		}

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

			var webHookEvents = JsonConvert.DeserializeObject<List<Event>>(requestBody, new WebHookEventConverter());
			return webHookEvents.ToArray();
		}

		/// <summary>
		/// Parses the inbound email webhook asynchronously.
		/// </summary>
		/// <param name="httpRequest">The HTTP request.</param>
		/// <returns>The <see cref="InboundEmail"/></returns>
		public InboundEmail ParseInboundEmailWebhook(HttpRequest httpRequest)
		{
			return ParseInboundEmailWebhook(httpRequest.Body);
		}

		/// <summary>
		/// Parses the inbound email webhook asynchronously.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns>The <see cref="InboundEmail"/></returns>
		public InboundEmail ParseInboundEmailWebhook(Stream stream)
		{
			// Parse the multipart content received from SendGrid
			var parser = new MultipartFormDataParser(stream);

			// Convert the 'headers' from a string into array of KeyValuePair
			var rawHeaders = parser
				.GetParameterValue("headers", "")
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

			// Convert the 'from' from a string into a strongly typed object
			var rawFrom = parser.GetParameterValue("from", "");
			var piecesFrom = rawFrom.Split(new[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
			var from = new MailAddress(piecesFrom[1], piecesFrom[0].Replace("\"", "").Trim());

			// Convert the 'to' from a string into a strongly typed object
			var rawTo = parser.GetParameterValue("to", "");
			var piecesTo = rawFrom.Split(new[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
			var to = new MailAddress(piecesFrom[1], piecesFrom[0].Replace("\"", "").Trim());

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
				To = to
			};

			return inboundEmail;
		}

		#endregion
	}
}
