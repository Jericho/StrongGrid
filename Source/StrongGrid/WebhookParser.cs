using StrongGrid.Json;
using StrongGrid.Models.Webhooks;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
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
	public class WebhookParser
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

		/// <summary>
		/// Parses the signed events webhook asynchronously.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="publicKey">Your public key. To obtain this value, see <see cref="StrongGrid.Resources.WebhookSettings.GetSignedEventsPublicKeyAsync"/>.</param>
		/// <param name="signature">The signature.</param>
		/// <param name="timestamp">The timestamp.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An array of <see cref="Event">events</see>.</returns>
		public async Task<Event[]> ParseSignedEventsWebhookAsync(Stream stream, string publicKey, string signature, string timestamp, CancellationToken cancellationToken = default)
		{
			string requestBody;
			using (var streamReader = new StreamReader(stream))
			{
#if NET7_0_OR_GREATER
				requestBody = await streamReader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
#else
				requestBody = await streamReader.ReadToEndAsync().ConfigureAwait(false);
#endif
			}

			var webHookEvents = ParseSignedEventsWebhook(requestBody, publicKey, signature, timestamp);
			return webHookEvents;
		}

		/// <summary>
		/// Parses the events webhook asynchronously.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An array of <see cref="Event">events</see>.</returns>
		public async Task<Event[]> ParseEventsWebhookAsync(Stream stream, CancellationToken cancellationToken = default)
		{
			var webHookEvents = await JsonSerializer.DeserializeAsync<Event[]>(stream, JsonFormatter.DeserializerOptions, cancellationToken).ConfigureAwait(false);
			return webHookEvents;
		}

		/// <summary>
		/// Parses the signed events webhook.
		/// </summary>
		/// <param name="requestBody">The content submitted by SendGrid's WebHook.</param>
		/// <param name="publicKey">Your public key. To obtain this value, <see cref="StrongGrid.Resources.WebhookSettings.GetSignedEventsPublicKeyAsync"/>.</param>
		/// <param name="signature">The signature.</param>
		/// <param name="timestamp">The timestamp.</param>
		/// <returns>An array of <see cref="Event">events</see>.</returns>
		public Event[] ParseSignedEventsWebhook(string requestBody, string publicKey, string signature, string timestamp)
		{
			if (string.IsNullOrEmpty(publicKey)) throw new ArgumentNullException(nameof(publicKey));
			if (string.IsNullOrEmpty(signature)) throw new ArgumentNullException(nameof(signature));
			if (string.IsNullOrEmpty(timestamp)) throw new ArgumentNullException(nameof(timestamp));

			// Decode the base64 encoded values
			var signatureBytes = Convert.FromBase64String(signature);
			var publicKeyBytes = Convert.FromBase64String(publicKey);

			// Must combine the timestamp and the payload
			var data = Encoding.UTF8.GetBytes(timestamp + requestBody);

			/*
				The 'ECDsa.ImportSubjectPublicKeyInfo' method was introduced in .NET core 3.0
				and the DSASignatureFormat enum was introduced in .NET 5.0.

				We can get rid of the 'ConvertECDSASignature' class and the Utils methods that
				convert public keys when we stop suporting .NET framework and .NET standard.
			*/

#if NET5_0_OR_GREATER
			// Verify the signature
			var eCDsa = ECDsa.Create();
			eCDsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);
			var verified = eCDsa.VerifyData(data, signatureBytes, HashAlgorithmName.SHA256, DSASignatureFormat.Rfc3279DerSequence);
#elif NET48_OR_GREATER || NETSTANDARD2_1
			// Convert the signature and public key provided by SendGrid into formats usable by the ECDsa .net crypto class
			var sig = ConvertECDSASignature.LightweightConvertSignatureFromX9_62ToISO7816_8(256, signatureBytes);
			var (x, y) = Utils.GetXYFromSecp256r1PublicKey(publicKeyBytes);

			// Verify the signature
			var eCDsa = ECDsa.Create();
			eCDsa.ImportParameters(new ECParameters
			{
				Curve = ECCurve.NamedCurves.nistP256, // aka secp256r1 aka prime256v1
				Q = new ECPoint
				{
					X = x,
					Y = y
				}
			});
			var verified = eCDsa.VerifyData(data, sig, HashAlgorithmName.SHA256);
#else
#error Unhandled TFM
#endif

			if (!verified)
			{
				throw new SecurityException("Webhook signature validation failed.");
			}

			var webHookEvents = ParseEventsWebhook(requestBody);
			return webHookEvents;
		}

		/// <summary>
		/// Parses the events webhook.
		/// </summary>
		/// <param name="requestBody">The content submitted by SendGrid's WebHook.</param>
		/// <returns>An array of <see cref="Event">events</see>.</returns>
		public Event[] ParseEventsWebhook(string requestBody)
		{
			var webHookEvents = JsonSerializer.Deserialize<Event[]>(requestBody, JsonFormatter.DeserializerOptions);
			return webHookEvents;
		}

		/// <summary>
		/// Parses the inbound email webhook asynchronously.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The <see cref="InboundEmail"/>.</returns>
		public async Task<InboundEmail> ParseInboundEmailWebhookAsync(Stream stream, CancellationToken cancellationToken = default)
		{
			// It's important to rewind the stream
			stream.Position = 0;

			// Asynchronously parse the multipart content received from SendGrid
			var parser = await SendGridMultipartFormDataParser.ParseAsync(stream, cancellationToken).ConfigureAwait(false);

			return ParseInboundEmail(parser);
		}

		/// <summary>
		/// Parses the inbound email webhook.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns>The <see cref="InboundEmail"/>.</returns>
		[Obsolete("Use the async version of this method, it can read the content of the stream much more efficiently.")]
		public InboundEmail ParseInboundEmailWebhook(Stream stream)
		{
			// It's important to rewind the stream
			stream.Position = 0;

			// Parse the multipart content received from SendGrid
			var parser = SendGridMultipartFormDataParser.Parse(stream);

			return ParseInboundEmail(parser);
		}

		#endregion

		#region PRIVATE METHODS

		private static InboundEmail ParseInboundEmail(SendGridMultipartFormDataParser parser)
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

			return inboundEmail;
		}

		#endregion
	}
}
