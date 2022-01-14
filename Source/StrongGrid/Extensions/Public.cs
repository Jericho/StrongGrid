using StrongGrid.Models;
using StrongGrid.Models.Search;
using StrongGrid.Resources;
using StrongGrid.Utilities;
using StrongGrid.Warmup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid
{
	/// <summary>
	/// Public extension methods.
	/// </summary>
	public static class Public
	{
		/// <summary>
		/// Send an email to a single recipient without using a template (which means you must provide the subject, html content and text content).
		/// </summary>
		/// <param name="mailResource">The mail resource.</param>
		/// <param name="to">To.</param>
		/// <param name="from">From.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="trackOpens">if set to <c>true</c> [track opens].</param>
		/// <param name="trackClicks">if set to <c>true</c> [track clicks].</param>
		/// <param name="subscriptionTracking">The subscription tracking.</param>
		/// <param name="replyTo">The reply-to address.</param>
		/// <param name="attachments">The attachments.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customArgs">The custom arguments.</param>
		/// <param name="sendAt">The send at.</param>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="unsubscribeOptions">The unsubscribe options.</param>
		/// <param name="ipPoolName">Name of the ip pool.</param>
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The message id.
		/// </returns>
		/// <remarks>
		/// This overload is ideal when sending an email without using a template.
		/// This is a convenience method with simplified parameters.
		/// If you need more options, use the <see cref="IMail.SendAsync" /> method.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Too many recipients.</exception>
		/// <exception cref="Exception">Email exceeds the size limit.</exception>
		public static Task<string> SendToSingleRecipientAsync(
			this IMail mailResource,
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
			IEnumerable<KeyValuePair<string, string>> headers = null,
			IEnumerable<string> categories = null,
			IEnumerable<KeyValuePair<string, string>> customArgs = null,
			DateTime? sendAt = null,
			string batchId = null,
			UnsubscribeOptions unsubscribeOptions = null,
			string ipPoolName = null,
			MailSettings mailSettings = null,
			MailPriority priority = MailPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			var recipients = new[] { to };
			return mailResource.SendToMultipleRecipientsAsync(recipients, from, subject, htmlContent, textContent, trackOpens, trackClicks, subscriptionTracking, replyTo, attachments, headers, categories, customArgs, sendAt, batchId, unsubscribeOptions, ipPoolName, mailSettings, priority, cancellationToken);
		}

		/// <summary>
		/// Send an email to a single recipient using a legacy template.
		/// </summary>
		/// <param name="mailResource">The mail resource.</param>
		/// <param name="to">To.</param>
		/// <param name="from">From.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="substitutions">Data to be merged in the content.</param>
		/// <param name="trackOpens">if set to <c>true</c> [track opens].</param>
		/// <param name="trackClicks">if set to <c>true</c> [track clicks].</param>
		/// <param name="subscriptionTracking">The subscription tracking.</param>
		/// <param name="replyTo">The reply-to address.</param>
		/// <param name="attachments">The attachments.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customArgs">The custom arguments.</param>
		/// <param name="sendAt">The send at.</param>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="unsubscribeOptions">The unsubscribe options.</param>
		/// <param name="ipPoolName">Name of the ip pool.</param>
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The message id.
		/// </returns>
		/// <remarks>
		/// This is a convenience method with simplified parameters.
		/// If you need more options, use the <see cref="IMail.SendAsync" /> method.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Too many recipients.</exception>
		/// <exception cref="Exception">Email exceeds the size limit.</exception>
		public static Task<string> SendToSingleRecipientAsync(
			this IMail mailResource,
			MailAddress to,
			MailAddress from,
			string subject,
			string htmlContent,
			string textContent,
			string templateId,
			IEnumerable<KeyValuePair<string, string>> substitutions = null,
			bool trackOpens = true,
			bool trackClicks = true,
			SubscriptionTrackingSettings subscriptionTracking = null,
			MailAddress replyTo = null,
			IEnumerable<Attachment> attachments = null,
			IEnumerable<KeyValuePair<string, string>> headers = null,
			IEnumerable<string> categories = null,
			IEnumerable<KeyValuePair<string, string>> customArgs = null,
			DateTime? sendAt = null,
			string batchId = null,
			UnsubscribeOptions unsubscribeOptions = null,
			string ipPoolName = null,
			MailSettings mailSettings = null,
			MailPriority priority = MailPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			var recipients = new[] { to };
			return mailResource.SendToMultipleRecipientsAsync(recipients, from, subject, htmlContent, textContent, templateId, substitutions, trackOpens, trackClicks, subscriptionTracking, replyTo, attachments, headers, categories, customArgs, sendAt, batchId, unsubscribeOptions, ipPoolName, mailSettings, priority, cancellationToken);
		}

		/// <summary>
		/// Send an email to a single recipient using a dynamic template.
		/// </summary>
		/// <param name="mailResource">The mail resource.</param>
		/// <param name="to">To.</param>
		/// <param name="from">From.</param>
		/// <param name="dynamicTemplateId">The identifier of the template.</param>
		/// <param name="dynamicData">The data to be merged in the content.</param>
		/// <param name="trackOpens">if set to <c>true</c> [track opens].</param>
		/// <param name="trackClicks">if set to <c>true</c> [track clicks].</param>
		/// <param name="subscriptionTracking">The subscription tracking.</param>
		/// <param name="replyTo">The reply-to address.</param>
		/// <param name="attachments">The attachments.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customArgs">The custom arguments.</param>
		/// <param name="sendAt">The send at.</param>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="unsubscribeOptions">The unsubscribe options.</param>
		/// <param name="ipPoolName">Name of the ip pool.</param>
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The message id.
		/// </returns>
		/// <remarks>
		/// This is a convenience method with simplified parameters.
		/// If you need more options, use the <see cref="IMail.SendAsync" /> method.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Too many recipients.</exception>
		/// <exception cref="Exception">Email exceeds the size limit.</exception>
		public static Task<string> SendToSingleRecipientAsync(
			this IMail mailResource,
			MailAddress to,
			MailAddress from,
			string dynamicTemplateId,
			object dynamicData = null,
			bool trackOpens = true,
			bool trackClicks = true,
			SubscriptionTrackingSettings subscriptionTracking = null,
			MailAddress replyTo = null,
			IEnumerable<Attachment> attachments = null,
			IEnumerable<KeyValuePair<string, string>> headers = null,
			IEnumerable<string> categories = null,
			IEnumerable<KeyValuePair<string, string>> customArgs = null,
			DateTime? sendAt = null,
			string batchId = null,
			UnsubscribeOptions unsubscribeOptions = null,
			string ipPoolName = null,
			MailSettings mailSettings = null,
			MailPriority priority = MailPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			var recipients = new[] { to };
			return mailResource.SendToMultipleRecipientsAsync(recipients, from, dynamicTemplateId, dynamicData, trackOpens, trackClicks, subscriptionTracking, replyTo, attachments, headers, categories, customArgs, sendAt, batchId, unsubscribeOptions, ipPoolName, mailSettings, priority, cancellationToken);
		}

		/// <summary>
		/// Send an AMP email to a single recipient without using a template (which means you must provide the subject, html content and text content).
		/// </summary>
		/// <param name="mailResource">The mail resource.</param>
		/// <param name="to">To.</param>
		/// <param name="from">From.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="ampContent">Content of the AMP.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="trackOpens">if set to <c>true</c> [track opens].</param>
		/// <param name="trackClicks">if set to <c>true</c> [track clicks].</param>
		/// <param name="subscriptionTracking">The subscription tracking.</param>
		/// <param name="replyTo">The reply-to address.</param>
		/// <param name="attachments">The attachments.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customArgs">The custom arguments.</param>
		/// <param name="sendAt">The send at.</param>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="unsubscribeOptions">The unsubscribe options.</param>
		/// <param name="ipPoolName">Name of the ip pool.</param>
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The message id.
		/// </returns>
		/// <remarks>
		/// This overload is ideal when sending an email without using a template.
		/// This is a convenience method with simplified parameters.
		/// If you need more options, use the <see cref="IMail.SendAsync" /> method.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Too many recipients.</exception>
		/// <exception cref="Exception">Email exceeds the size limit.</exception>
		public static Task<string> SendAmpEmailToSingleRecipientAsync(
			this IMail mailResource,
			MailAddress to,
			MailAddress from,
			string subject,
			string ampContent,
			string htmlContent,
			string textContent,
			bool trackOpens = true,
			bool trackClicks = true,
			SubscriptionTrackingSettings subscriptionTracking = null,
			MailAddress replyTo = null,
			IEnumerable<Attachment> attachments = null,
			IEnumerable<KeyValuePair<string, string>> headers = null,
			IEnumerable<string> categories = null,
			IEnumerable<KeyValuePair<string, string>> customArgs = null,
			DateTime? sendAt = null,
			string batchId = null,
			UnsubscribeOptions unsubscribeOptions = null,
			string ipPoolName = null,
			MailSettings mailSettings = null,
			MailPriority priority = MailPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			var recipients = new[] { to };
			return mailResource.SendAmpEmailToMultipleRecipientsAsync(recipients, from, subject, ampContent, htmlContent, textContent, trackOpens, trackClicks, subscriptionTracking, replyTo, attachments, headers, categories, customArgs, sendAt, batchId, unsubscribeOptions, ipPoolName, mailSettings, priority, cancellationToken);
		}

		/// <summary>
		/// Send the same email to multiple recipients without using a template (which means you must provide the subject, html content and text content).
		/// </summary>
		/// <param name="mailResource">The mail resource.</param>
		/// <param name="recipients">The recipients.</param>
		/// <param name="from">From.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="trackOpens">if set to <c>true</c> [track opens].</param>
		/// <param name="trackClicks">if set to <c>true</c> [track clicks].</param>
		/// <param name="subscriptionTracking">The subscription tracking.</param>
		/// <param name="replyTo">The reply-to address.</param>
		/// <param name="attachments">The attachments.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customArgs">The custom arguments.</param>
		/// <param name="sendAt">The send at.</param>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="unsubscribeOptions">The unsubscribe options.</param>
		/// <param name="ipPoolName">Name of the ip pool.</param>
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The message id.
		/// </returns>
		/// <remarks>
		/// This is a convenience method with simplified parameters.
		/// If you need more options, use the <see cref="IMail.SendAsync" /> method.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Too many recipients.</exception>
		/// <exception cref="Exception">Email exceeds the size limit.</exception>
		public static Task<string> SendToMultipleRecipientsAsync(
			this IMail mailResource,
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
			IEnumerable<KeyValuePair<string, string>> headers = null,
			IEnumerable<string> categories = null,
			IEnumerable<KeyValuePair<string, string>> customArgs = null,
			DateTime? sendAt = null,
			string batchId = null,
			UnsubscribeOptions unsubscribeOptions = null,
			string ipPoolName = null,
			MailSettings mailSettings = null,
			MailPriority priority = MailPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			var personalizations = new[]
			{
				new MailPersonalization
				{
					To = recipients.ToArray()
				}
			};

			var contents = new List<MailContent>();
			if (!string.IsNullOrEmpty(textContent)) contents.Add(new MailContent("text/plain", textContent));
			if (!string.IsNullOrEmpty(htmlContent)) contents.Add(new MailContent("text/html", htmlContent));

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

			var replyToList = replyTo == null ? null : new[] { replyTo };

			return mailResource.SendAsync(personalizations, subject, contents, from, replyToList, attachments, null, headers, categories, customArgs, sendAt, batchId, unsubscribeOptions, ipPoolName, mailSettings, trackingSettings, priority, cancellationToken);
		}

		/// <summary>
		/// Send the same email to multiple recipients using a legacy template.
		/// </summary>
		/// <param name="mailResource">The mail resource.</param>
		/// <param name="recipients">The recipients.</param>
		/// <param name="from">From.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="substitutions">Data to be merged in the content.</param>
		/// <param name="trackOpens">if set to <c>true</c> [track opens].</param>
		/// <param name="trackClicks">if set to <c>true</c> [track clicks].</param>
		/// <param name="subscriptionTracking">The subscription tracking.</param>
		/// <param name="replyTo">The reply-to address.</param>
		/// <param name="attachments">The attachments.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customArgs">The custom arguments.</param>
		/// <param name="sendAt">The send at.</param>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="unsubscribeOptions">The unsubscribe options.</param>
		/// <param name="ipPoolName">Name of the ip pool.</param>
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The message id.
		/// </returns>
		/// <remarks>
		/// This is a convenience method with simplified parameters.
		/// If you need more options, use the <see cref="IMail.SendAsync" /> method.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Too many recipients.</exception>
		/// <exception cref="Exception">Email exceeds the size limit.</exception>
		public static Task<string> SendToMultipleRecipientsAsync(
			this IMail mailResource,
			IEnumerable<MailAddress> recipients,
			MailAddress from,
			string subject,
			string htmlContent,
			string textContent,
			string templateId,
			IEnumerable<KeyValuePair<string, string>> substitutions = null,
			bool trackOpens = true,
			bool trackClicks = true,
			SubscriptionTrackingSettings subscriptionTracking = null,
			MailAddress replyTo = null,
			IEnumerable<Attachment> attachments = null,
			IEnumerable<KeyValuePair<string, string>> headers = null,
			IEnumerable<string> categories = null,
			IEnumerable<KeyValuePair<string, string>> customArgs = null,
			DateTime? sendAt = null,
			string batchId = null,
			UnsubscribeOptions unsubscribeOptions = null,
			string ipPoolName = null,
			MailSettings mailSettings = null,
			MailPriority priority = MailPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			var personalizations = new[]
			{
				new MailPersonalization
				{
					To = recipients.ToArray(),
					Substitutions = substitutions?.ToArray()
				}
			};

			var contents = new List<MailContent>();
			if (!string.IsNullOrEmpty(textContent)) contents.Add(new MailContent("text/plain", textContent));
			if (!string.IsNullOrEmpty(htmlContent)) contents.Add(new MailContent("text/html", htmlContent));

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

			var replyToList = replyTo == null ? null : new[] { replyTo };

			return mailResource.SendAsync(personalizations, subject, contents, from, replyToList, attachments, templateId, headers, categories, customArgs, sendAt, batchId, unsubscribeOptions, ipPoolName, mailSettings, trackingSettings, priority, cancellationToken);
		}

		/// <summary>
		/// Send the same email to multiple recipients.
		/// </summary>
		/// <param name="mailResource">The mail resource.</param>
		/// <param name="recipients">The recipients.</param>
		/// <param name="from">From.</param>
		/// <param name="dynamicTemplateId">The identifier of the template.</param>
		/// <param name="dynamicData">The data to be merged in the content.</param>
		/// <param name="trackOpens">if set to <c>true</c> [track opens].</param>
		/// <param name="trackClicks">if set to <c>true</c> [track clicks].</param>
		/// <param name="subscriptionTracking">The subscription tracking.</param>
		/// <param name="replyTo">The reply-to address.</param>
		/// <param name="attachments">The attachments.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customArgs">The custom arguments.</param>
		/// <param name="sendAt">The send at.</param>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="unsubscribeOptions">The unsubscribe options.</param>
		/// <param name="ipPoolName">Name of the ip pool.</param>
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The message id.
		/// </returns>
		/// <remarks>
		/// This is a convenience method with simplified parameters.
		/// If you need more options, use the <see cref="IMail.SendAsync" /> method.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Too many recipients.</exception>
		/// <exception cref="Exception">Email exceeds the size limit.</exception>
		public static Task<string> SendToMultipleRecipientsAsync(
			this IMail mailResource,
			IEnumerable<MailAddress> recipients,
			MailAddress from,
			string dynamicTemplateId,
			object dynamicData = null,
			bool trackOpens = true,
			bool trackClicks = true,
			SubscriptionTrackingSettings subscriptionTracking = null,
			MailAddress replyTo = null,
			IEnumerable<Attachment> attachments = null,
			IEnumerable<KeyValuePair<string, string>> headers = null,
			IEnumerable<string> categories = null,
			IEnumerable<KeyValuePair<string, string>> customArgs = null,
			DateTime? sendAt = null,
			string batchId = null,
			UnsubscribeOptions unsubscribeOptions = null,
			string ipPoolName = null,
			MailSettings mailSettings = null,
			MailPriority priority = MailPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			if (!Template.IsDynamic(dynamicTemplateId))
			{
				throw new ArgumentException($"{dynamicTemplateId} is not a valid dynamic template identifier.", nameof(dynamicTemplateId));
			}

			var personalizations = new[]
			{
				new MailPersonalization
				{
					To = recipients.ToArray(),
					DynamicData = dynamicData
				}
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

			var replyToList = replyTo == null ? null : new[] { replyTo };

			return mailResource.SendAsync(personalizations, null, null, from, replyToList, attachments, dynamicTemplateId, headers, categories, customArgs, sendAt, batchId, unsubscribeOptions, ipPoolName, mailSettings, trackingSettings, priority, cancellationToken);
		}

		/// <summary>
		/// Send the same AMP email to multiple recipients.
		/// </summary>
		/// <param name="mailResource">The mail resource.</param>
		/// <param name="recipients">The recipients.</param>
		/// <param name="from">From.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="ampContent">Content of the AMP.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="trackOpens">if set to <c>true</c> [track opens].</param>
		/// <param name="trackClicks">if set to <c>true</c> [track clicks].</param>
		/// <param name="subscriptionTracking">The subscription tracking.</param>
		/// <param name="replyTo">The reply-to address.</param>
		/// <param name="attachments">The attachments.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customArgs">The custom arguments.</param>
		/// <param name="sendAt">The send at.</param>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="unsubscribeOptions">The unsubscribe options.</param>
		/// <param name="ipPoolName">Name of the ip pool.</param>
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The message id.
		/// </returns>
		/// <remarks>
		/// This is a convenience method with simplified parameters.
		/// If you need more options, use the <see cref="IMail.SendAsync" /> method.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Too many recipients.</exception>
		/// <exception cref="Exception">Email exceeds the size limit.</exception>
		public static Task<string> SendAmpEmailToMultipleRecipientsAsync(
			this IMail mailResource,
			IEnumerable<MailAddress> recipients,
			MailAddress from,
			string subject,
			string ampContent,
			string htmlContent,
			string textContent,
			bool trackOpens = true,
			bool trackClicks = true,
			SubscriptionTrackingSettings subscriptionTracking = null,
			MailAddress replyTo = null,
			IEnumerable<Attachment> attachments = null,
			IEnumerable<KeyValuePair<string, string>> headers = null,
			IEnumerable<string> categories = null,
			IEnumerable<KeyValuePair<string, string>> customArgs = null,
			DateTime? sendAt = null,
			string batchId = null,
			UnsubscribeOptions unsubscribeOptions = null,
			string ipPoolName = null,
			MailSettings mailSettings = null,
			MailPriority priority = MailPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			var personalizations = new[]
			{
				new MailPersonalization
				{
					To = recipients.ToArray()
				}
			};

			var contents = new List<MailContent>();
			if (!string.IsNullOrEmpty(textContent)) contents.Add(new MailContent("text/plain", textContent));
			if (!string.IsNullOrEmpty(ampContent)) contents.Add(new MailContent("text/x-amp-html", ampContent));
			if (!string.IsNullOrEmpty(htmlContent)) contents.Add(new MailContent("text/html", htmlContent));

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

			var replyToList = replyTo == null ? null : new[] { replyTo };

			return mailResource.SendAsync(personalizations, subject, contents, from, replyToList, attachments, null, headers, categories, customArgs, sendAt, batchId, unsubscribeOptions, ipPoolName, mailSettings, trackingSettings, priority, cancellationToken);
		}

		/// <summary>
		/// Send an email to a single recipient without using a template (which means you must provide the subject, html content and text content).
		/// </summary>
		/// <param name="warmupEngine">The warmup engine.</param>
		/// <param name="to">To.</param>
		/// <param name="from">From.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="trackOpens">if set to <c>true</c> [track opens].</param>
		/// <param name="trackClicks">if set to <c>true</c> [track clicks].</param>
		/// <param name="subscriptionTracking">The subscription tracking.</param>
		/// <param name="replyTo">The reply-to address.</param>
		/// <param name="attachments">The attachments.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customArgs">The custom arguments.</param>
		/// <param name="sendAt">The send at.</param>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="unsubscribeOptions">The unsubscribe options.</param>
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The result.
		/// </returns>
		/// <remarks>
		/// This overload is ideal when sending an email without using a template.
		/// This is a convenience method with simplified parameters.
		/// If you need more options, use the <see cref="IWarmupEngine.SendAsync" /> method.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Too many recipients.</exception>
		/// <exception cref="Exception">Email exceeds the size limit.</exception>
		public static Task<WarmupResult> SendToSingleRecipientAsync(
			this IWarmupEngine warmupEngine,
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
			IEnumerable<KeyValuePair<string, string>> headers = null,
			IEnumerable<string> categories = null,
			IEnumerable<KeyValuePair<string, string>> customArgs = null,
			DateTime? sendAt = null,
			string batchId = null,
			UnsubscribeOptions unsubscribeOptions = null,
			MailSettings mailSettings = null,
			MailPriority priority = MailPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			var recipients = new[] { to };
			return warmupEngine.SendToMultipleRecipientsAsync(recipients, from, subject, htmlContent, textContent, trackOpens, trackClicks, subscriptionTracking, replyTo, attachments, headers, categories, customArgs, sendAt, batchId, unsubscribeOptions, mailSettings, priority, cancellationToken);
		}

		/// <summary>
		/// Send an email to a single recipient using a legacy template.
		/// </summary>
		/// <param name="warmupEngine">The warmup engine.</param>
		/// <param name="to">To.</param>
		/// <param name="from">From.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="substitutions">Data to be merged in the content.</param>
		/// <param name="trackOpens">if set to <c>true</c> [track opens].</param>
		/// <param name="trackClicks">if set to <c>true</c> [track clicks].</param>
		/// <param name="subscriptionTracking">The subscription tracking.</param>
		/// <param name="replyTo">The reply-to address.</param>
		/// <param name="attachments">The attachments.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customArgs">The custom arguments.</param>
		/// <param name="sendAt">The send at.</param>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="unsubscribeOptions">The unsubscribe options.</param>
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The result.
		/// </returns>
		/// <remarks>
		/// This is a convenience method with simplified parameters.
		/// If you need more options, use the <see cref="IWarmupEngine.SendAsync" /> method.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Too many recipients.</exception>
		/// <exception cref="Exception">Email exceeds the size limit.</exception>
		public static Task<WarmupResult> SendToSingleRecipientAsync(
			this IWarmupEngine warmupEngine,
			MailAddress to,
			MailAddress from,
			string subject,
			string htmlContent,
			string textContent,
			string templateId,
			IEnumerable<KeyValuePair<string, string>> substitutions = null,
			bool trackOpens = true,
			bool trackClicks = true,
			SubscriptionTrackingSettings subscriptionTracking = null,
			MailAddress replyTo = null,
			IEnumerable<Attachment> attachments = null,
			IEnumerable<KeyValuePair<string, string>> headers = null,
			IEnumerable<string> categories = null,
			IEnumerable<KeyValuePair<string, string>> customArgs = null,
			DateTime? sendAt = null,
			string batchId = null,
			UnsubscribeOptions unsubscribeOptions = null,
			MailSettings mailSettings = null,
			MailPriority priority = MailPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			var recipients = new[] { to };
			return warmupEngine.SendToMultipleRecipientsAsync(recipients, from, subject, htmlContent, textContent, templateId, substitutions, trackOpens, trackClicks, subscriptionTracking, replyTo, attachments, headers, categories, customArgs, sendAt, batchId, unsubscribeOptions, mailSettings, priority, cancellationToken);
		}

		/// <summary>
		/// Send an email to a single recipient using a dynamic template.
		/// </summary>
		/// <param name="warmupEngine">The warmup engine.</param>
		/// <param name="to">To.</param>
		/// <param name="from">From.</param>
		/// <param name="dynamicTemplateId">The identifier of the template.</param>
		/// <param name="dynamicData">The data to be merged in the content.</param>
		/// <param name="trackOpens">if set to <c>true</c> [track opens].</param>
		/// <param name="trackClicks">if set to <c>true</c> [track clicks].</param>
		/// <param name="subscriptionTracking">The subscription tracking.</param>
		/// <param name="replyTo">The reply-to address.</param>
		/// <param name="attachments">The attachments.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customArgs">The custom arguments.</param>
		/// <param name="sendAt">The send at.</param>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="unsubscribeOptions">The unsubscribe options.</param>
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The result.
		/// </returns>
		/// <remarks>
		/// This is a convenience method with simplified parameters.
		/// If you need more options, use the <see cref="IWarmupEngine.SendAsync" /> method.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Too many recipients.</exception>
		/// <exception cref="Exception">Email exceeds the size limit.</exception>
		public static Task<WarmupResult> SendToSingleRecipientAsync(
			this IWarmupEngine warmupEngine,
			MailAddress to,
			MailAddress from,
			string dynamicTemplateId,
			object dynamicData = null,
			bool trackOpens = true,
			bool trackClicks = true,
			SubscriptionTrackingSettings subscriptionTracking = null,
			MailAddress replyTo = null,
			IEnumerable<Attachment> attachments = null,
			IEnumerable<KeyValuePair<string, string>> headers = null,
			IEnumerable<string> categories = null,
			IEnumerable<KeyValuePair<string, string>> customArgs = null,
			DateTime? sendAt = null,
			string batchId = null,
			UnsubscribeOptions unsubscribeOptions = null,
			MailSettings mailSettings = null,
			MailPriority priority = MailPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			var recipients = new[] { to };
			return warmupEngine.SendToMultipleRecipientsAsync(recipients, from, dynamicTemplateId, dynamicData, trackOpens, trackClicks, subscriptionTracking, replyTo, attachments, headers, categories, customArgs, sendAt, batchId, unsubscribeOptions, mailSettings, priority, cancellationToken);
		}

		/// <summary>
		/// Send the same email to multiple recipients without using a template (which means you must provide the subject, html content and text content).
		/// </summary>
		/// <param name="warmupEngine">The warmup engine.</param>
		/// <param name="recipients">The recipients.</param>
		/// <param name="from">From.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="trackOpens">if set to <c>true</c> [track opens].</param>
		/// <param name="trackClicks">if set to <c>true</c> [track clicks].</param>
		/// <param name="subscriptionTracking">The subscription tracking.</param>
		/// <param name="replyTo">The reply-to address.</param>
		/// <param name="attachments">The attachments.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customArgs">The custom arguments.</param>
		/// <param name="sendAt">The send at.</param>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="unsubscribeOptions">The unsubscribe options.</param>
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The result.
		/// </returns>
		/// <remarks>
		/// This is a convenience method with simplified parameters.
		/// If you need more options, use the <see cref="IWarmupEngine.SendAsync" /> method.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Too many recipients.</exception>
		/// <exception cref="Exception">Email exceeds the size limit.</exception>
		public static Task<WarmupResult> SendToMultipleRecipientsAsync(
			this IWarmupEngine warmupEngine,
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
			IEnumerable<KeyValuePair<string, string>> headers = null,
			IEnumerable<string> categories = null,
			IEnumerable<KeyValuePair<string, string>> customArgs = null,
			DateTime? sendAt = null,
			string batchId = null,
			UnsubscribeOptions unsubscribeOptions = null,
			MailSettings mailSettings = null,
			MailPriority priority = MailPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			var personalizations = new[]
			{
				new MailPersonalization
				{
					To = recipients.ToArray()
				}
			};

			var contents = new List<MailContent>();
			if (!string.IsNullOrEmpty(textContent)) contents.Add(new MailContent("text/plain", textContent));
			if (!string.IsNullOrEmpty(htmlContent)) contents.Add(new MailContent("text/html", htmlContent));

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

			var replyToList = replyTo == null ? null : new[] { replyTo };

			return warmupEngine.SendAsync(personalizations, subject, contents, from, replyToList, attachments, null, headers, categories, customArgs, sendAt, batchId, unsubscribeOptions, mailSettings, trackingSettings, priority, cancellationToken);
		}

		/// <summary>
		/// Send the same email to multiple recipients using a legacy template.
		/// </summary>
		/// <param name="warmupEngine">The warmup engine.</param>
		/// <param name="recipients">The recipients.</param>
		/// <param name="from">From.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="substitutions">Data to be merged in the content.</param>
		/// <param name="trackOpens">if set to <c>true</c> [track opens].</param>
		/// <param name="trackClicks">if set to <c>true</c> [track clicks].</param>
		/// <param name="subscriptionTracking">The subscription tracking.</param>
		/// <param name="replyTo">The reply-to address.</param>
		/// <param name="attachments">The attachments.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customArgs">The custom arguments.</param>
		/// <param name="sendAt">The send at.</param>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="unsubscribeOptions">The unsubscribe options.</param>
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The result.
		/// </returns>
		/// <remarks>
		/// This is a convenience method with simplified parameters.
		/// If you need more options, use the <see cref="IWarmupEngine.SendAsync" /> method.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Too many recipients.</exception>
		/// <exception cref="Exception">Email exceeds the size limit.</exception>
		public static Task<WarmupResult> SendToMultipleRecipientsAsync(
			this IWarmupEngine warmupEngine,
			IEnumerable<MailAddress> recipients,
			MailAddress from,
			string subject,
			string htmlContent,
			string textContent,
			string templateId,
			IEnumerable<KeyValuePair<string, string>> substitutions = null,
			bool trackOpens = true,
			bool trackClicks = true,
			SubscriptionTrackingSettings subscriptionTracking = null,
			MailAddress replyTo = null,
			IEnumerable<Attachment> attachments = null,
			IEnumerable<KeyValuePair<string, string>> headers = null,
			IEnumerable<string> categories = null,
			IEnumerable<KeyValuePair<string, string>> customArgs = null,
			DateTime? sendAt = null,
			string batchId = null,
			UnsubscribeOptions unsubscribeOptions = null,
			MailSettings mailSettings = null,
			MailPriority priority = MailPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			var personalizations = new[]
			{
				new MailPersonalization
				{
					To = recipients.ToArray(),
					Substitutions = substitutions?.ToArray()
				}
			};

			var contents = new List<MailContent>();
			if (!string.IsNullOrEmpty(textContent)) contents.Add(new MailContent("text/plain", textContent));
			if (!string.IsNullOrEmpty(htmlContent)) contents.Add(new MailContent("text/html", htmlContent));

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

			var replyToList = replyTo == null ? null : new[] { replyTo };

			return warmupEngine.SendAsync(personalizations, subject, contents, from, replyToList, attachments, templateId, headers, categories, customArgs, sendAt, batchId, unsubscribeOptions, mailSettings, trackingSettings, priority, cancellationToken);
		}

		/// <summary>
		/// Send the same email to multiple recipients.
		/// </summary>
		/// <param name="warmupEngine">The warmup engine.</param>
		/// <param name="recipients">The recipients.</param>
		/// <param name="from">From.</param>
		/// <param name="dynamicTemplateId">The identifier of the template.</param>
		/// <param name="dynamicData">The data to be merged in the content.</param>
		/// <param name="trackOpens">if set to <c>true</c> [track opens].</param>
		/// <param name="trackClicks">if set to <c>true</c> [track clicks].</param>
		/// <param name="subscriptionTracking">The subscription tracking.</param>
		/// <param name="replyTo">The reply-to address.</param>
		/// <param name="attachments">The attachments.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customArgs">The custom arguments.</param>
		/// <param name="sendAt">The send at.</param>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="unsubscribeOptions">The unsubscribe options.</param>
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The result.
		/// </returns>
		/// <remarks>
		/// This is a convenience method with simplified parameters.
		/// If you need more options, use the <see cref="IWarmupEngine.SendAsync" /> method.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Too many recipients.</exception>
		/// <exception cref="Exception">Email exceeds the size limit.</exception>
		public static Task<WarmupResult> SendToMultipleRecipientsAsync(
			this IWarmupEngine warmupEngine,
			IEnumerable<MailAddress> recipients,
			MailAddress from,
			string dynamicTemplateId,
			object dynamicData = null,
			bool trackOpens = true,
			bool trackClicks = true,
			SubscriptionTrackingSettings subscriptionTracking = null,
			MailAddress replyTo = null,
			IEnumerable<Attachment> attachments = null,
			IEnumerable<KeyValuePair<string, string>> headers = null,
			IEnumerable<string> categories = null,
			IEnumerable<KeyValuePair<string, string>> customArgs = null,
			DateTime? sendAt = null,
			string batchId = null,
			UnsubscribeOptions unsubscribeOptions = null,
			MailSettings mailSettings = null,
			MailPriority priority = MailPriority.Normal,
			CancellationToken cancellationToken = default)
		{
			if (!Template.IsDynamic(dynamicTemplateId))
			{
				throw new ArgumentException($"{dynamicTemplateId} is not a valid dynamic template identifier.", nameof(dynamicTemplateId));
			}

			var personalizations = new[]
			{
				new MailPersonalization
				{
					To = recipients.ToArray(),
					DynamicData = dynamicData
				}
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

			var replyToList = replyTo == null ? null : new[] { replyTo };

			return warmupEngine.SendAsync(personalizations, null, null, from, replyToList, attachments, dynamicTemplateId, headers, categories, customArgs, sendAt, batchId, unsubscribeOptions, mailSettings, trackingSettings, priority, cancellationToken);
		}

		/// <summary>
		/// Get all of the details about the messages matching the criteria.
		/// </summary>
		/// <param name="emailActivities">The email activities resource.</param>
		/// <param name="criteria">Filtering criteria.</param>
		/// <param name="limit">Number of IP activity entries to return.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="EmailMessageActivity" />.
		/// </returns>
		public static Task<EmailMessageActivity[]> SearchAsync(this IEmailActivities emailActivities, Models.Search.Legacy.ISearchCriteria criteria, int limit = 20, CancellationToken cancellationToken = default)
		{
			var filterCriteria = criteria == null ? Enumerable.Empty<Models.Search.Legacy.ISearchCriteria>() : new[] { criteria };
			return emailActivities.SearchAsync(filterCriteria, limit, cancellationToken);
		}

		/// <summary>
		/// Get all of the details about the messages matching the criteria.
		/// </summary>
		/// <param name="emailActivities">The email activities resource.</param>
		/// <param name="filterConditions">Filtering conditions.</param>
		/// <param name="limit">Number of IP activity entries to return.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="EmailMessageActivity" />.
		/// </returns>
		public static Task<EmailMessageActivity[]> SearchAsync(this IEmailActivities emailActivities, IEnumerable<Models.Search.Legacy.ISearchCriteria> filterConditions, int limit = 20, CancellationToken cancellationToken = default)
		{
			var filters = new List<KeyValuePair<SearchLogicalOperator, IEnumerable<Models.Search.Legacy.ISearchCriteria>>>();
			if (filterConditions != null && filterConditions.Any()) filters.Add(new KeyValuePair<SearchLogicalOperator, IEnumerable<Models.Search.Legacy.ISearchCriteria>>(SearchLogicalOperator.And, filterConditions));
			return emailActivities.SearchAsync(filters, limit, cancellationToken);
		}

		/// <summary>
		/// Get all of the details about the contacts matching the criteria.
		/// </summary>
		/// <param name="contacts">The contacts resource.</param>
		/// <param name="criteria">Filtering criteria.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		public static Task<Contact[]> SearchAsync(this IContacts contacts, SearchCriteria<ContactsFilterField> criteria, CancellationToken cancellationToken = default)
		{
			var filterCriteria = criteria == null ? Array.Empty<SearchCriteria<ContactsFilterField>>() : new[] { criteria };
			return contacts.SearchAsync(filterCriteria, cancellationToken);
		}

		/// <summary>
		/// Get all of the details about the contacts matching the criteria.
		/// </summary>
		/// <param name="contacts">The contacts resource.</param>
		/// <param name="filterConditions">Filtering conditions.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		public static Task<Contact[]> SearchAsync(this IContacts contacts, IEnumerable<SearchCriteria<ContactsFilterField>> filterConditions, CancellationToken cancellationToken = default)
		{
			var filters = new List<KeyValuePair<SearchLogicalOperator, IEnumerable<SearchCriteria<ContactsFilterField>>>>();
			if (filterConditions != null && filterConditions.Any()) filters.Add(new KeyValuePair<SearchLogicalOperator, IEnumerable<SearchCriteria<ContactsFilterField>>>(SearchLogicalOperator.And, filterConditions));
			return contacts.SearchAsync(filters, cancellationToken);
		}

		/// <summary>
		/// Update a segment.
		/// </summary>
		/// <param name="segments">The segments resource.</param>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="criteria">The criteria.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Segment" />.
		/// </returns>
		public static Task<Segment> UpdateAsync(this ISegments segments, string segmentId, Parameter<string> name = default, Parameter<SearchCriteria<ContactsFilterField>> criteria = default, CancellationToken cancellationToken = default)
		{
			var filterCriteria = criteria.HasValue && criteria.Value != null ? new[] { criteria.Value } : Array.Empty<SearchCriteria<ContactsFilterField>>();
			return segments.UpdateAsync(segmentId, name, filterCriteria, cancellationToken);
		}

		/// <summary>
		/// Update a segment.
		/// </summary>
		/// <param name="segments">The segments resource.</param>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="filterConditions">The enumeration of criteria.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Segment" />.
		/// </returns>
		public static Task<Segment> UpdateAsync(this ISegments segments, string segmentId, Parameter<string> name = default, Parameter<IEnumerable<SearchCriteria<ContactsFilterField>>> filterConditions = default, CancellationToken cancellationToken = default)
		{
			var filters = new List<KeyValuePair<SearchLogicalOperator, IEnumerable<SearchCriteria<ContactsFilterField>>>>();
			if (filterConditions.HasValue && filterConditions.Value != null && filterConditions.Value.Any()) filters.Add(new KeyValuePair<SearchLogicalOperator, IEnumerable<SearchCriteria<ContactsFilterField>>>(SearchLogicalOperator.And, filterConditions.Value));
			return segments.UpdateAsync(segmentId, name, filters, cancellationToken);
		}

		/// <summary>
		/// Retrieve unassigned IP addresses.
		/// </summary>
		/// <param name="ipAddresses">The IP addresses resource.</param>
		/// <param name="limit">The number of IPs you want returned at the same time.</param>
		/// <param name="offset">The offset for the number of IPs that you are requesting.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="IpAddress">Ip addresses</see>.
		/// </returns>
		public static async Task<IpAddress[]> GetUnassignedAsync(this IIpAddresses ipAddresses, CancellationToken cancellationToken = default)
		{
			var unassignedIpAddresses = new List<IpAddress>();
			var currentOffset = 0;

			while (true)
			{
				// Retrieve 500 ip addresses at a time (that's the maximum SendGrid allow us to retrieve at a time)
				var allIpAddresses = await ipAddresses.GetAllAsync(limit: Utils.MaxSendGridPagingLimit, offset: currentOffset, cancellationToken: cancellationToken).ConfigureAwait(false);

				// Take the addresses that have not been added to a pool
				unassignedIpAddresses.AddRange(allIpAddresses.Where(ip => ip.Pools == null || !ip.Pools.Any()));

				// Stop if there are no more addresses to fetch
				if (allIpAddresses.Length < Utils.MaxSendGridPagingLimit) break;

				// Increase the offset so we retrieve the next set of 500 addresses
				currentOffset += Utils.MaxSendGridPagingLimit;
			}

			return unassignedIpAddresses.ToArray();
		}
	}
}
