using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	public class Mail
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Constructs the SendGrid Mail object.
		/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Mail/index.html
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public Mail(IClient client, string endpoint = "/mail")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Send an email to a single recipient
		/// </summary>
		/// <remarks>
		/// This is a convenience method with simplified parameters. 
		/// If you need more options, use the <see cref="SendAsync"/> method.
		/// </remarks>
		public Task SendToSingleRecipientAsync(
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
			DateTime? sendAt = null,
			string batchId = null,
			UnsubscribeOptions unsubscribeOptions = null,
			string ipPoolName = null,
			MailSettings mailSettings = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			var recipients = new[] { to };
			return SendToMultipleRecipientsAsync(recipients, from, subject, htmlContent, textContent, trackOpens, trackClicks, subscriptionTracking, replyTo, attachments, templateId, sections, headers, categories, sendAt, batchId, unsubscribeOptions, ipPoolName, mailSettings, cancellationToken);
		}

		/// <summary>
		/// Send the same email to multiple recipients
		/// </summary>
		/// <remarks>
		/// This is a convenience method with simplified parameters. 
		/// If you need more options, use the <see cref="SendAsync"/> method.
		/// </remarks>
		public Task SendToMultipleRecipientsAsync(
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
					Enabled = trackClicks,
					EnabledInTextContent = trackClicks
				},
				OpenTracking = new OpenTrackingSettings { Enabled = trackOpens },
				GoogleAnalytics = new GoogleAnalyticsSettings { Enabled = false },
				SubscriptionTracking = subscriptionTracking
			};

			return SendAsync(personalizations, subject, contents, from, replyTo, attachments, templateId, sections, headers, categories, sendAt, batchId, unsubscribeOptions, ipPoolName, mailSettings, trackingSettings, cancellationToken);
		}

		/// <summary>
		/// Send email(s) over SendGrid’s v3 Web API
		/// </summary>
		public async Task SendAsync(
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
			if (sections != null && sections.Any()) data.Add("sections", JToken.FromObject(sections.ToArray()));
			if (headers != null && headers.Any()) data.Add("headers", JToken.FromObject(headers.ToArray()));
			if (categories != null && categories.Any()) data.Add("categories", JToken.FromObject(categories.ToArray()));
			if (sendAt.HasValue) data.Add("send_at", sendAt.Value.ToUnixTime());
			if (!string.IsNullOrEmpty(batchId)) data.Add("batch_id", batchId);
			if (unsubscribeOptions != null) data.Add("asm", JToken.FromObject(unsubscribeOptions));
			if (!string.IsNullOrEmpty(ipPoolName)) data.Add("ip_pool_name", ipPoolName);
			if (mailSettings != null) data.Add("mail_settings", JToken.FromObject(mailSettings));
			if (trackingSettings != null) data.Add("tracking_settings", JToken.FromObject(trackingSettings));

			var response = await _client.PostAsync(string.Format("{0}/send", _endpoint), data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}
	}
}
