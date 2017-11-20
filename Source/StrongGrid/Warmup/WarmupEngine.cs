using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Warmup
{
	/// <summary>
	/// Engine that can warmup IP addresses according to a custom schedule.
	/// </summary>
	public class WarmupEngine : IWarmupEngine
	{
		private WarmupSettings _warmupSettings;
		private IClient _client;
		private ISystemClock _systemClock;
		private IWarmupProgressRepository _warmupProgressRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="WarmupEngine" /> class.
		/// </summary>
		/// <param name="warmupSettings">The warmup settings.</param>
		/// <param name="client">The StrongGrid client.</param>
		/// <param name="warmupProgressRepository">The repository where progress information is stored</param>
		[StrongGrid.Utilities.ExcludeFromCodeCoverage]
		public WarmupEngine(WarmupSettings warmupSettings, IClient client, IWarmupProgressRepository warmupProgressRepository)
			: this(warmupSettings, client, warmupProgressRepository, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WarmupEngine" /> class.
		/// </summary>
		/// <param name="warmupSettings">The warmup settings.</param>
		/// <param name="client">The StrongGrid client.</param>
		/// <param name="warmupProgressRepository">The repository where progress information is stored</param>
		/// <param name="systemClock">The system clock. This is for unit testing only.</param>
		internal WarmupEngine(WarmupSettings warmupSettings, IClient client, IWarmupProgressRepository warmupProgressRepository, ISystemClock systemClock = null)
		{
			_warmupSettings = warmupSettings ?? throw new ArgumentNullException(nameof(warmupSettings));
			_client = client ?? throw new ArgumentNullException(nameof(client));
			_warmupProgressRepository = warmupProgressRepository ?? throw new ArgumentNullException(nameof(warmupProgressRepository));
			_systemClock = systemClock ?? SystemClock.Instance;
		}

		/// <summary>
		/// Add IP addressses to your account and prepare a new IP pool to warmup these new IP addresses.
		/// </summary>
		/// <param name="count">The number of IPs to add to the account.</param>
		/// <param name="subusers">Array of usernames to be assigned a send IP.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public async Task PrepareWithNewIpAddressesAsync(int count, string[] subusers, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _client.IpAddresses.AddAsync(count, subusers, false, cancellationToken).ConfigureAwait(false);
			var newIpAddresses = result.IpAddresses.Select(r => r.Address).ToArray();
			await PrepareWithExistingIpAddressesAsync(newIpAddresses).ConfigureAwait(false);
		}

		/// <summary>
		/// Prepare a new IP pool to warmup the IP addresses.
		/// </summary>
		/// <param name="ipAddresses">The IP addresses to warmup.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public async Task PrepareWithExistingIpAddressesAsync(string[] ipAddresses, CancellationToken cancellationToken = default(CancellationToken))
		{
			// Create a new pool
			var newIpPool = await _client.IpPools.CreateAsync(_warmupSettings.PoolName, cancellationToken).ConfigureAwait(false);

			// Add each address to the new pool
			foreach (var ipAddress in ipAddresses)
			{
				await _client.IpPools.AddAddressAsync(newIpPool.Name, ipAddress, cancellationToken).ConfigureAwait(false);
			}

			// Record the start of process
			await _warmupProgressRepository.BeginWarmupProcessAsync(_warmupSettings.PoolName, ipAddresses, _warmupSettings.DailyVolumePerIpAddress, _warmupSettings.ResetDays, cancellationToken).ConfigureAwait(false);
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
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The message id.
		/// </returns>
		/// <remarks>
		/// This is a convenience method with simplified parameters.
		/// If you need more options, use the <see cref="SendAsync" /> method.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Too many recipients</exception>
		/// <exception cref="Exception">Email exceeds the size limit</exception>
		public Task<WarmupResult> SendToSingleRecipientAsync(
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
			MailSettings mailSettings = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			var recipients = new[] { to };
			return SendToMultipleRecipientsAsync(recipients, from, subject, htmlContent, textContent, trackOpens, trackClicks, subscriptionTracking, replyTo, attachments, templateId, sections, headers, categories, customArgs, sendAt, batchId, unsubscribeOptions, mailSettings, cancellationToken);
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
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The message id.
		/// </returns>
		/// <remarks>
		/// This is a convenience method with simplified parameters.
		/// If you need more options, use the <see cref="SendAsync" /> method.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Too many recipients</exception>
		/// <exception cref="Exception">Email exceeds the size limit</exception>
		public Task<WarmupResult> SendToMultipleRecipientsAsync(
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
			MailSettings mailSettings = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			var personalizations = recipients.Select(r => new MailPersonalization { To = new[] { r } });

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

			return SendAsync(personalizations, subject, contents, from, replyTo, attachments, templateId, sections, headers, categories, customArgs, sendAt, batchId, unsubscribeOptions, mailSettings, trackingSettings, cancellationToken);
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
		/// <param name="mailSettings">The mail settings.</param>
		/// <param name="trackingSettings">The tracking settings.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The message id.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">Too many recipients</exception>
		/// <exception cref="Exception">Email exceeds the size limit</exception>
		public async Task<WarmupResult> SendAsync(
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
			MailSettings mailSettings = null,
			TrackingSettings trackingSettings = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			/*
			var cacheDir = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
				"MyApplicationName");
			*/

			// Validate parameters
			if (personalizations == null || !personalizations.Any())
			{
				throw new ArgumentNullException(nameof(personalizations));
			}

			// Get the current warmup status
			var warmupStatus = await _warmupProgressRepository.GetWarmupStatusAsync(_warmupSettings.PoolName, cancellationToken).ConfigureAwait(false);
			var numberOfIpAddressesInThePool = warmupStatus.IpAddresses.Length;

			// Determine how many emails can be sent on the pool today
			var warmupDay = 1;
			var emailsSentLastDay = 0;

			if (warmupStatus.DateLastSent.Date != DateTime.MinValue.Date)
			{
				if (warmupStatus.Completed)
				{
					// The warmup process has already been completed
					warmupDay = 0;
					emailsSentLastDay = 0;
				}
				else if (warmupStatus.DateLastSent.Date == _systemClock.UtcNow.Date)
				{
					// The lastSendDate is "Today".
					warmupDay = warmupStatus.WarmupDay;
					emailsSentLastDay = warmupStatus.EmailsSentLastDay;
				}
				else if (warmupStatus.DateLastSent.Date.AddDays(_warmupSettings.ResetDays) >= _systemClock.UtcNow.Date)
				{
					// The lastSendDate is "Yesterday".
					// Move on to the next warmupDay's sending volume.
					warmupDay = warmupStatus.WarmupDay + 1;
					emailsSentLastDay = 0;
				}
				else
				{
					// The lastSendDate is not "Yesterday".
					// The previous warmupDay's sending volume will be
					// repeated to avoid over-sending (this is days of
					// warmup, not calendar days, which matters to ISPs)
					warmupDay = warmupStatus.WarmupDay;
					emailsSentLastDay = 0;
				}
			}

			// Check if the process is completed
			var warmupCompleted = warmupStatus.Completed || warmupDay > _warmupSettings.DailyVolumePerIpAddress.Length;

			// Calculate the number of remaining emails for today
			var maxDailyVolume = warmupCompleted ? 0 : numberOfIpAddressesInThePool * _warmupSettings.DailyVolumePerIpAddress[warmupDay - 1];
			var remainingDailyEmails = Math.Max(maxDailyVolume - emailsSentLastDay, 0);

			// Determine which emails will be sent from the pool and which ones will not
			var total = 0;
			var personalizationsWithRunningTotals =
				(from p in personalizations
				 select new
				 {
					 RunningTotal = total +=
						 p.To?.Count(r => r != null) ?? 0 +
						 p.Cc?.Count(r => r != null) ?? 0 +
						 p.Bcc?.Count(r => r != null) ?? 0,
					 Personalization = p
				 }).ToArray();
			var personalizationsOnPool = personalizationsWithRunningTotals
				.Where(p => p.RunningTotal <= remainingDailyEmails)
				.Select(p => p.Personalization)
				.ToArray();
			var personalizationsNotOnPool = personalizationsWithRunningTotals
				.Where(p => p.RunningTotal > remainingDailyEmails)
				.Select(p => p.Personalization)
				.ToArray();

			var emailsToSend = personalizationsOnPool
				.Sum(p =>
					p.To?.Count(r => r != null) ?? 0 +
					p.Cc?.Count(r => r != null) ?? 0 +
					p.Bcc?.Count(r => r != null) ?? 0);

			// The proces could also be completed if we are on the last day of the process and there won't be any emails left to send after this batch
			warmupCompleted |= warmupDay == _warmupSettings.DailyVolumePerIpAddress.Length && remainingDailyEmails - emailsToSend <= 0;

			// Send the emails
			var tasks = new Task<string>[]
			{
				// These emails (if any) will be sent on the pool
				SendEmailAsync(
					personalizationsOnPool,
					subject,
					contents,
					from,
					replyTo,
					attachments,
					templateId,
					sections,
					headers,
					categories,
					customArgs,
					sendAt,
					batchId,
					unsubscribeOptions,
					_warmupSettings.PoolName,
					mailSettings,
					trackingSettings,
					cancellationToken),

				// These emails (if any) will not be sent on the pool
				SendEmailAsync(
					personalizationsNotOnPool,
					subject,
					contents,
					from,
					replyTo,
					attachments,
					templateId,
					sections,
					headers,
					categories,
					customArgs,
					sendAt,
					batchId,
					unsubscribeOptions,
					null,
					mailSettings,
					trackingSettings,
					cancellationToken)
			};
			await Task.WhenAll(tasks).ConfigureAwait(false);

			// Update status and clean up if necesary
			if (!warmupStatus.Completed)
			{
				// Delete the pool if the process is completed
				if (warmupCompleted)
				{
					await _client.IpPools.DeleteAsync(_warmupSettings.PoolName, cancellationToken).ConfigureAwait(false);
				}

				// Update the progress info
				emailsSentLastDay += emailsToSend;
				await _warmupProgressRepository.UpdateStatusAsync(_warmupSettings.PoolName, warmupDay, _systemClock.UtcNow, emailsSentLastDay, warmupCompleted, cancellationToken);
			}

			// Return status
			return new WarmupResult(warmupCompleted, tasks[0].Result, tasks[1].Result);
		}

		private Task<string> SendEmailAsync(
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
			if (personalizations == null || !personalizations.Any())
			{
				return Task.FromResult<string>(null);
			}

			return _client.Mail
				.SendAsync(
					personalizations,
					subject,
					contents,
					from,
					replyTo,
					attachments,
					templateId,
					sections,
					headers,
					categories,
					customArgs,
					sendAt,
					batchId,
					unsubscribeOptions,
					ipPoolName,
					mailSettings,
					trackingSettings,
					cancellationToken);
		}
	}
}
