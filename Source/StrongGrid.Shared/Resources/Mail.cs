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
		/// Send email(s) over SendGrid’s v3 Web API
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
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
			data.Add("personalizations", JToken.FromObject(personalizations.ToArray()));
			data.Add("from", JToken.FromObject(from));
			if (replyTo != null) data.Add("reply_to", JToken.FromObject(replyTo));
			data.Add("subject", subject);
			data.Add("content", JToken.FromObject(contents.ToArray()));
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
