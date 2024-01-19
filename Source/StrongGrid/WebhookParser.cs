using MimeKit;
using StrongGrid.Json;
using StrongGrid.Models.Webhooks;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid
{
	/// <summary>
	/// Allows parsing of information posted from SendGrid.
	/// This parser supports both 'Events' and 'Inbound emails'.
	/// </summary>
	public class WebhookParser : IWebhookParser
	{
		#region PROPERTIES

		/// <summary>
		/// The name of the HTTP header where SendGrid stores the webhook signature value.
		/// </summary>
		public const string SIGNATURE_HEADER_NAME = "X-Twilio-Email-Event-Webhook-Signature";

		/// <summary>
		/// The name of the HTTP header where SendGrid stores the webhook timestamp value.
		/// </summary>
		public const string TIMESTAMP_HEADER_NAME = "X-Twilio-Email-Event-Webhook-Timestamp";

		#endregion

		#region CTOR

#if NETSTANDARD2_1 || NET5_0_OR_GREATER
		static WebhookParser()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		}
#endif

		#endregion

		#region PUBLIC METHODS

		/// <inheritdoc/>
		public async Task<Event[]> ParseEventsWebhookAsync(Stream stream, CancellationToken cancellationToken = default)
		{
			var webHookEvents = await JsonSerializer.DeserializeAsync<Event[]>(stream, JsonFormatter.DeserializerOptions, cancellationToken).ConfigureAwait(false);
			return webHookEvents;
		}

		/// <inheritdoc/>
		public Event[] ParseEventsWebhook(string requestBody)
		{
			var webHookEvents = JsonSerializer.Deserialize<Event[]>(requestBody, JsonFormatter.DeserializerOptions);
			return webHookEvents;
		}

		/// <inheritdoc/>
		public async Task<InboundEmail> ParseInboundEmailWebhookAsync(Stream stream, CancellationToken cancellationToken = default)
		{
			// It's important to rewind the stream (but only if permitted)
			if (stream.CanSeek) stream.Position = 0;

			// Asynchronously parse the multipart content received from SendGrid
			var parser = await SendGridMultipartFormDataParser.ParseAsync(stream, cancellationToken).ConfigureAwait(false);

			return await ParseInboundEmailAsync(parser, cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		[Obsolete("Use the async version of this method, it can read the content of the stream much more efficiently.")]
		public InboundEmail ParseInboundEmailWebhook(Stream stream)
		{
			// It's important to rewind the stream (but only if permitted)
			if (stream.CanSeek) stream.Position = 0;

			// Parse the multipart content received from SendGrid
			var parser = SendGridMultipartFormDataParser.Parse(stream);

			return ParseInboundEmailAsync(parser, CancellationToken.None).GetAwaiter().GetResult();
		}

		#endregion

		#region PRIVATE METHODS

		private static async Task<InboundEmail> ParseInboundEmailAsync(SendGridMultipartFormDataParser parser, CancellationToken cancellationToken)
		{
			// Convert the 'headers' from a string into array of KeyValuePair
			var headers = parser
					.GetParameterValue("headers", string.Empty)
					.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
					.Select(header =>
					{
						var splitHeader = header.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
						var key = splitHeader[0];
						var value = splitHeader.Length >= 2 ? splitHeader[1] : null;
						return new KeyValuePair<string, string>(key, value);
					}).ToArray();

			// Raw email
			var rawEmail = parser.GetParameterValue("email", string.Empty);

			// Combine the 'attachment-info' and Files into an array of Attachments
			var attachmentInfoJsonDoc = JsonDocument.Parse(parser.GetParameterValue("attachment-info", "{}"));
			var attachments = attachmentInfoJsonDoc.RootElement.EnumerateObject()
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
			var envelope = JsonSerializer.Deserialize<InboundEmailEnvelope>(parser.GetParameterValue("envelope", "{}"), JsonFormatter.DeserializerOptions);

			// Convert the 'from' from a string into an email address
			var rawFrom = parser.GetParameterValue("from", string.Empty);
			var from = MailAddressParser.ParseEmailAddress(rawFrom);

			// Convert the 'to' from a string into an array of email addresses
			var rawTo = parser.GetParameterValue("to", string.Empty);
			var to = MailAddressParser.ParseEmailAddresses(rawTo);

			// Convert the 'cc' from a string into an array of email addresses
			var rawCc = parser.GetParameterValue("cc", string.Empty);
			var cc = MailAddressParser.ParseEmailAddresses(rawCc);

			// Arrange the InboundEmail
			var inboundEmail = new InboundEmail
			{
				Attachments = attachments,
				Charsets = parser.Charsets.ToArray(),
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
				Cc = cc,
				RawEmail = rawEmail
			};

			// If the format of the payload is "raw", we can parse the MIME content and derive additional values like attachments, Html, Text, etc.
			if (!string.IsNullOrEmpty(inboundEmail.RawEmail))
			{
				using var ms = new MemoryStream(Encoding.UTF8.GetBytes(inboundEmail.RawEmail));
				using var message = await MimeMessage.LoadAsync(ms, cancellationToken).ConfigureAwait(false);

				var mimeInlineContent = message.BodyParts.Where(part => part.ContentDisposition?.Disposition?.Equals("inline", StringComparison.OrdinalIgnoreCase) ?? false);
				var convertedAttachments = await message.Attachments.Union(mimeInlineContent)
					.ForEachAsync((mimeEntity, index) => ConvertMimeEntityToAttachmentAsync(mimeEntity, index, cancellationToken))
					.ConfigureAwait(false);

				inboundEmail.Html = message.HtmlBody;
				inboundEmail.Text = message.TextBody;
				inboundEmail.Headers = message.Headers.Select(mimeHeader => new KeyValuePair<string, string>(mimeHeader.Field, mimeHeader.Value)).ToArray();
				inboundEmail.Attachments = convertedAttachments;
			}

			return inboundEmail;
		}

		private static async Task<InboundEmailAttachment> ConvertMimeEntityToAttachmentAsync(MimeEntity mimeEntity, int index, CancellationToken cancellationToken)
		{
			var attachment = new InboundEmailAttachment
			{
				ContentId = mimeEntity.ContentId,
				ContentType = mimeEntity.ContentType.MimeType,
				Data = new MemoryStream(),
			};

			if (mimeEntity is MimePart mimePart)
			{
				attachment.FileName = mimePart.FileName;
				attachment.Name = mimePart.ContentType.Name;
				await mimePart.Content.DecodeToAsync(attachment.Data, cancellationToken).ConfigureAwait(false);
			}
			else if (mimeEntity is MessagePart mimeMessage)
			{
				var attachmentName = !string.IsNullOrEmpty(mimeMessage.Message.Subject) ? mimeMessage.Message.Subject : $"Attachment{index}";
				MimeTypes.TryGetExtension(mimeMessage.ContentType.MimeType, out var extension);
				attachment.FileName = $"{attachmentName}{extension}";
				attachment.Name = attachmentName;
				await mimeMessage.Message.WriteToAsync(attachment.Data, cancellationToken).ConfigureAwait(false);
			}
			else
			{
				throw new NotImplementedException($"Converting a MimeEntity type {mimeEntity.GetType().Name} to an attachment is not supported");
			}

			attachment.Data.Position = 0;

			return attachment;
		}

		#endregion
	}
}
