using StrongGrid.Models;
using StrongGrid.Models.Search;
using StrongGrid.Resources;
using StrongGrid.Utilities;
using StrongGrid.Warmup;
using System;
using System.Collections.Generic;
using System.IO;
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
		public static Task<EmailMessageActivity[]> SearchAsync(this IEmailActivities emailActivities, ISearchCriteria criteria, int limit = 20, CancellationToken cancellationToken = default)
		{
			var filterCriteria = criteria == null ? Enumerable.Empty<ISearchCriteria>() : new[] { criteria };
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
		public static Task<EmailMessageActivity[]> SearchAsync(this IEmailActivities emailActivities, IEnumerable<ISearchCriteria> filterConditions, int limit = 20, CancellationToken cancellationToken = default)
		{
			var filters = new List<KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>>();
			if (filterConditions != null && filterConditions.Any()) filters.Add(new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, filterConditions));
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
		public static Task<Contact[]> SearchAsync(this IContacts contacts, ISearchCriteria criteria, CancellationToken cancellationToken = default)
		{
			var filterCriteria = criteria == null ? Array.Empty<SearchCriteria>() : new[] { criteria };
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
		public static Task<Contact[]> SearchAsync(this IContacts contacts, IEnumerable<ISearchCriteria> filterConditions, CancellationToken cancellationToken = default)
		{
			var filters = new List<KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>>();
			if (filterConditions != null && filterConditions.Any()) filters.Add(new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, filterConditions));
			return contacts.SearchAsync(filters, cancellationToken);
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
		public static Task<Contact[]> SearchAsync(this IContacts contacts, IEnumerable<KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>> filterConditions, CancellationToken cancellationToken = default)
		{
			var query = Utils.ToQueryDslVersion1(filterConditions);
			return contacts.SearchAsync(query, cancellationToken);
		}

		/// <summary>
		/// Create a segment.
		/// </summary>
		/// <param name="segments">The segments resource.</param>
		/// <param name="name">The name.</param>
		/// <param name="criteria">The criteria.</param>
		/// <param name="listId">The id of the list if this segment is a child of a list. This implies the query is rewritten as (${query_dsl}) AND CONTAINS(list_ids, ${parent_list_id}).</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Segment" />.
		/// </returns>
		public static Task<Segment> CreateAsync(this ISegments segments, string name, ISearchCriteria criteria, string listId = null, CancellationToken cancellationToken = default)
		{
			var filterCriteria = criteria != null ? new[] { criteria } : Array.Empty<ISearchCriteria>();
			return segments.CreateAsync(name, filterCriteria, listId, cancellationToken);
		}

		/// <summary>
		/// Create a segment.
		/// </summary>
		/// <param name="segments">The segments resource.</param>
		/// <param name="name">The name.</param>
		/// <param name="filterConditions">The enumeration of criteria.</param>
		/// <param name="listId">The id of the list if this segment is a child of a list. This implies the query is rewritten as (${query_dsl}) AND CONTAINS(list_ids, ${parent_list_id}).</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Segment" />.
		/// </returns>
		public static Task<Segment> CreateAsync(this ISegments segments, string name, IEnumerable<ISearchCriteria> filterConditions, string listId = null, CancellationToken cancellationToken = default)
		{
			var filters = new List<KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>>();
			if (filterConditions != null && filterConditions.Any()) filters.Add(new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, filterConditions));
			return segments.CreateAsync(name, filters, listId, cancellationToken);
		}

		/// <summary>
		/// Create a segment.
		/// </summary>
		/// <param name="segments">The segments resource.</param>
		/// <param name="name">The name.</param>
		/// <param name="filterConditions">The enumeration of criteria.</param>
		/// <param name="listId">The id of the list if this segment is a child of a list. This implies the query is rewritten as (${query_dsl}) AND CONTAINS(list_ids, ${parent_list_id}).</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Segment" />.
		/// </returns>
		public static Task<Segment> CreateAsync(this ISegments segments, string name, IEnumerable<KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>> filterConditions, string listId = null, CancellationToken cancellationToken = default)
		{
			var query = Utils.ToQueryDslVersion2(filterConditions);
			return segments.CreateAsync(name, query, listId, QueryLanguageVersion.Version2, cancellationToken);
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
		public static Task<Segment> UpdateAsync(this ISegments segments, string segmentId, Parameter<string> name = default, Parameter<ISearchCriteria> criteria = default, CancellationToken cancellationToken = default)
		{
			var filterCriteria = criteria.HasValue && criteria.Value != null ? new[] { criteria.Value } : Array.Empty<ISearchCriteria>();
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
		public static Task<Segment> UpdateAsync(this ISegments segments, string segmentId, Parameter<string> name = default, Parameter<IEnumerable<ISearchCriteria>> filterConditions = default, CancellationToken cancellationToken = default)
		{
			var filters = new List<KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>>();
			if (filterConditions.HasValue && filterConditions.Value != null && filterConditions.Value.Any()) filters.Add(new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, filterConditions.Value));
			return segments.UpdateAsync(segmentId, name, filters, cancellationToken);
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
		public static Task<Segment> UpdateAsync(this ISegments segments, string segmentId, Parameter<string> name = default, Parameter<IEnumerable<KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>>> filterConditions = default, CancellationToken cancellationToken = default)
		{
			var query = filterConditions.HasValue ? (Parameter<string>)Utils.ToQueryDslVersion2(filterConditions.Value) : (Parameter<string>)default;
			return segments.UpdateAsync(segmentId, name, query, QueryLanguageVersion.Version2, cancellationToken);
		}

		/// <summary>
		/// Retrieve unassigned IP addresses.
		/// </summary>
		/// <param name="ipAddresses">The IP addresses resource.</param>
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

		/// <summary>
		/// Download the files generated by an export job and save them to files.
		/// </summary>
		/// <param name="contacts">The Contacts resource.</param>
		/// <param name="jobId">The job identifier.</param>
		/// <param name="destinationFolder">The folder where the files will be saved.</param>
		/// <param name="decompress">Indicate if GZip compressed files should be automatically decompressed.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public static async Task DownloadExportFilesAsync(this IContacts contacts, string jobId, string destinationFolder, bool decompress = false, CancellationToken cancellationToken = default)
		{
			var exportFiles = await contacts.DownloadExportFilesAsync(jobId, decompress, cancellationToken).ConfigureAwait(false);

			foreach (var exportFile in exportFiles)
			{
				var destinationPath = Path.Combine(destinationFolder, exportFile.FileName);
				using (Stream output = File.OpenWrite(destinationPath))
				{
					exportFile.Stream.CopyTo(output);
				}
			}
		}

		/// <summary>
		/// Download the files generated by an export job as streams.
		/// </summary>
		/// <param name="contacts">The Contacts resource.</param>
		/// <param name="jobId">The job identifier.</param>
		/// <param name="decompress">Indicate if GZip compressed files should be automatically decompressed.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Stream"/>.
		/// </returns>
		public static async Task<(string FileName, Stream Stream)[]> DownloadExportFilesAsync(this IContacts contacts, string jobId, bool decompress = false, CancellationToken cancellationToken = default)
		{
			var job = await contacts.GetExportJobAsync(jobId, cancellationToken).ConfigureAwait(false);
			return await contacts.DownloadExportFilesAsync(job, decompress, cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// Download the CSV and save it to a file.
		/// </summary>
		/// <param name="emailActivities">The Email Activities resource.</param>
		/// <param name="downloadUUID">UUID used to locate the download CSV request entry. You can find this UUID in the email that is sent with the POST Request a CSV.</param>
		/// <param name="destinationPath">The path and name of the CSV file.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public static async Task DownloadCsvAsync(this IEmailActivities emailActivities, string downloadUUID, string destinationPath, CancellationToken cancellationToken = default)
		{
			using (var responseStream = await emailActivities.DownloadCsvAsync(downloadUUID, cancellationToken).ConfigureAwait(false))
			{
				using (Stream output = File.OpenWrite(destinationPath))
				{
					responseStream.CopyTo(output);
				}
			}
		}
	}
}
