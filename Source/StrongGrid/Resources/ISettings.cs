using StrongGrid.Models;
using StrongGrid.Utilities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to set and check the status of user settings.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Settings/index.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface ISettings
	{
		/// <summary>
		/// Get the current Enforced TLS settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EnforcedTlsSettings" />.
		/// </returns>
		Task<EnforcedTlsSettings> GetEnforcedTlsSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

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
		Task<EnforcedTlsSettings> UpdateEnforcedTlsSettingsAsync(bool requireTls, bool requireValidCert, string onBehalfOf = null, CancellationToken cancellationToken = default);

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
		Task<GlobalSetting[]> GetAllPartnerSettingsAsync(int limit = 25, int offset = 0, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get New Relic Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="NewRelicSettings" />.
		/// </returns>
		Task<NewRelicSettings> GetNewRelicSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

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
		Task<NewRelicSettings> UpdateNewRelicSettingsAsync(bool enabled, string licenseKey, string onBehalfOf = null, CancellationToken cancellationToken = default);

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
		Task<GlobalSetting[]> GetAllTrackingSettingsAsync(int limit = 25, int offset = 0, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get Click Tracking Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="ClickTrackingSettings" />.
		/// </returns>
		Task<ClickTrackingSettings> GetClickTrackingSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Change the click tracking settings.
		/// </summary>
		/// <param name="enabledInText">if set to <c>true</c>, enables click tracking in text content.</param>
		/// <param name="enabledInHtml">if set to <c>true</c>, enables click tracking in HTML content.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="ClickTrackingSettings" />.
		/// </returns>
		Task<ClickTrackingSettings> UpdateClickTrackingSettingsAsync(Parameter<bool> enabledInText = default, Parameter<bool> enabledInHtml = default, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get Google Analytics Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="GoogleAnalyticsGlobalSettings" />.
		/// </returns>
		Task<GoogleAnalyticsGlobalSettings> GetGoogleAnalyticsGlobalSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

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
		Task<GoogleAnalyticsGlobalSettings> UpdateGoogleAnalyticsGlobalSettingsAsync(bool enabled, string utmSource, string utmMedium, string utmTerm, string utmContent, string utmCampaign, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get Open Tracking Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the setting is set; otherwise, <c>false</c>.
		/// </returns>
		Task<bool> GetOpenTrackingSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Change the open tracking settings.
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the setting is set; otherwise, <c>false</c>.
		/// </returns>
		Task<bool> UpdateOpenTrackingSettingsAsync(bool enabled, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get Subscription Tracking Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SubscriptionSettings" />.
		/// </returns>
		Task<SubscriptionSettings> GetSubscriptionTrackingSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

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
		Task<SubscriptionSettings> UpdateSubscriptionTrackingSettingsAsync(bool enabled, string landingPageHtml, string url, string replacementTag, string htmlContent, string textContent, string onBehalfOf = null, CancellationToken cancellationToken = default);

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
		Task<GlobalSetting[]> GetAllMailSettingsAsync(int limit = 25, int offset = 0, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get BCC Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EmailAddressSetting" />.
		/// </returns>
		Task<EmailAddressSetting> GetBccMailSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

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
		Task<EmailAddressSetting> UpdateBccMailSettingsAsync(bool enabled, string email, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get Address Whitelist Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="AddressWhitelistSettings" />.
		/// </returns>
		Task<AddressWhitelistSettings> GetAddressWhitelistMailSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

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
		Task<AddressWhitelistSettings> UpdateAddressWhitelistMailSettingsAsync(bool enabled, IEnumerable<string> emailAddresses, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get Footer Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="FooterGlobalSettings" />.
		/// </returns>
		Task<FooterGlobalSettings> GetFooterMailSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

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
		Task<FooterGlobalSettings> UpdateFooterMailSettingsAsync(bool enabled, string htmlContent, string textContent, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get Forward Spam Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EmailAddressSetting" />.
		/// </returns>
		Task<EmailAddressSetting> GetForwardSpamMailSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

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
		Task<EmailAddressSetting> UpdateForwardSpamMailSettingsAsync(bool enabled, string email, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get Plain Content Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the setting is set; otherwise, <c>false</c>.
		/// </returns>
		Task<bool> GetPlainContentMailSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Change the Plain Content settings.
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the setting is set; otherwise, <c>false</c>.
		/// </returns>
		Task<bool> UpdatePlainContentMailSettingsAsync(bool enabled, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get Spam Check Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SpamCheckingSettings" />.
		/// </returns>
		Task<SpamCheckSettings> GetSpamCheckMailSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

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
		Task<SpamCheckSettings> UpdateSpamCheckMailSettingsAsync(bool enabled, string postToUrl, int threshold, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get Template Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateSettings" />.
		/// </returns>
		Task<TemplateSettings> GetTemplateMailSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

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
		Task<TemplateSettings> UpdateTemplateMailSettingsAsync(bool enabled, string htmlContent, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get Bounce Purge Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="BouncePurgeSettings" />.
		/// </returns>
		Task<BouncePurgeSettings> GetBouncePurgeMailSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

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
		Task<BouncePurgeSettings> UpdatBouncePurgeMailSettingsAsync(bool enabled, int hardBounces, int softBounces, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get Forward Bounce Settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EmailAddressSetting" />.
		/// </returns>
		Task<EmailAddressSetting> GetForwardBounceMailSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

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
		Task<EmailAddressSetting> UpdateForwardBounceMailSettingsAsync(bool enabled, string email, string onBehalfOf = null, CancellationToken cancellationToken = default);
	}
}
