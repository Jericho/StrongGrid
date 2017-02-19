using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to send email over SendGrid’s Web API.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Mail/index.html
	/// </remarks>
	public class Mail
	{
		private const string _endpoint = "mail";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Mail" /> class.
		/// </summary>
		/// <param name="client">The HTTP client</param>
		public Mail(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Send an email to a single recipient
		/// </summary>
		/// <param name="to">To.</param>
		/// <param name="from">From.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="trackOpens">if set to <c>true</c> [track opens].</param>
		/// <param name="trackClicks">if set to <c>true</c> [track clicks].</param>
		/// <param name="subscriptionTracking">The subscription tracking.</param>
		/// <param name="replyTo">The reply to.</param>
		/// <param name="attachments">The attachments.</param>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="sections">The sections.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customArgs">The custom arguments.</param>
		/// <param name="sendAt">The send at.</param>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="unsubscribeOptions">The unsubscribe options.</param>
		/// <param name="ipPoolName">Name of the ip pool.</param>
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The message id.
		/// </returns>
		/// <remarks>
		/// This is a convenience method with simplified parameters.
		/// If you need more options, use the <see cref="SendAsync" /> method.
		/// </remarks>
		public Task<string> SendToSingleRecipientAsync(
			MailAddress to,
			MailAddress from,
			string subject,
			string htmlContent,
			string textContent,
			bool trackOpens = true,
			bool trackClicks = true,
			SubscriptionTrackingSettings subscriptionTracking = null,
			MailAddress replyTo = null,
			IEnumerable<Attachment> attachments = null,
			string templateId = null,
			IEnumerable<KeyValuePair<string, string>> sections = null,
			IEnumerable<KeyValuePair<string, string>> headers = null,
			IEnumerable<string> categories = null,
			IEnumerable<KeyValuePair<string, string>> customArgs = null,
			DateTime? sendAt = null,
			string batchId = null,
			UnsubscribeOptions unsubscribeOptions = null,
			string ipPoolName = null,
			MailSettings mailSettings = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			var recipients = new[] { to };
			return SendToMultipleRecipientsAsync(recipients, from, subject, htmlContent, textContent, trackOpens, trackClicks, subscriptionTracking, replyTo, attachments, templateId, sections, headers, categories, customArgs, sendAt, batchId, unsubscribeOptions, ipPoolName, mailSettings, cancellationToken);
		}

		/// <summary>
		/// Send the same email to multiple recipients
		/// </summary>
		/// <param name="recipients">The recipients.</param>
		/// <param name="from">From.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="trackOpens">if set to <c>true</c> [track opens].</param>
		/// <param name="trackClicks">if set to <c>true</c> [track clicks].</param>
		/// <param name="subscriptionTracking">The subscription tracking.</param>
		/// <param name="replyTo">The reply to.</param>
		/// <param name="attachments">The attachments.</param>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="sections">The sections.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customArgs">The custom arguments.</param>
		/// <param name="sendAt">The send at.</param>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="unsubscribeOptions">The unsubscribe options.</param>
		/// <param name="ipPoolName">Name of the ip pool.</param>
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The message id.
		/// </returns>
		/// <remarks>
		/// This is a convenience method with simplified parameters.
		/// If you need more options, use the <see cref="SendAsync" /> method.
		/// </remarks>
		public Task<string> SendToMultipleRecipientsAsync(
			IEnumerable<MailAddress> recipients,
			MailAddress from,
			string subject,
			string htmlContent,
			string textContent,
			bool trackOpens = true,
			bool trackClicks = true,
			SubscriptionTrackingSettings subscriptionTracking = null,
			MailAddress replyTo = null,
			IEnumerable<Attachment> attachments = null,
			string templateId = null,
			IEnumerable<KeyValuePair<string, string>> sections = null,
			IEnumerable<KeyValuePair<string, string>> headers = null,
			IEnumerable<string> categories = null,
			IEnumerable<KeyValuePair<string, string>> customArgs = null,
			DateTime? sendAt = null,
			string batchId = null,
			UnsubscribeOptions unsubscribeOptions = null,
			string ipPoolName = null,
			MailSettings mailSettings = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			var personalizations = recipients.Select(r => new MailPersonalization { To = new[] { r } });
			var contents = new[]
			{
				new MailContent("text/plain", textContent),
				new MailContent("text/html", htmlContent)
			};
			var trackingSettings = new TrackingSettings
			{
				ClickTracking = new ClickTrackingSettings
				{
					EnabledInHtmlContent = trackClicks,
					EnabledInTextContent = trackClicks
				},
				OpenTracking = new OpenTrackingSettings { Enabled = trackOpens },
				GoogleAnalytics = new GoogleAnalyticsSettings { Enabled = false },
				SubscriptionTracking = subscriptionTracking
			};

			return SendAsync(personalizations, subject, contents, from, replyTo, attachments, templateId, sections, headers, categories, customArgs, sendAt, batchId, unsubscribeOptions, ipPoolName, mailSettings, trackingSettings, cancellationToken);
		}

		/// <summary>
		/// Send email(s) over SendGrid’s v3 Web API
		/// </summary>
		/// <param name="personalizations">The personalizations.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="contents">The contents.</param>
		/// <param name="from">From.</param>
		/// <param name="replyTo">The reply to.</param>
		/// <param name="attachments">The attachments.</param>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="sections">The sections.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customArgs">The custom arguments.</param>
		/// <param name="sendAt">The send at.</param>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="unsubscribeOptions">The unsubscribe options.</param>
		/// <param name="ipPoolName">Name of the ip pool.</param>
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="trackingSettings">The tracking settings.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The message id.
		/// </returns>
		public async Task<string> SendAsync(
			IEnumerable<MailPersonalization> personalizations,
			string subject,
			IEnumerable<MailContent> contents,
			MailAddress from,
			MailAddress replyTo = null,
			IEnumerable<Attachment> attachments = null,
			string templateId = null,
			IEnumerable<KeyValuePair<string, string>> sections = null,
			IEnumerable<KeyValuePair<string, string>> headers = null,
			IEnumerable<string> categories = null,
			IEnumerable<KeyValuePair<string, string>> customArgs = null,
			DateTime? sendAt = null,
			string batchId = null,
			UnsubscribeOptions unsubscribeOptions = null,
			string ipPoolName = null,
			MailSettings mailSettings = null,
			TrackingSettings trackingSettings = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject();
			if (personalizations != null && personalizations.Any()) data.Add("personalizations", JToken.FromObject(personalizations.ToArray()));
			if (from != null) data.Add("from", JToken.FromObject(from));
			if (replyTo != null) data.Add("reply_to", JToken.FromObject(replyTo));
			if (!string.IsNullOrEmpty(subject)) data.Add("subject", subject);
			if (contents != null && contents.Any()) data.Add("content", JToken.FromObject(contents.ToArray()));
			if (attachments != null && attachments.Any()) data.Add("attachments", JToken.FromObject(attachments.ToArray()));
			if (!string.IsNullOrEmpty(templateId)) data.Add("template_id", templateId);
			if (categories != null && categories.Any()) data.Add("categories", JToken.FromObject(categories.ToArray()));
			if (sendAt.HasValue) data.Add("send_at", sendAt.Value.ToUnixTime());
			if (!string.IsNullOrEmpty(batchId)) data.Add("batch_id", batchId);
			if (unsubscribeOptions != null) data.Add("asm", JToken.FromObject(unsubscribeOptions));
			if (!string.IsNullOrEmpty(ipPoolName)) data.Add("ip_pool_name", ipPoolName);
			if (mailSettings != null) data.Add("mail_settings", JToken.FromObject(mailSettings));
			if (trackingSettings != null) data.Add("tracking_settings", JToken.FromObject(trackingSettings));

			if (sections != null && sections.Any())
			{
				var sctns = new JObject();
				foreach (var section in sections)
				{
					sctns.Add(section.Key, section.Value);
				}

				data.Add("sections", sctns);
			}

			if (headers != null && headers.Any())
			{
				var hdrs = new JObject();
				foreach (var header in headers)
				{
					hdrs.Add(header.Key, header.Value);
				}

				data.Add("headers", hdrs);
			}

			if (customArgs != null && customArgs.Any())
			{
				var args = new JObject();
				foreach (var customArg in customArgs)
				{
					args.Add(customArg.Key, customArg.Value);
				}

				data.Add("custom_args", args);
			}

			var response = await _client
				.PostAsync($"{_endpoint}/send")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsResponse()
				.ConfigureAwait(false);

			var messageId = (string)null;
			if (response.Message.Headers.Contains("X-Message-Id"))
			{
				messageId = response.Message.Headers.GetValues("X-Message-Id").FirstOrDefault();
			}

			return messageId;
		}
	}
}
