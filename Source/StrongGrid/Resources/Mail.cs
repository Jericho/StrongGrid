using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Models;
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
	/// <seealso cref="StrongGrid.Resources.IMail" />
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Mail/index.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public class Mail : IMail
	{
		// SendGrid doesn't allow emails that exceed 30MB
		private const int MAX_EMAIL_SIZE = 30 * 1024 * 1024;

		private const string _endpoint = "mail";

		// The following values are documented here: https://docs.microsoft.com/en-us/openspecs/exchange_server_protocols/ms-oxcmail/2bb19f1b-b35e-4966-b1cb-1afd044e83ab
		// X-Priority:
		//   - a number from 1 to 5 where 1 corresponds to "high" and 5 corresponds to "low"
		// Priority:
		//   - Urgent
		//   - Normal
		//   - Non-Urgent
		// Importance:
		//   - High
		//   - Normal
		//   - Low
		// X-MSMail-Priority:
		//   - High
		//   - Normal
		//   - Low
		private static readonly IDictionary<MailPriority, KeyValuePair<string, string>[]> _priorityHeaders = new Dictionary<MailPriority, KeyValuePair<string, string>[]>()
		{
			{
				MailPriority.High,
				new[]
				{
					new KeyValuePair<string, string>("X-Priority", "1"),
					new KeyValuePair<string, string>("Priority", "urgent"),
					new KeyValuePair<string, string>("Importance", "high"),
					new KeyValuePair<string, string>("X-MSMail-Priority", "high")
				}
			},
			{
				MailPriority.Normal,
				new[]
				{
					new KeyValuePair<string, string>("X-Priority", "3"),
					new KeyValuePair<string, string>("Priority", "normal"),
					new KeyValuePair<string, string>("Importance", "normal"),
					new KeyValuePair<string, string>("X-MSMail-Priority", "normal")
				}
			},
			{
				MailPriority.Low,
				new[]
				{
					new KeyValuePair<string, string>("X-Priority", "5"),
					new KeyValuePair<string, string>("Priority", "non-urgent"),
					new KeyValuePair<string, string>("Importance", "low"),
					new KeyValuePair<string, string>("X-MSMail-Priority", "low")
				}
			}
		};

		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Mail" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal Mail(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Send email(s) over SendGrid’s v3 Web API.
		/// </summary>
		/// <param name="personalizations">The personalizations.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="contents">The contents.</param>
		/// <param name="from">From.</param>
		/// <param name="replyTo">The reply to.</param>
		/// <param name="attachments">The attachments.</param>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customArgs">The custom arguments.</param>
		/// <param name="sendAt">The send at.</param>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="unsubscribeOptions">The unsubscribe options.</param>
		/// <param name="ipPoolName">Name of the ip pool.</param>
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="trackingSettings">The tracking settings.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The message id.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">Too many recipients.</exception>
		/// <exception cref="Exception">Email exceeds the size limit.</exception>
		public async Task<string> SendAsync(
			IEnumerable<MailPersonalization> personalizations,
			string subject,
			IEnumerable<MailContent> contents,
			MailAddress from,
			MailAddress replyTo = null,
			IEnumerable<Attachment> attachments = null,
			string templateId = null,
			IEnumerable<KeyValuePair<string, string>> headers = null,
			IEnumerable<string> categories = null,
			IEnumerable<KeyValuePair<string, string>> customArgs = null,
			DateTime? sendAt = null,
			string batchId = null,
			UnsubscribeOptions unsubscribeOptions = null,
			string ipPoolName = null,
			MailSettings mailSettings = null,
			TrackingSettings trackingSettings = null,
			MailPriority priority = MailPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			if (_client?.BaseClient?.DefaultRequestHeaders?.Authorization?.Scheme?.Equals("Basic", StringComparison.OrdinalIgnoreCase) ?? false)
			{
				const string errorMessage = "SendGrid does not support Basic authentication when sending transactional emails.";
				const string diagnosticLog = "This request was not dispatched to SendGrid because the exception returned by their API in this scenario is not clear: 'Permission denied, wrong credentials'.";
				throw new SendGridException(errorMessage, null, diagnosticLog);
			}

			if (personalizations == null || !personalizations.Any())
			{
				throw new ArgumentNullException(nameof(personalizations));
			}

			// This comparer is used to perform case-insensitive comparisons of email addresses
			var emailAddressComparer = new LambdaComparer<MailAddress>((address1, address2) => address1.Email.Equals(address2.Email, StringComparison.OrdinalIgnoreCase));

			// It's important to make a copy of the personalizations to ensure we don't modify the original array
			var personalizationsCopy = personalizations.Where(p => p != null).ToArray();
			foreach (var personalization in personalizationsCopy)
			{
				// Make sure the arrays are not null otherwise Linq's 'Except' method will throw a ArgumentNull exception (See GH-286).
				if (personalization.To == null) personalization.To = Array.Empty<MailAddress>();
				if (personalization.Cc == null) personalization.Cc = Array.Empty<MailAddress>();
				if (personalization.Bcc == null) personalization.Bcc = Array.Empty<MailAddress>();

				// Avoid duplicate addresses. This is important because SendGrid does not throw any
				// exception when a recipient is duplicated (which gives you the impression the email
				// was sent) but it does not actually send the email
				personalization.To = personalization.To
					.Distinct(emailAddressComparer)
					.ToArray();
				personalization.Cc = personalization.Cc
					.Distinct(emailAddressComparer)
					.Except(personalization.To, emailAddressComparer)
					.ToArray();
				personalization.Bcc = personalization.Bcc
					.Distinct(emailAddressComparer)
					.Except(personalization.To, emailAddressComparer)
					.Except(personalization.Cc, emailAddressComparer)
					.ToArray();

				// SendGrid doesn't like empty arrays
				if (!personalization.To.Any()) personalization.To = null;
				if (!personalization.Cc.Any()) personalization.Cc = null;
				if (!personalization.Bcc.Any()) personalization.Bcc = null;

				// Surround recipient names with double-quotes if necessary
				personalization.To = EnsureRecipientsNamesAreQuoted(personalization.To);
				personalization.Cc = EnsureRecipientsNamesAreQuoted(personalization.Cc);
				personalization.Bcc = EnsureRecipientsNamesAreQuoted(personalization.Bcc);
			}

			// The total number of recipients must be less than 1000. This includes all recipients defined within the to, cc, and bcc parameters, across each object that you include in the personalizations array.
			var numberOfRecipients = personalizationsCopy.Sum(p => p.To?.Count(r => r != null) ?? 0);
			numberOfRecipients += personalizationsCopy.Sum(p => p.Cc?.Count(r => r != null) ?? 0);
			numberOfRecipients += personalizationsCopy.Sum(p => p.Bcc?.Count(r => r != null) ?? 0);
			if (numberOfRecipients >= 1000) throw new ArgumentOutOfRangeException(nameof(numberOfRecipients), numberOfRecipients, "The total number of recipients must be less than 1000");

			var isDynamicTemplate = Template.IsDynamic(templateId);
			var personalizationConverter = new MailPersonalizationConverter(isDynamicTemplate);

			var data = new JObject();
			data.AddPropertyIfValue("from", from);
			data.AddPropertyIfValue("reply_to", replyTo);
			data.AddPropertyIfValue("subject", subject);
			data.AddPropertyIfValue("content", contents);
			data.AddPropertyIfValue("attachments", attachments);
			data.AddPropertyIfValue("template_id", templateId);
			data.AddPropertyIfValue("categories", categories);
			data.AddPropertyIfValue("send_at", sendAt?.ToUnixTime());
			data.AddPropertyIfValue("batch_id", batchId);
			data.AddPropertyIfValue("asm", unsubscribeOptions);
			data.AddPropertyIfValue("ip_pool_name", ipPoolName);
			data.AddPropertyIfValue("mail_settings", mailSettings);
			data.AddPropertyIfValue("tracking_settings", trackingSettings);
			data.AddPropertyIfValue("personalizations", personalizationsCopy, personalizationConverter);

			if (_priorityHeaders.TryGetValue(priority, out KeyValuePair<string, string>[] priorityHeaders))
			{
				headers = (headers ?? Array.Empty<KeyValuePair<string, string>>()).Concat(priorityHeaders);
			}
			else
			{
				throw new Exception($"{priority} is an unknown priority");
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

			// SendGrid does not allow emails that exceed 30MB
			var contentSize = JsonConvert.SerializeObject(data, Formatting.None).Length;
			if (contentSize > MAX_EMAIL_SIZE) throw new Exception("Email exceeds the size limit");

			var response = await _client
				.PostAsync($"{_endpoint}/send")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsResponse()
				.ConfigureAwait(false);

			if (response.Message.Headers.TryGetValues("X-Message-Id", out IEnumerable<string> values))
			{
				return values?.FirstOrDefault();
			}

			return null;
		}

		/// <summary>
		/// If a recipient name contains a comma or a semi-colon, SendGrid requires that it be surrounded by double-quotes.
		/// </summary>
		/// <param name="recipients">The array of recipients.</param>
		/// <returns>An aray of recipient where their names have been quoted, if necessary.</returns>
		private static MailAddress[] EnsureRecipientsNamesAreQuoted(MailAddress[] recipients)
		{
			if (recipients == null) return null;

			var recipientsWithName = recipients.Where(recipient => !string.IsNullOrEmpty(recipient?.Name));
			var recipientsWithoutName = recipients.Except(recipientsWithName);

			var recipientsNameContainsComma = recipientsWithName.Where(recipient => recipient.Name.Contains(';') || recipient.Name.Contains(','));
			var recipientsNameDoesNotContainComma = recipientsWithName.Except(recipientsNameContainsComma);

			return recipientsNameContainsComma
				.Select(recipient => new MailAddress(recipient.Email, recipient.Name.EnsureStartsWith("\"").EnsureEndsWith("\"")))
				.Union(recipientsNameDoesNotContainComma)
				.Union(recipientsWithoutName)
				.ToArray();
		}
	}
}
