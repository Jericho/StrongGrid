using StrongGrid.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Warmup
{
	/// <summary>
	/// Engine that can warmup IP addresses according to a custom schedule.
	/// </summary>
	public interface IWarmupEngine
	{
		/// <summary>
		/// Add IP addressses to your account and prepare a new IP pool to warmup these new IP addresses.
		/// </summary>
		/// <param name="count">The number of IPs to add to the account.</param>
		/// <param name="subusers">Array of usernames to be assigned a send IP.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task PrepareWithNewIpAddressesAsync(int count, string[] subusers, CancellationToken cancellationToken = default);

		/// <summary>
		/// Prepare a new IP pool to warmup the specified IP addresses.
		/// </summary>
		/// <param name="ipAddresses">IP addresses to warmup.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task PrepareWithExistingIpAddressesAsync(string[] ipAddresses, CancellationToken cancellationToken = default);

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
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="trackingSettings">The tracking settings.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The result.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">Too many recipients.</exception>
		/// <exception cref="Exception">Email exceeds the size limit.</exception>
		Task<WarmupResult> SendAsync(
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
			MailSettings mailSettings = null,
			TrackingSettings trackingSettings = null,
			MailPriority priority = MailPriority.Normal,
			CancellationToken cancellationToken = default);
	}
}
