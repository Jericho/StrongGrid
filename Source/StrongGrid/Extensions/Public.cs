using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StrongGrid.Models;
using StrongGrid.Models.Search;
using StrongGrid.Models.Webhooks;
using StrongGrid.Resources;
using StrongGrid.Utilities;
using StrongGrid.Warmup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text;
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
		/// <param name="limit">Maximum number of activity entries to return.</param>
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
		/// <param name="limit">Maximum number of activity entries to return.</param>
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
		/// Get all of the details about the messages matching the criteria.
		/// </summary>
		/// <param name="emailActivities">The email activities resource.</param>
		/// <param name="filterConditions">Filtering conditions.</param>
		/// <param name="limit">Maximum number of activity entries to return.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		public static Task<EmailMessageActivity[]> SearchAsync(this IEmailActivities emailActivities, IEnumerable<KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>> filterConditions, int limit = 20, CancellationToken cancellationToken = default)
		{
			var query = Utils.ToQueryDslVersion1(filterConditions);
			return emailActivities.SearchAsync(query, limit, cancellationToken);
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
#if NET5_0_OR_GREATER
					await exportFile.Stream.CopyToAsync(output, cancellationToken).ConfigureAwait(false);
#else
					await exportFile.Stream.CopyToAsync(output).ConfigureAwait(false);
#endif
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
#if NET5_0_OR_GREATER
					await responseStream.CopyToAsync(output, cancellationToken).ConfigureAwait(false);
#else
					await responseStream.CopyToAsync(output).ConfigureAwait(false);
#endif
				}
			}
		}

		/// <summary>
		/// Add recipient address to the suppressions list for a given group.
		/// If the group has been deleted, this request will add the address to the global suppression.
		/// </summary>
		/// <param name="suppressions">The Suppressions resource.</param>
		/// <param name="groupId">ID of the suppression group.</param>
		/// <param name="email">Email address to add to the suppression group.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public static Task AddAddressToUnsubscribeGroupAsync(this ISuppressions suppressions, long groupId, string email, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return suppressions.AddAddressesToUnsubscribeGroupAsync(groupId, new[] { email }, onBehalfOf, cancellationToken);
		}

		/// <summary>
		/// Generate a new API Key for billing.
		/// </summary>
		/// <param name="apiKeys">The ApiKeys resource.</param>
		/// <param name="name">The name.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="ApiKey" />.
		/// </returns>
		public static Task<ApiKey> CreateWithBillingPermissionsAsync(this IApiKeys apiKeys, string name, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var scopes = new[]
			{
				"billing.delete",
				"billing.read",
				"billing.update"
			};

			return apiKeys.CreateAsync(name, scopes, onBehalfOf, cancellationToken);
		}

		/// <summary>
		/// Generate a new API Key with the same permissions that have been granted to you.
		/// </summary>
		/// <param name="apiKeys">The ApiKeys resource.</param>
		/// <param name="name">The name.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="ApiKey" />.
		/// </returns>
		/// <remarks>
		/// If you specify an API Key when instanciating the <see cref="StrongGrid.Client" />, the new API Key will inherit the permissions of that API Key.
		/// If you specify a username and password when instanciating the <see cref="StrongGrid.Client" />, the new API Key will inherit the permissions of that user.
		/// </remarks>
		public static async Task<ApiKey> CreateWithAllPermissionsAsync(this IApiKeys apiKeys, string name, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var privateField = apiKeys.GetType().GetField("_client", BindingFlags.NonPublic | BindingFlags.Instance);
			if (privateField == null) throw new ArgumentException("Unable to find the HttpClient in the resource.", nameof(apiKeys));
			var client = (Pathoschild.Http.Client.IClient)privateField.GetValue(apiKeys);

			var scopes = await client.GetCurrentScopes(true, cancellationToken).ConfigureAwait(false);
			var superApiKey = await apiKeys.CreateAsync(name, scopes, onBehalfOf, cancellationToken).ConfigureAwait(false);
			return superApiKey;
		}

		/// <summary>
		/// Generate a new API Key with the same "read" permissions that have ben granted to you.
		/// </summary>
		/// <param name="apiKeys">The ApiKeys resource.</param>
		/// <param name="name">The name.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="ApiKey" />.
		/// </returns>
		/// <remarks>
		/// If you specify an API Key when instanciating the <see cref="StrongGrid.Client" />, the new API Key will inherit the "read" permissions of that API Key.
		/// If you specify a username and password when instanciating the <see cref="StrongGrid.Client" />, the new API Key will inherit the "read" permissions of that user.
		/// </remarks>
		public static async Task<ApiKey> CreateWithReadOnlyPermissionsAsync(this IApiKeys apiKeys, string name, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var privateField = apiKeys.GetType().GetField("_client", BindingFlags.NonPublic | BindingFlags.Instance);
			if (privateField == null) throw new ArgumentException("Unable to find the HttpClient in the resource.", nameof(apiKeys));
			var client = (Pathoschild.Http.Client.IClient)privateField.GetValue(apiKeys);

			var scopes = await client.GetCurrentScopes(true, cancellationToken).ConfigureAwait(false);
			scopes = scopes.Where(s => s.EndsWith(".read", System.StringComparison.OrdinalIgnoreCase)).ToArray();

			var readOnlyApiKey = await apiKeys.CreateAsync(name, scopes, onBehalfOf, cancellationToken).ConfigureAwait(false);
			return readOnlyApiKey;
		}

		/// <summary>
		/// Send a teammate invitation via email with the same "read" permissions that have been granted to you.
		/// A teammate invite will expire after 7 days, but you may resend the invite at any time
		/// to reset the expiration date.
		/// </summary>
		/// <param name="teammates">The teammates resource.</param>
		/// <param name="email">The email address of the teammate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		/// <remarks>
		/// Essentials, Legacy Lite, and Free Trial users may create up to one teammate per account.
		/// There is not a teammate limit for Pro and higher plans.
		/// </remarks>
		public static async Task<TeammateInvitation> InviteTeammateWithReadOnlyPrivilegesAsync(this ITeammates teammates, string email, CancellationToken cancellationToken = default)
		{
			var privateField = teammates.GetType().GetField("_client", BindingFlags.NonPublic | BindingFlags.Instance);
			if (privateField == null) throw new ArgumentException("Unable to find the HttpClient in the resource.", nameof(teammates));
			var client = (Pathoschild.Http.Client.IClient)privateField.GetValue(teammates);

			var scopes = await client.GetCurrentScopes(true, cancellationToken).ConfigureAwait(true);
			scopes = scopes.Where(s => s.EndsWith(".read", StringComparison.OrdinalIgnoreCase)).ToArray();

			return await teammates.InviteTeammateAsync(email, scopes, cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// Parses the signed events webhook asynchronously.
		/// </summary>
		/// <param name="parser">The webhook parser.</param>
		/// <param name="stream">The stream.</param>
		/// <param name="publicKey">Your public key. To obtain this value, see <see cref="StrongGrid.Resources.WebhookSettings.GetSignedEventsPublicKeyAsync"/>.</param>
		/// <param name="signature">The signature.</param>
		/// <param name="timestamp">The timestamp.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An array of <see cref="Event">events</see>.</returns>
		public static async Task<Event[]> ParseSignedEventsWebhookAsync(this IWebhookParser parser, Stream stream, string publicKey, string signature, string timestamp, CancellationToken cancellationToken = default)
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

			var webHookEvents = parser.ParseSignedEventsWebhook(requestBody, publicKey, signature, timestamp);
			return webHookEvents;
		}

		/// <summary>
		/// Parses the signed events webhook.
		/// </summary>
		/// <param name="parser">The webhook parser.</param>
		/// <param name="requestBody">The content submitted by SendGrid's WebHook.</param>
		/// <param name="publicKey">Your public key. To obtain this value, <see cref="StrongGrid.Resources.WebhookSettings.GetSignedEventsPublicKeyAsync"/>.</param>
		/// <param name="signature">The signature.</param>
		/// <param name="timestamp">The timestamp.</param>
		/// <returns>An array of <see cref="Event">events</see>.</returns>
		public static Event[] ParseSignedEventsWebhook(this IWebhookParser parser, string requestBody, string publicKey, string signature, string timestamp)
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

			// Prepare the parameters
			var ecParams = new ECParameters
			{
				Curve = ECCurve.NamedCurves.nistP256, // aka secp256r1 aka prime256v1
				Q = new ECPoint
				{
					X = x,
					Y = y
				}
			};

			// Create a new instance of the Elliptic Curve Digital Signature Algorithm (ECDSA)
			var eCDsa = ECDsa.Create(ecParams);

			// Verify the signature
			var verified = eCDsa.VerifyData(data, sig, HashAlgorithmName.SHA256);
#else
#error Unhandled TFM
#endif

			if (!verified)
			{
				throw new SecurityException("Webhook signature validation failed.");
			}

			var webHookEvents = parser.ParseEventsWebhook(requestBody);
			return webHookEvents;
		}

		/// <summary>
		/// Get the current event webhook settings.
		/// </summary>
		/// <param name="webhookSettings">The webhook settings resource.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EventWebhookSettings" />.
		/// </returns>
		public static Task<EventWebhookSettings> GetEventWebhookSettingsAsync(this IWebhookSettings webhookSettings, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return webhookSettings.GetEventWebhookSettingsAsync(null, onBehalfOf, cancellationToken);
		}

		/// <summary>
		/// Change the events settings.
		/// </summary>
		/// <param name="webhookSettings">The webhook settings resource.</param>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="url">The webhook endpoint url.</param>
		/// <param name="bounce">if set to <c>true</c> [bounce].</param>
		/// <param name="click">if set to <c>true</c> [click].</param>
		/// <param name="deferred">if set to <c>true</c> [deferred].</param>
		/// <param name="delivered">if set to <c>true</c> [delivered].</param>
		/// <param name="dropped">if set to <c>true</c> [dropped].</param>
		/// <param name="groupResubscribe">if set to <c>true</c> [groupResubscribe].</param>
		/// <param name="groupUnsubscribe">if set to <c>true</c> [groupUnsubscribe].</param>
		/// <param name="open">if set to <c>true</c> [open].</param>
		/// <param name="processed">if set to <c>true</c> [processed].</param>
		/// <param name="spamReport">if set to <c>true</c> [spamReport].</param>
		/// <param name="unsubscribe">if set to <c>true</c> [unsubscribe].</param>
		/// <param name="friendlyName">The friendly name.</param>
		/// <param name="oauthClientId">The OAuth client ID that SendGrid will pass to your OAuth server or service provider to generate an OAuth access token. When passing data in this parameter, you must also specify the oauthTokenUrl.</param>
		/// <param name="oauthClientSecret">The OAuth client secret that SendGrid will pass to your OAuth server or service provider to generate an OAuth access token. This secret is needed only once to create an access token. SendGrid will store the secret, allowing you to update your client ID and Token URL without passing the secret to SendGrid again. When passing data in this parameter, you must also specify the oauthClientId and oauthTokenUrl.</param>
		/// <param name="oAuthTokenUrl">The URL where SendGrid will send the OAuth client ID and client secret to generate an OAuth access token. This should be your OAuth server or service provider. When passing data in this parameter, you must also specify the oauthClientId.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EventWebhookSettings" />.
		/// </returns>
		public static Task<EventWebhookSettings> UpdateEventWebhookSettingsAsync(
			this IWebhookSettings webhookSettings,
			bool enabled,
			string url,
			bool bounce = default,
			bool click = default,
			bool deferred = default,
			bool delivered = default,
			bool dropped = default,
			bool groupResubscribe = default,
			bool groupUnsubscribe = default,
			bool open = default,
			bool processed = default,
			bool spamReport = default,
			bool unsubscribe = default,
			string friendlyName = null,
			string oauthClientId = null,
			string oauthClientSecret = null,
			string oAuthTokenUrl = null,
			string onBehalfOf = null,
			CancellationToken cancellationToken = default)
		{
			return webhookSettings.UpdateEventWebhookSettingsAsync(null, enabled, url, bounce, click, deferred, delivered, dropped, groupResubscribe, groupUnsubscribe, open, processed, spamReport, unsubscribe, friendlyName, oauthClientId, oauthClientSecret, oAuthTokenUrl, onBehalfOf, cancellationToken);
		}

		/// <summary>
		/// Sends a fake event notification post to the provided URL.
		/// </summary>
		/// <param name="webhookSettings">The webhook settings resource.</param>
		/// <param name="url">The URL where you would like the test notification to be sent.</param>
		/// <param name="oAuthClientId">The client ID Twilio SendGrid sends to your OAuth server or service provider to generate an OAuth access token. When passing data in this parameter, you must also specify oauThokenUrl.</param>
		/// <param name="oAuthClientSecret">This value is needed only once to create an access token. SendGrid will store this secret, allowing you to update your Client ID and Token URL without passing the secret to SendGrid again. When passing data in this field, you must also specify oAuthClientId and oAuthTokenUrl.</param>
		/// <param name="oAuthTokenUrl">The URL where Twilio SendGrid sends the Client ID and Client Secret to generate an access token. This should be your OAuth server or service provider. When passing data in this parameter, you must also include oAuthClientId.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public static Task SendEventTestAsync(this IWebhookSettings webhookSettings, string url, string oAuthClientId = null, string oAuthClientSecret = null, string oAuthTokenUrl = null, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return webhookSettings.SendEventTestAsync(null, url, oAuthClientId, oAuthClientSecret, oAuthTokenUrl, onBehalfOf, cancellationToken);
		}

		/// <summary>
		/// Enable or disable signature verification for a single Event Webhook.
		/// </summary>
		/// <param name="webhookSettings">The webhook settings resource.</param>
		/// <param name="id">The ID of the Event Webhook you want to update.</param>
		/// <param name="enabled">Indicates if the signature verification should be enbladle or not.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The async task.</returns>
		public static Task ToggleEventWebhookSignatureVerificationAsync(this IWebhookSettings webhookSettings, string id, bool enabled, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return webhookSettings.ToggleEventWebhookSignatureVerificationAsync(null, enabled, onBehalfOf, cancellationToken);
		}

		/// <summary>
		/// Get the signed events public key.
		/// </summary>
		/// <param name="webhookSettings">The webhook settings resource.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The public key.
		/// </returns>
		public static Task<string> GetSignedEventsPublicKeyAsync(this IWebhookSettings webhookSettings, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return webhookSettings.GetSignedEventsPublicKeyAsync(null, onBehalfOf, cancellationToken);
		}

		/// <summary>
		/// Adds and configures a StrongGrid client to the specified <see cref="IServiceCollection"/>.
		/// </summary>
		/// <remarks>This method registers the StrongGrid client as a scoped service in the dependency injection
		/// container. It also configures an <see cref="HttpClient"/> with the specified name, which the StrongGrid client
		/// will use for making HTTP requests. The <see cref="ILogger{TCategoryName}"/> for the StrongGrid client is also
		/// resolved from the service provider.</remarks>
		/// <param name="services">The service collection to which the StrongGrid client will be added.</param>
		/// <param name="apiKey">The API key used to authenticate with the StrongGrid service. This value cannot be null or empty.</param>
		/// <param name="clientOptions">Optional configuration options for the StrongGrid client. If not provided, default options will be used.</param>
		/// <param name="httpClientName">The name of the <see cref="HttpClient"/> to be used by the StrongGrid client. Defaults to "StrongGrid".</param>
		/// <returns>An <see cref="IHttpClientBuilder"/> that can be used to further configure the underlying <see cref="HttpClient"/>.</returns>
		public static IHttpClientBuilder AddStrongGrid(this IServiceCollection services, string apiKey, StrongGridClientOptions clientOptions = null, string httpClientName = "StrongGrid")
		{
			return services.AddStrongGrid(apiKey, null, clientOptions, httpClientName);
		}

		/// <summary>
		/// Adds and configures a StrongGrid client to the specified <see cref="IServiceCollection"/>.
		/// </summary>
		/// <remarks>This method registers the StrongGrid client as a scoped service in the dependency injection
		/// container. It also configures an <see cref="HttpClient"/> with the specified proxy settings and associates it with
		/// the provided <paramref name="httpClientName"/>.</remarks>
		/// <param name="services">The <see cref="IServiceCollection"/> to which the StrongGrid client will be added.</param>
		/// <param name="apiKey">The API key used to authenticate with the StrongGrid service.</param>
		/// <param name="proxy">The <see cref="IWebProxy"/> to use for HTTP requests. Pass <see langword="null"/> to disable proxy usage.</param>
		/// <param name="clientOptions">Optional configuration options for the StrongGrid client. If <see langword="null"/>, default options will be used.</param>
		/// <param name="httpClientName">The name of the <see cref="HttpClient"/> to be used by the StrongGrid client. Defaults to "StrongGrid".</param>
		/// <returns>An <see cref="IHttpClientBuilder"/> that can be used to further configure the underlying <see cref="HttpClient"/>.</returns>
		public static IHttpClientBuilder AddStrongGrid(this IServiceCollection services, string apiKey, IWebProxy proxy, StrongGridClientOptions clientOptions = null, string httpClientName = "StrongGrid")
		{
			var httpClientBuilder = services
				.AddHttpClient(httpClientName)
				.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
				{
					Proxy = proxy,
					UseProxy = proxy != null,
				});

			services.AddScoped<StrongGrid.IClient>(serviceProvider =>
			{
				var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(httpClientName);
				var logger = serviceProvider.GetRequiredService<ILogger<StrongGrid.Client>>();
				return new StrongGrid.Client(apiKey, httpClient, clientOptions, logger);
			});

			return httpClientBuilder;
		}

		/// <summary>
		/// Adds and configures a StrongGrid client to the specified <see cref="IServiceCollection"/>.
		/// </summary>
		/// <remarks>This method registers the StrongGrid client as a scoped service in the dependency injection
		/// container. It also configures an <see cref="HttpClient"/> with the specified name, which the StrongGrid client
		/// will use for making HTTP requests. The <see cref="ILogger{TCategoryName}"/> for the StrongGrid client is also
		/// resolved from the service provider.</remarks>
		/// <param name="services">The service collection to which the StrongGrid client will be added.</param>
		/// <param name="apiKey">The API key used to authenticate with the StrongGrid service. This value cannot be null or empty.</param>
		/// <param name="clientOptions">Optional configuration options for the StrongGrid client. If not provided, default options will be used.</param>
		/// <param name="httpClientName">The name of the <see cref="HttpClient"/> to be used by the StrongGrid client. Defaults to "StrongGrid".</param>
		/// <returns>An <see cref="IHttpClientBuilder"/> that can be used to further configure the underlying <see cref="HttpClient"/>.</returns>
		[Obsolete("The legacy client, legacy resources and legacy model classes are obsolete")]
		public static IHttpClientBuilder AddLegacyStrongGrid(this IServiceCollection services, string apiKey, StrongGridClientOptions clientOptions = null, string httpClientName = "StrongGrid")
		{
			return services.AddLegacyStrongGrid(apiKey, null, clientOptions, httpClientName);
		}

		/// <summary>
		/// Adds and configures a legacy StrongGrid client to the specified <see cref="IServiceCollection"/>.
		/// </summary>
		/// <remarks>This method registers the legacy StrongGrid client as a scoped service in the dependency injection
		/// container. It also configures an <see cref="HttpClient"/> with the specified proxy settings and associates it with
		/// the provided <paramref name="httpClientName"/>.</remarks>
		/// <param name="services">The <see cref="IServiceCollection"/> to which the legacy StrongGrid client will be added.</param>
		/// <param name="apiKey">The API key used to authenticate with the StrongGrid service.</param>
		/// <param name="proxy">The <see cref="IWebProxy"/> to use for HTTP requests. Pass <see langword="null"/> to disable proxy usage.</param>
		/// <param name="clientOptions">Optional configuration options for the legacy StrongGrid client. If <see langword="null"/>, default options will be used.</param>
		/// <param name="httpClientName">The name of the <see cref="HttpClient"/> to be used by the legacy StrongGrid client. Defaults to "StrongGrid".</param>
		/// <returns>An <see cref="IHttpClientBuilder"/> that can be used to further configure the underlying <see cref="HttpClient"/>.</returns>
		[Obsolete("The legacy client, legacy resources and legacy model classes are obsolete")]
		public static IHttpClientBuilder AddLegacyStrongGrid(this IServiceCollection services, string apiKey, IWebProxy proxy, StrongGridClientOptions clientOptions = null, string httpClientName = "StrongGrid")
		{
			var httpClientBuilder = services
				.AddHttpClient(httpClientName)
				.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
				{
					Proxy = proxy,
					UseProxy = proxy != null,
				});

			services.AddScoped<StrongGrid.ILegacyClient>(serviceProvider =>
			{
				var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(httpClientName);
				var logger = serviceProvider.GetRequiredService<ILogger<StrongGrid.LegacyClient>>();
				return new StrongGrid.LegacyClient(apiKey, httpClient, clientOptions, logger);
			});

			return httpClientBuilder;
		}

		/// <summary>
		/// Configures the client options to use the European Union SendGrid API endpoint.
		/// </summary>
		/// <remarks>Use this method to direct API requests to SendGrid's European Union infrastructure, which may be
		/// required for compliance with regional data regulations.</remarks>
		/// <param name="options">The client options to configure. Cannot be null.</param>
		/// <returns>The same <see cref="StrongGridClientOptions"/> instance with the API endpoint set to the European Union endpoint.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> is null.</exception>
		public static StrongGridClientOptions WithEuropeanUnionApiEndPoint(this StrongGridClientOptions options)
		{
			if (options == null) throw new ArgumentNullException(nameof(options));
			options.ApiEndPoint = new Uri("https://api.eu.sendgrid.com/v3");
			return options;
		}
	}
}
