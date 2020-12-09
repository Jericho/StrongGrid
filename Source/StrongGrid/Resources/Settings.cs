using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to set and check the status of user settings.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.ISettings" />
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/index.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public class Settings : ISettings
	{
		private const string _endpoint = "settings";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Settings" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal Settings(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Get the current Enforced TLS settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EnforcedTlsSettings" />.
		/// </returns>
		public Task<EnforcedTlsSettings> GetEnforcedTlsSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("user/settings/enforced_tls")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<EnforcedTlsSettings>();
		}

		/// <summary>
		/// Change the Enforced TLS settings.
		/// </summary>
		/// <param name="requireTls">if set to <c>true</c> [require TLS].</param>
		/// <param name="requireValidCert">if set to <c>true</c> [require valid cert].</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EnforcedTlsSettings" />.
		/// </returns>
		public Task<EnforcedTlsSettings> UpdateEnforcedTlsSettingsAsync(bool requireTls, bool requireValidCert, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var enforcedTlsSettings = new EnforcedTlsSettings
			{
				RequireTls = requireTls,
				RequireValidCertificate = requireValidCert
			};
			var data = JObject.FromObject(enforcedTlsSettings);
			return _client
				.PatchAsync("user/settings/enforced_tls")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<EnforcedTlsSettings>();
		}

		/// <summary>
		/// Get Partner Settings.
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="GlobalSetting" />.
		/// </returns>
		public Task<GlobalSetting[]> GetAllPartnerSettingsAsync(int limit = 25, int offset = 0, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("partner_settings")
				.OnBehalfOf(onBehalfOf)
				.WithArgument("limit", limit)
				.WithArgument("offset", offset)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<GlobalSetting[]>("result");
		}

		/// <summary>
		/// Get New Relic Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="NewRelicSettings" />.
		/// </returns>
		public Task<NewRelicSettings> GetNewRelicSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("partner_settings/new_relic")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<NewRelicSettings>();
		}

		/// <summary>
		/// Change the New Relic settings.
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="licenseKey">The license key.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="NewRelicSettings" />.
		/// </returns>
		public Task<NewRelicSettings> UpdateNewRelicSettingsAsync(bool enabled, string licenseKey, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var newRelicSettings = new NewRelicSettings
			{
				Enabled = enabled,
				LicenseKey = licenseKey
			};
			var data = JObject.FromObject(newRelicSettings);
			return _client
				.PatchAsync("partner_settings/new_relic")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<NewRelicSettings>();
		}

		/// <summary>
		/// Get Tracking Settings.
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="GlobalSetting" />.
		/// </returns>
		public Task<GlobalSetting[]> GetAllTrackingSettingsAsync(int limit = 25, int offset = 0, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("tracking_settings")
				.OnBehalfOf(onBehalfOf)
				.WithArgument("limit", limit)
				.WithArgument("offset", offset)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<GlobalSetting[]>("result");
		}

		/// <summary>
		/// Get Click Tracking Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the setting is set; otherwise, <c>false</c>.
		/// </returns>
		public async Task<bool> GetClickTrackingSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var clickSettings = await _client
				.GetAsync("tracking_settings/click")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsRawJsonObject()
				.ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "enabled": false
			// }
			var isEnabled = clickSettings["enabled"].Value<bool>();
			return isEnabled;
		}

		/// <summary>
		/// Change the click tracking settings.
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the setting is set; otherwise, <c>false</c>.
		/// </returns>
		public Task<bool> UpdateClickTrackingSettingsAsync(bool enabled, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject
			{
				{ "enabled", enabled }
			};
			return _client
				.PatchAsync("tracking_settings/click")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<bool>("enabled");
		}

		/// <summary>
		/// Get Google Analytics Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="GoogleAnalyticsGlobalSettings" />.
		/// </returns>
		public Task<GoogleAnalyticsGlobalSettings> GetGoogleAnalyticsGlobalSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("tracking_settings/google_analytics")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<GoogleAnalyticsGlobalSettings>();
		}

		/// <summary>
		/// Change the New Relic settings.
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="utmSource">The utm source.</param>
		/// <param name="utmMedium">The utm medium.</param>
		/// <param name="utmTerm">The utm term.</param>
		/// <param name="utmContent">Content of the utm.</param>
		/// <param name="utmCampaign">The utm campaign.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="GoogleAnalyticsGlobalSettings" />.
		/// </returns>
		public Task<GoogleAnalyticsGlobalSettings> UpdateGoogleAnalyticsGlobalSettingsAsync(bool enabled, string utmSource, string utmMedium, string utmTerm, string utmContent, string utmCampaign, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var googleAnalyticsGlobalSettings = new GoogleAnalyticsGlobalSettings
			{
				Enabled = enabled,
				UtmSource = utmSource,
				UtmMedium = utmMedium,
				UtmTerm = utmTerm,
				UtmContent = utmContent,
				UtmCampaign = utmCampaign
			};
			var data = JObject.FromObject(googleAnalyticsGlobalSettings);
			return _client
				.PatchAsync("tracking_settings/google_analytics")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<GoogleAnalyticsGlobalSettings>();
		}

		/// <summary>
		/// Get Open Tracking Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the setting is set; otherwise, <c>false</c>.
		/// </returns>
		public Task<bool> GetOpenTrackingSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("tracking_settings/open")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<bool>("enabled");
		}

		/// <summary>
		/// Change the open tracking settings.
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the setting is set; otherwise, <c>false</c>.
		/// </returns>
		public Task<bool> UpdateOpenTrackingSettingsAsync(bool enabled, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject
			{
				{ "enabled", enabled }
			};
			return _client
				.PatchAsync("tracking_settings/open")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<bool>("enabled");
		}

		/// <summary>
		/// Get Subscription Tracking Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SubscriptionSettings" />.
		/// </returns>
		public Task<SubscriptionSettings> GetSubscriptionTrackingSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("tracking_settings/subscription")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SubscriptionSettings>();
		}

		/// <summary>
		/// Change the Subscription Tracking settings.
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="landingPageHtml">The landing page HTML.</param>
		/// <param name="url">The URL.</param>
		/// <param name="replacementTag">The replacement tag.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SubscriptionSettings" />.
		/// </returns>
		public Task<SubscriptionSettings> UpdateSubscriptionTrackingSettingsAsync(bool enabled, string landingPageHtml, string url, string replacementTag, string htmlContent, string textContent, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var subscriptionTrackingSettings = new SubscriptionSettings
			{
				Enabled = enabled,
				LandingPageHtml = landingPageHtml,
				Url = url,
				ReplacementTag = replacementTag,
				HtmlContent = htmlContent,
				TextContent = textContent
			};
			var data = JObject.FromObject(subscriptionTrackingSettings);
			return _client
				.PatchAsync("tracking_settings/subscription")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SubscriptionSettings>();
		}

		/// <summary>
		/// Get Mail Settings.
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An aray of <see cref="GlobalSetting" />.
		/// </returns>
		public Task<GlobalSetting[]> GetAllMailSettingsAsync(int limit = 25, int offset = 0, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("mail_settings")
				.OnBehalfOf(onBehalfOf)
				.WithArgument("limit", limit)
				.WithArgument("offset", offset)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<GlobalSetting[]>("result");
		}

		/// <summary>
		/// Get BCC Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EmailAddressSetting" />.
		/// </returns>
		public Task<EmailAddressSetting> GetBccMailSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("mail_settings/bcc")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<EmailAddressSetting>();
		}

		/// <summary>
		/// Change the BCC settings.
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="email">The email.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EmailAddressSetting" />.
		/// </returns>
		public Task<EmailAddressSetting> UpdateBccMailSettingsAsync(bool enabled, string email, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var bccMailSettings = new EmailAddressSetting
			{
				Enabled = enabled,
				EmailAddress = email
			};
			var data = JObject.FromObject(bccMailSettings);
			return _client
				.PatchAsync("mail_settings/bcc")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<EmailAddressSetting>();
		}

		/// <summary>
		/// Get Address Whitelist Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="AddressWhitelistSettings" />.
		/// </returns>
		public Task<AddressWhitelistSettings> GetAddressWhitelistMailSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("mail_settings/address_whitelist")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<AddressWhitelistSettings>();
		}

		/// <summary>
		/// Change the Address Whitelist settings.
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="emailAddresses">The email addresses.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="AddressWhitelistSettings" />.
		/// </returns>
		public Task<AddressWhitelistSettings> UpdateAddressWhitelistMailSettingsAsync(bool enabled, IEnumerable<string> emailAddresses, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var addressWhitelistSettings = new AddressWhitelistSettings
			{
				Enabled = enabled,
				EmailAddresses = emailAddresses.ToArray()
			};
			var data = JObject.FromObject(addressWhitelistSettings);
			return _client
				.PatchAsync("mail_settings/address_whitelist")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<AddressWhitelistSettings>();
		}

		/// <summary>
		/// Get Footer Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="FooterGlobalSettings" />.
		/// </returns>
		public Task<FooterGlobalSettings> GetFooterMailSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("mail_settings/footer")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<FooterGlobalSettings>();
		}

		/// <summary>
		/// Change the Footer settings.
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="FooterGlobalSettings" />.
		/// </returns>
		public Task<FooterGlobalSettings> UpdateFooterMailSettingsAsync(bool enabled, string htmlContent, string textContent, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var footerGlobalSetting = new FooterGlobalSettings
			{
				Enabled = enabled,
				HtmlContent = htmlContent,
				TextContent = textContent
			};
			var data = JObject.FromObject(footerGlobalSetting);
			return _client
				.PatchAsync("mail_settings/footer")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<FooterGlobalSettings>();
		}

		/// <summary>
		/// Get Forward Spam Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EmailAddressSetting" />.
		/// </returns>
		public Task<EmailAddressSetting> GetForwardSpamMailSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("mail_settings/forward_spam")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<EmailAddressSetting>();
		}

		/// <summary>
		/// Change the Forward Spam settings.
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="email">The email.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EmailAddressSetting" />.
		/// </returns>
		public Task<EmailAddressSetting> UpdateForwardSpamMailSettingsAsync(bool enabled, string email, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var forwardSpamMailSettins = new EmailAddressSetting
			{
				Enabled = enabled,
				EmailAddress = email
			};
			var data = JObject.FromObject(forwardSpamMailSettins);
			return _client
				.PatchAsync("mail_settings/forward_spam")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<EmailAddressSetting>();
		}

		/// <summary>
		/// Get Plain Content Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the setting is set; otherwise, <c>false</c>.
		/// </returns>
		public Task<bool> GetPlainContentMailSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("mail_settings/plain_content")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<bool>("enabled");
		}

		/// <summary>
		/// Change the Plain Content settings.
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the setting is set; otherwise, <c>false</c>.
		/// </returns>
		public Task<bool> UpdatePlainContentMailSettingsAsync(bool enabled, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject
			{
				{ "enabled", enabled }
			};
			return _client
				.PatchAsync("mail_settings/plain_content")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<bool>("enabled");
		}

		/// <summary>
		/// Get Spam Check Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SpamCheckingSettings" />.
		/// </returns>
		public Task<SpamCheckSettings> GetSpamCheckMailSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("mail_settings/spam_check")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SpamCheckSettings>();
		}

		/// <summary>
		/// Change the Spam Check settings.
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="postToUrl">The post to URL.</param>
		/// <param name="threshold">The threshold.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SpamCheckingSettings" />.
		/// </returns>
		public Task<SpamCheckSettings> UpdateSpamCheckMailSettingsAsync(bool enabled, string postToUrl, int threshold, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var spamCheckMailSettings = new SpamCheckSettings
			{
				Enabled = enabled,
				Url = postToUrl,
				Threshold = threshold
			};
			var data = JObject.FromObject(spamCheckMailSettings);
			return _client
				.PatchAsync("mail_settings/spam_check")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SpamCheckSettings>();
		}

		/// <summary>
		/// Get Template Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateSettings" />.
		/// </returns>
		public Task<TemplateSettings> GetTemplateMailSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("mail_settings/template")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<TemplateSettings>();
		}

		/// <summary>
		/// Change the Template settings.
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateSettings" />.
		/// </returns>
		public Task<TemplateSettings> UpdateTemplateMailSettingsAsync(bool enabled, string htmlContent, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var templateMailSettings = new TemplateSettings
			{
				Enabled = enabled,
				HtmlContent = htmlContent
			};
			var data = JObject.FromObject(templateMailSettings);
			return _client
				.PatchAsync("mail_settings/template")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<TemplateSettings>();
		}

		/// <summary>
		/// Get Bounce Purge Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="BouncePurgeSettings" />.
		/// </returns>
		public Task<BouncePurgeSettings> GetBouncePurgeMailSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("mail_settings/bounce_purge")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<BouncePurgeSettings>();
		}

		/// <summary>
		/// Change the Bounce Purge settings.
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="hardBounces">The hard bounces.</param>
		/// <param name="softBounces">The soft bounces.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="BouncePurgeSettings" />.
		/// </returns>
		public Task<BouncePurgeSettings> UpdatBouncePurgeMailSettingsAsync(bool enabled, int hardBounces, int softBounces, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var bouncePurgeSettings = new BouncePurgeSettings
			{
				Enabled = enabled,
				HardBounces = hardBounces,
				SoftBounces = softBounces
			};
			var data = JObject.FromObject(bouncePurgeSettings);
			return _client
				.PatchAsync("mail_settings/bounce_purge")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<BouncePurgeSettings>();
		}

		/// <summary>
		/// Get Forward Bounce Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EmailAddressSetting" />.
		/// </returns>
		public Task<EmailAddressSetting> GetForwardBounceMailSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("mail_settings/forward_bounce")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<EmailAddressSetting>();
		}

		/// <summary>
		/// Change the Forward Spam settings.
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="email">The email.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EmailAddressSetting" />.
		/// </returns>
		public Task<EmailAddressSetting> UpdateForwardBounceMailSettingsAsync(bool enabled, string email, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var forwardSpamMailSettings = new EmailAddressSetting
			{
				Enabled = enabled,
				EmailAddress = email
			};
			var data = JObject.FromObject(forwardSpamMailSettings);
			return _client
				.PatchAsync("mail_settings/forward_bounce")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<EmailAddressSetting>();
		}
	}
}
