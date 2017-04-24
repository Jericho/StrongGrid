using StrongGrid.Model;
using System;
using System.Collections.Generic;
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
	public interface IMail
	{
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
		Task<string> SendToSingleRecipientAsync(
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
			CancellationToken cancellationToken = default(CancellationToken));

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
		Task<string> SendToMultipleRecipientsAsync(
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
			CancellationToken cancellationToken = default(CancellationToken));

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
		Task<string> SendAsync(
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
			CancellationToken cancellationToken = default(CancellationToken));
	}
}
