using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to set and check the status of user settings.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/index.html
	/// </remarks>
	public class Settings
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Settings" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public Settings(IClient client, string endpoint = "/settings")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Get the current Enforced TLS settings.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EnforcedTlsSettings" />.
		/// </returns>
		public async Task<EnforcedTlsSettings> GetEnforcedTlsSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/user/settings/enforced_tls", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var settings = JObject.Parse(responseContent).ToObject<EnforcedTlsSettings>();
			return settings;
		}

		/// <summary>
		/// Change the Enforced TLS settings
		/// </summary>
		/// <param name="requireTls">if set to <c>true</c> [require TLS].</param>
		/// <param name="requireValidCert">if set to <c>true</c> [require valid cert].</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EnforcedTlsSettings" />.
		/// </returns>
		public async Task<EnforcedTlsSettings> UpdateEnforcedTlsSettingsAsync(bool requireTls, bool requireValidCert, CancellationToken cancellationToken = default(CancellationToken))
		{
			var enforcedTlsSettings = new EnforcedTlsSettings
			{
				RequireTls = requireTls,
				RequireValidCertificate = requireValidCert
			};
			var data = JObject.FromObject(enforcedTlsSettings);
			var response = await _client.PatchAsync("/user/settings/enforced_tls", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var updatedSettings = JObject.Parse(responseContent).ToObject<EnforcedTlsSettings>();
			return updatedSettings;
		}

		/// <summary>
		/// Get Partner Settings
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="GlobalSetting" />.
		/// </returns>
		public async Task<GlobalSetting[]> GetAllPartnerSettingsAsync(int limit = 25, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("/partner_settings?limit={0}&offset={1}", limit, offset);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "result": [
			//     {
			//       "name": "new_relic",
			//       "title": "New Relic",
			//       "description": "lorem ipsum... .",
			//       "enabled": true
			//     }
			//   ]
			// }
			// We use a dynamic object to get rid of the 'result' property and simply return an array of partner settings
			dynamic dynamicObject = JObject.Parse(responseContent);
			dynamic dynamicArray = dynamicObject.result;

			var partnerSettings = dynamicArray.ToObject<GlobalSetting[]>();
			return partnerSettings;
		}

		/// <summary>
		/// Get New Relic Settings
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="NewRelicSettings" />.
		/// </returns>
		public async Task<NewRelicSettings> GetNewRelicSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/partner_settings/new_relic", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var newRelicSettings = JObject.Parse(responseContent).ToObject<NewRelicSettings>();
			return newRelicSettings;
		}

		/// <summary>
		/// Change the New Relic settings
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="licenseKey">The license key.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="NewRelicSettings" />.
		/// </returns>
		public async Task<NewRelicSettings> UpdateNewRelicSettingsAsync(bool enabled, string licenseKey, CancellationToken cancellationToken = default(CancellationToken))
		{
			var newRelicSettings = new NewRelicSettings
			{
				Enabled = enabled,
				LicenseKey = licenseKey
			};
			var data = JObject.FromObject(newRelicSettings);
			var response = await _client.PatchAsync("/partner_settings/new_relic", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var updatedSettings = JObject.Parse(responseContent).ToObject<NewRelicSettings>();
			return updatedSettings;
		}

		/// <summary>
		/// Get Tracking Settings
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="GlobalSetting" />.
		/// </returns>
		public async Task<GlobalSetting[]> GetAllTrackingSettingsAsync(int limit = 25, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("/tracking_settings?limit={0}&offset={1}", limit, offset);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "result": [
			//     {
			//       "name": "open",
			//       "title": "Open Tracking",
			//       "description": "lorem ipsum... .",
			//       "enabled": true
			//     }
			//  ]
			// }
			// We use a dynamic object to get rid of the 'result' property and simply return an array of partner settings
			dynamic dynamicObject = JObject.Parse(responseContent);
			dynamic dynamicArray = dynamicObject.result;

			var trackingSettings = dynamicArray.ToObject<GlobalSetting[]>();
			return trackingSettings;
		}

		/// <summary>
		/// Get Click Tracking Settings
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the setting is set; otherwise, <c>false</c>.
		/// </returns>
		public async Task<bool> GetClickTrackingSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/tracking_settings/click", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "result": [
			//     {
			//       "enabled": true
			//     }
			//   ]
			// }
			// We use a dynamic object to get rid of the 'enabled' property and simply return a boolean
			dynamic dynamicObject = JObject.Parse(responseContent);

			var isEnabled = (bool)dynamicObject.enabled;
			return isEnabled;
		}

		/// <summary>
		/// Change the click tracking settings
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the setting is set; otherwise, <c>false</c>.
		/// </returns>
		public async Task<bool> UpdateClickTrackingSettingsAsync(bool enabled, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "enabled", enabled }
			};
			var response = await _client.PatchAsync("/tracking_settings/click", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "enabled": true
			// }
			// We use a dynamic object to get rid of the 'enabled' property and simply return a boolean
			dynamic dynamicObject = JObject.Parse(responseContent);

			var isEnabled = (bool)dynamicObject.enabled;
			return isEnabled;
		}

		/// <summary>
		/// Get Google Analytics Settings
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="GoogleAnalyticsGlobalSettings" />.
		/// </returns>
		public async Task<GoogleAnalyticsGlobalSettings> GetGoogleAnalyticsGlobalSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/tracking_settings/google_analytics", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var googleAnalyticsGlobalSettings = JObject.Parse(responseContent).ToObject<GoogleAnalyticsGlobalSettings>();
			return googleAnalyticsGlobalSettings;
		}

		/// <summary>
		/// Change the New Relic settings
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="utmSource">The utm source.</param>
		/// <param name="utmMedium">The utm medium.</param>
		/// <param name="utmTerm">The utm term.</param>
		/// <param name="utmContent">Content of the utm.</param>
		/// <param name="utmCampaign">The utm campaign.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="GoogleAnalyticsGlobalSettings" />.
		/// </returns>
		public async Task<GoogleAnalyticsGlobalSettings> UpdateGoogleAnalyticsGlobalSettingsAsync(bool enabled, string utmSource, string utmMedium, string utmTerm, string utmContent, string utmCampaign, CancellationToken cancellationToken = default(CancellationToken))
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
			var response = await _client.PatchAsync("/tracking_settings/google_analytics", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var updatedSettings = JObject.Parse(responseContent).ToObject<GoogleAnalyticsGlobalSettings>();
			return updatedSettings;
		}

		/// <summary>
		/// Get Open Tracking Settings
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the setting is set; otherwise, <c>false</c>.
		/// </returns>
		public async Task<bool> GetOpenTrackingSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/tracking_settings/open", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "enabled": true
			// }
			// We use a dynamic object to get rid of the 'enabled' property and simply return a boolean
			dynamic dynamicObject = JObject.Parse(responseContent);

			var isEnabled = (bool)dynamicObject.enabled;
			return isEnabled;
		}

		/// <summary>
		/// Change the open tracking settings
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the setting is set; otherwise, <c>false</c>.
		/// </returns>
		public async Task<bool> UpdateOpenTrackingSettingsAsync(bool enabled, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "enabled", enabled }
			};
			var response = await _client.PatchAsync("/tracking_settings/open", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "enabled": true
			// }
			// We use a dynamic object to get rid of the 'enabled' property and simply return a boolean
			dynamic dynamicObject = JObject.Parse(responseContent);

			var isEnabled = (bool)dynamicObject.enabled;
			return isEnabled;
		}

		/// <summary>
		/// Get Subscription Tracking Settings
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SubscriptionSettings" />.
		/// </returns>
		public async Task<SubscriptionSettings> GetSubscriptionTrackingSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/tracking_settings/subscription", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var subscriptionSettings = JObject.Parse(responseContent).ToObject<SubscriptionSettings>();
			return subscriptionSettings;
		}

		/// <summary>
		/// Change the Subscription Tracking settings
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="landingPageHtml">The landing page HTML.</param>
		/// <param name="url">The URL.</param>
		/// <param name="replacementTag">The replacement tag.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SubscriptionSettings" />.
		/// </returns>
		public async Task<SubscriptionSettings> UpdateSubscriptionTrackingSettingsAsync(bool enabled, string landingPageHtml, string url, string replacementTag, string htmlContent, string textContent, CancellationToken cancellationToken = default(CancellationToken))
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
			var response = await _client.PatchAsync("/tracking_settings/subscription", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var subscriptionSettings = JObject.Parse(responseContent).ToObject<SubscriptionSettings>();
			return subscriptionSettings;
		}

		/// <summary>
		/// Get Mail Settings
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An aray of <see cref="GlobalSetting" />.
		/// </returns>
		public async Task<GlobalSetting[]> GetAllMailSettingsAsync(int limit = 25, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("/mail_settings?limit={0}&offset={1}", limit, offset);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "result": [
			//     {
			//       "name": "bcc",
			//       "title": "Bcc",
			//       "description": "lorem ipsum... .",
			//       "enabled": true
			//     }
			//   ]
			// }
			// We use a dynamic object to get rid of the 'result' property and simply return an array of settings
			dynamic dynamicObject = JObject.Parse(responseContent);
			dynamic dynamicArray = dynamicObject.result;

			var mailSettings = dynamicArray.ToObject<GlobalSetting[]>();
			return mailSettings;
		}

		/// <summary>
		/// Get BCC Settings
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EmailAddressSetting" />.
		/// </returns>
		public async Task<EmailAddressSetting> GetBccMailSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/mail_settings/bcc", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var bccMailSettings = JObject.Parse(responseContent).ToObject<EmailAddressSetting>();
			return bccMailSettings;
		}

		/// <summary>
		/// Change the BCC settings
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="email">The email.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EmailAddressSetting" />.
		/// </returns>
		public async Task<EmailAddressSetting> UpdateBccMailSettingsAsync(bool enabled, string email, CancellationToken cancellationToken = default(CancellationToken))
		{
			var bccMailSettings = new EmailAddressSetting
			{
				Enabled = enabled,
				EmailAddress = email
			};
			var data = JObject.FromObject(bccMailSettings);
			var response = await _client.PatchAsync("/mail_settings/bcc", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var updatedSettings = JObject.Parse(responseContent).ToObject<EmailAddressSetting>();
			return updatedSettings;
		}

		/// <summary>
		/// Get Address Whitelist Settings
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="AddressWhitelistSettings" />.
		/// </returns>
		public async Task<AddressWhitelistSettings> GetAddressWhitelistMailSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/mail_settings/address_whitelist", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var addressWhitelistSettings = JObject.Parse(responseContent).ToObject<AddressWhitelistSettings>();
			return addressWhitelistSettings;
		}

		/// <summary>
		/// Change the Address Whitelist settings
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="emailAddresses">The email addresses.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="AddressWhitelistSettings" />.
		/// </returns>
		public async Task<AddressWhitelistSettings> UpdateAddressWhitelistMailSettingsAsync(bool enabled, IEnumerable<string> emailAddresses, CancellationToken cancellationToken = default(CancellationToken))
		{
			var addressWhitelistSettings = new AddressWhitelistSettings
			{
				Enabled = enabled,
				EmailAddresses = emailAddresses.ToArray()
			};
			var data = JObject.FromObject(addressWhitelistSettings);
			var response = await _client.PatchAsync("/mail_settings/address_whitelist", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var updatedSettings = JObject.Parse(responseContent).ToObject<AddressWhitelistSettings>();
			return updatedSettings;
		}

		/// <summary>
		/// Get Footer Settings
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="FooterGlobalSettings" />.
		/// </returns>
		public async Task<FooterGlobalSettings> GetFooterMailSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/mail_settings/footer", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var bccMailSettings = JObject.Parse(responseContent).ToObject<FooterGlobalSettings>();
			return bccMailSettings;
		}

		/// <summary>
		/// Change the Footer settings
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="FooterGlobalSettings" />.
		/// </returns>
		public async Task<FooterGlobalSettings> UpdateFooterMailSettingsAsync(bool enabled, string htmlContent, string textContent, CancellationToken cancellationToken = default(CancellationToken))
		{
			var footerGlobalSetting = new FooterGlobalSettings
			{
				Enabled = enabled,
				HtmlContent = htmlContent,
				TextContent = textContent
			};
			var data = JObject.FromObject(footerGlobalSetting);
			var response = await _client.PatchAsync("/mail_settings/footer", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var updatedSettings = JObject.Parse(responseContent).ToObject<FooterGlobalSettings>();
			return updatedSettings;
		}

		/// <summary>
		/// Get Forward Spam Settings
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EmailAddressSetting" />.
		/// </returns>
		public async Task<EmailAddressSetting> GetForwardSpamMailSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/mail_settings/forward_spam", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var forwardSpamMailSettins = JObject.Parse(responseContent).ToObject<EmailAddressSetting>();
			return forwardSpamMailSettins;
		}

		/// <summary>
		/// Change the Forward Spam settings
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="email">The email.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EmailAddressSetting" />.
		/// </returns>
		public async Task<EmailAddressSetting> UpdateForwardSpamMailSettingsAsync(bool enabled, string email, CancellationToken cancellationToken = default(CancellationToken))
		{
			var forwardSpamMailSettins = new EmailAddressSetting
			{
				Enabled = enabled,
				EmailAddress = email
			};
			var data = JObject.FromObject(forwardSpamMailSettins);
			var response = await _client.PatchAsync("/mail_settings/forward_spam", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var updatedSettings = JObject.Parse(responseContent).ToObject<EmailAddressSetting>();
			return updatedSettings;
		}

		/// <summary>
		/// Get Plain Content Settings
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the setting is set; otherwise, <c>false</c>.
		/// </returns>
		public async Task<bool> GetPlainContentMailSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/mail_settings/plain_content", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "result": [
			//     {
			//       "enabled": true
			//     }
			//   ]
			// }
			// We use a dynamic object to get rid of the 'enabled' property and simply return a boolean
			dynamic dynamicObject = JObject.Parse(responseContent);

			var isEnabled = (bool)dynamicObject.enabled;
			return isEnabled;
		}

		/// <summary>
		/// Change the Plain Content settings
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the setting is set; otherwise, <c>false</c>.
		/// </returns>
		public async Task<bool> UpdatPlainContentMailSettingsAsync(bool enabled, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "enabled", enabled }
			};
			var response = await _client.PatchAsync("/mail_settings/plain_content", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "enabled": true
			// }
			// We use a dynamic object to get rid of the 'enabled' property and simply return a boolean
			dynamic dynamicObject = JObject.Parse(responseContent);

			var isEnabled = (bool)dynamicObject.enabled;
			return isEnabled;
		}

		/// <summary>
		/// Get Spam Check Settings
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SpamCheckingSettings" />.
		/// </returns>
		public async Task<SpamCheckSettings> GetSpamCheckMailSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/mail_settings/spam_check", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var spamCheckMailSettings = JObject.Parse(responseContent).ToObject<SpamCheckSettings>();
			return spamCheckMailSettings;
		}

		/// <summary>
		/// Change the Spam Check settings
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="postToUrl">The post to URL.</param>
		/// <param name="threshold">The threshold.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SpamCheckingSettings" />.
		/// </returns>
		public async Task<SpamCheckSettings> UpdateSpamCheckMailSettingsAsync(bool enabled, string postToUrl, int threshold, CancellationToken cancellationToken = default(CancellationToken))
		{
			var spamCheckMailSettings = new SpamCheckSettings
			{
				Enabled = enabled,
				Url = postToUrl,
				Threshold = threshold
			};
			var data = JObject.FromObject(spamCheckMailSettings);
			var response = await _client.PatchAsync("/mail_settings/spam_check", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var updatedSettings = JObject.Parse(responseContent).ToObject<SpamCheckSettings>();
			return updatedSettings;
		}

		/// <summary>
		/// Get Template Settings
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateSettings" />.
		/// </returns>
		public async Task<TemplateSettings> GetTemplateMailSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/mail_settings/template", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var templateMailSettings = JObject.Parse(responseContent).ToObject<TemplateSettings>();
			return templateMailSettings;
		}

		/// <summary>
		/// Change the Template settings
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateSettings" />.
		/// </returns>
		public async Task<TemplateSettings> UpdateTemplateMailSettingsAsync(bool enabled, string htmlContent, CancellationToken cancellationToken = default(CancellationToken))
		{
			var templateMailSettings = new TemplateSettings
			{
				Enabled = enabled,
				HtmlContent = htmlContent
			};
			var data = JObject.FromObject(templateMailSettings);
			var response = await _client.PatchAsync("/mail_settings/template", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var updatedSettings = JObject.Parse(responseContent).ToObject<TemplateSettings>();
			return updatedSettings;
		}

		/// <summary>
		/// Get Bounce Purge Settings
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="BouncePurgeSettings" />.
		/// </returns>
		public async Task<BouncePurgeSettings> GetBouncePurgeMailSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/mail_settings/bounce_purge", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var bouncePurgeSettings = JObject.Parse(responseContent).ToObject<BouncePurgeSettings>();
			return bouncePurgeSettings;
		}

		/// <summary>
		/// Change the Bounce Purge settings
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="hardBounces">The hard bounces.</param>
		/// <param name="softBounces">The soft bounces.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="BouncePurgeSettings" />.
		/// </returns>
		public async Task<BouncePurgeSettings> UpdatBouncePurgeMailSettingsAsync(bool enabled, int hardBounces, int softBounces, CancellationToken cancellationToken = default(CancellationToken))
		{
			var bouncePurgeSettings = new BouncePurgeSettings
			{
				Enabled = enabled,
				HardBounces = hardBounces,
				SoftBounces = softBounces
			};
			var data = JObject.FromObject(bouncePurgeSettings);
			var response = await _client.PatchAsync("/mail_settings/bounce_purge", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var updatedSettings = JObject.Parse(responseContent).ToObject<BouncePurgeSettings>();
			return updatedSettings;
		}

		/// <summary>
		/// Get Forward Bounce Settings
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EmailAddressSetting" />.
		/// </returns>
		public async Task<EmailAddressSetting> GetForwardBounceMailSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/mail_settings/forward_bounce", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var forwardSpamMailSettings = JObject.Parse(responseContent).ToObject<EmailAddressSetting>();
			return forwardSpamMailSettings;
		}

		/// <summary>
		/// Change the Forward Spam settings
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="email">The email.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EmailAddressSetting" />.
		/// </returns>
		public async Task<EmailAddressSetting> UpdateForwardBounceMailSettingsAsync(bool enabled, string email, CancellationToken cancellationToken = default(CancellationToken))
		{
			var forwardSpamMailSettings = new EmailAddressSetting
			{
				Enabled = enabled,
				EmailAddress = email
			};
			var data = JObject.FromObject(forwardSpamMailSettings);
			var response = await _client.PatchAsync("/mail_settings/forward_bounce", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var updatedSettings = JObject.Parse(responseContent).ToObject<EmailAddressSetting>();
			return updatedSettings;
		}
	}
}
