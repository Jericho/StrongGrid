using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace StrongGrid.Resources
{
	public class Settings
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Constructs the SendGrid Settings object.
		/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/index.html
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint, do not prepend slash</param>
		public Settings(IClient client, string endpoint = "/settings")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Get the current Enforced TLS settings.
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/index.html</returns>
		public async Task<EnforcedTlsSettings> GetEnforcedTlsSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(_endpoint + "/enforced_tls", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var profile = JObject.Parse(responseContent).ToObject<EnforcedTlsSettings>();
			return profile;
		}

		/// <summary>
		/// Change the Enforced TLS settings
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/enforced_tls.html</returns>
		public async Task UpdateEnforcedTlsSettingsAsync(bool requireTls, bool requireValidCert, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "require_tls", requireTls },
				{ "require_valid_cert", requireValidCert }
			};
			var response = await _client.PatchAsync(_endpoint + "/enforced_tls", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Get Partner Settings
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/partners.html</returns>
		public async Task<GlobalSetting[]> GetAllPartnerSettingsAsync(int limit = 25, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("/partner_settings?limit={0}&offset={1}", limit, offset);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "result": [
			//     {
			//       "name": "new_relic",
			//       "title": "New Relic",
			//       "description": "lorem ipsum... .",
			//       "enabled": true
			//     }
			//	]
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
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/partners.html</returns>
		public async Task<NewRelicSettings> GetNewRelicSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/partner_settings/new_relic", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var newRelicSettings = JObject.Parse(responseContent).ToObject<NewRelicSettings>();
			return newRelicSettings;
		}

		/// <summary>
		/// Change the New Relic settings
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/partners.html</returns>
		public async Task UpdateNewRelicSettingsAsync(bool enabled, string licenseKey, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "enabled", enabled },
				{ "license_key", licenseKey }
			};
			var response = await _client.PatchAsync("/partner_settings/new_relic", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Get Tracking Settings
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/tracking.html</returns>
		public async Task<GlobalSetting[]> GetAllTrackingSettingsAsync(int limit = 25, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("/tracking_settings?limit={0}&offset={1}", limit, offset);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "result": [
			//     {
			//       "name": "open",
			//       "title": "Open Tracking",
			//       "description": "lorem ipsum... .",
			//       "enabled": true
			//     }
			//	]
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
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/tracking.html</returns>
		public async Task<bool> GetClickTrackingSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/tracking_settings/click", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "result": [
			//     {
			//       "enabled": true
			//     }
			//	]
			// }
			// We use a dynamic object to get rid of the 'enabled' property and simply return a boolean
			dynamic dynamicObject = JObject.Parse(responseContent);
			
			var isEnabled = (bool)dynamicObject.enabled;
			return isEnabled;
		}

		/// <summary>
		/// Change the click tracking settings
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/tracking.html</returns>
		public async Task UpdateClickTrackingSettingsAsync(bool enabled, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "enabled", enabled }
			};
			var response = await _client.PatchAsync("/tracking_settings/click", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Get Google Analytics Settings 
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/tracking.html</returns>
		public async Task<GoogleAnalyticsGlobalSettings> GetGogleAnalyticsGlobalSettinsgAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/tracking_settings/google_analytics", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var googleAnalyticsGlobalSettings = JObject.Parse(responseContent).ToObject<GoogleAnalyticsGlobalSettings>();
			return googleAnalyticsGlobalSettings;
		}

		/// <summary>
		/// Change the New Relic settings
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/tracking.html</returns>
		public async Task UpdateGoogleAnalyticsGlobalSettingsAsync(bool enabled, string utmSource, string utmMedium, string utmTerm, string utmContent, string utmCampaign, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "enabled", enabled },
				{ "utm_source", utmSource },
				{ "utm_medium", utmMedium },
				{ "utm_term", utmTerm },
				{ "utm_content", utmContent },
				{ "utm_campaign", utmCampaign }
			};
			var response = await _client.PatchAsync("/tracking_settings/google_analytics", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Get Open Tracking Settings
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/tracking.html</returns>
		public async Task<bool> GetOpenTrackingSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/tracking_settings/open", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "result": [
			//     {
			//       "enabled": true
			//     }
			//	]
			// }
			// We use a dynamic object to get rid of the 'enabled' property and simply return a boolean
			dynamic dynamicObject = JObject.Parse(responseContent);

			var isEnabled = (bool)dynamicObject.enabled;
			return isEnabled;
		}

		/// <summary>
		/// Change the open tracking settings
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/tracking.html</returns>
		public async Task UpdateOpenTrackingSettingsAsync(bool enabled, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "enabled", enabled }
			};
			var response = await _client.PatchAsync("/tracking_settings/open", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Get Subscription Tracking Settings
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/tracking.html</returns>
		public async Task<SubscriptionSettings> GetSubscriptionTrackingSettingsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/tracking_settings/subscription", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var subscriptionSettings = JObject.Parse(responseContent).ToObject<SubscriptionSettings>();
			return subscriptionSettings;
		}

		/// <summary>
		/// Change the Subscription Tracking settings
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/tracking.html</returns>
		public async Task UpdateSubscriptionTrackingSettingsAsync(bool enabled, string landingPageHtml, string url, string replacementTag, string htmlContent, string textContent, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "enabled", enabled },
				{ "landing", landingPageHtml },
				{ "url", url },
				{ "replace", replacementTag },
				{ "html_content", htmlContent },
				{ "plain_content", textContent }
			};
			var response = await _client.PatchAsync("/tracking_settings/subscription", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}
		
		/// <summary>
		/// Get Mail Settings
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/mail.html</returns>
		public async Task<GlobalSetting[]> GetAllMailSettingsAsync(int limit = 25, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("/mail_settings?limit={0}&offset={1}", limit, offset);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "result": [
			//     {
			//       "name": "bcc",
			//       "title": "Bcc",
			//       "description": "lorem ipsum... .",
			//       "enabled": true
			//     }
			//	]
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
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/mail.html</returns>
		public async Task<BccSettings> GetBccMailSettinsgAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/mail_settings/bcc", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var bccMailSettings = JObject.Parse(responseContent).ToObject<BccSettings>();
			return bccMailSettings;
		}

		/// <summary>
		/// Change the BCC settings
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/mail.html</returns>
		public async Task UpdateBccMailSettinsgAsync(bool enabled, string email, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "enabled", enabled },
				{ "email", email }
			};
			var response = await _client.PatchAsync("/mail_settings/bcc", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Get Address Whitelist Settings 
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/mail.html</returns>
		public async Task<AddressWhitelistSettings> GetAddressWhitelistMailSettinsgAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/mail_settings/address_whitelist", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var addressWhitelistSettings = JObject.Parse(responseContent).ToObject<AddressWhitelistSettings>();
			return addressWhitelistSettings;
		}

		/// <summary>
		/// Change the Address Whitelist settings
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/mail.html</returns>
		public async Task UpdateAddressWhitelistMailSettinsgAsync(bool enabled, IEnumerable<string> emailAddresses, CancellationToken cancellationToken = default(CancellationToken))
		{
			var addressWhitelistSettings = new AddressWhitelistSettings
			{
				Enabled = enabled,
				EmailAddresses = emailAddresses.ToArray()
			};
			var data = JObject.FromObject(addressWhitelistSettings);
			var response = await _client.PatchAsync("/mail_settings/address_whitelist", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Get Footer Settings 
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/mail.html</returns>
		public async Task<FooterGlobalSettings> GetFooterMailSettinsgAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/mail_settings/footer", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var bccMailSettings = JObject.Parse(responseContent).ToObject<FooterGlobalSettings>();
			return bccMailSettings;
		}

		/// <summary>
		/// Change the Footer settings
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/mail.html</returns>
		public async Task UpdateFooterMailSettinsgAsync(bool enabled, string htmlContent, string textContent, CancellationToken cancellationToken = default(CancellationToken))
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
		}
	}
}
