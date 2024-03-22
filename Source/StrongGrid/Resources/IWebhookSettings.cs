using StrongGrid.Models;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to set and check webhook settings.
	/// SendGrid’s Event Webhook will notify a URL of your choice via HTTP POST with information about events that occur as SendGrid processes your email.
	/// Common uses of this data are to remove unsubscribes, react to spam reports, determine unengaged recipients, identify bounced email addresses, or create advanced analytics of your email program.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Webhooks/event.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface IWebhookSettings
	{
		/// <summary>
		/// Retrieve a single Event Webhook by ID.
		/// </summary>
		/// <param name="id">The ID of the Event Webhook you want to retrieve.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EventWebhookSettings" />.
		/// </returns>
		Task<EventWebhookSettings> GetEventWebhookSettingsAsync(string id, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get all the event webhook settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="EventWebhookSettings" />.
		/// </returns>
		Task<EventWebhookSettings[]> GetAllEventWebhookSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Create event webhook settings.
		/// </summary>
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
		/// <param name="oAuthClientId">The OAuth client ID that SendGrid will pass to your OAuth server or service provider to generate an OAuth access token. When passing data in this parameter, you must also specify the oAuthTokenUrl.</param>
		/// <param name="oAuthClientSecret">The OAuth client secret that SendGrid will pass to your OAuth server or service provider to generate an OAuth access token. This secret is needed only once to create an access token. SendGrid will store the secret, allowing you to update your client ID and Token URL without passing the secret to SendGrid again. When passing data in this parameter, you must also specify the oAuthClientId and oAuthTokenUrl.</param>
		/// <param name="oAuthTokenUrl">The URL where SendGrid will send the OAuth client ID and client secret to generate an OAuth access token. This should be your OAuth server or service provider. When passing data in this parameter, you must also specify the oAuthClientId.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EventWebhookSettings" />.
		/// </returns>
		Task<EventWebhookSettings> CreateEventWebhookSettingsAsync(
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
			string oAuthClientId = null,
			string oAuthClientSecret = null,
			string oAuthTokenUrl = null,
			string onBehalfOf = null,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Change the events settings.
		/// </summary>
		/// <param name="id">The ID of the Event Webhook you want to update.</param>
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
		/// <param name="oAuthClientId">The OAuth client ID that SendGrid will pass to your OAuth server or service provider to generate an OAuth access token. When passing data in this parameter, you must also specify the oAuthTokenUrl.</param>
		/// <param name="oAuthClientSecret">The OAuth client secret that SendGrid will pass to your OAuth server or service provider to generate an OAuth access token. This secret is needed only once to create an access token. SendGrid will store the secret, allowing you to update your client ID and Token URL without passing the secret to SendGrid again. When passing data in this parameter, you must also specify the oAuthClientId and oAuthTokenUrl.</param>
		/// <param name="oAuthTokenUrl">The URL where SendGrid will send the OAuth client ID and client secret to generate an OAuth access token. This should be your OAuth server or service provider. When passing data in this parameter, you must also specify the oAuthClientId.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EventWebhookSettings" />.
		/// </returns>
		Task<EventWebhookSettings> UpdateEventWebhookSettingsAsync(
			string id,
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
			string oAuthClientId = null,
			string oAuthClientSecret = null,
			string oAuthTokenUrl = null,
			string onBehalfOf = null,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Enable or disable signature verification for a single Event Webhook.
		/// </summary>
		/// <param name="id">The ID of the Event Webhook you want to update.</param>
		/// <param name="enabled">Indicates if the signature verification should be enbladle or not.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The async task.</returns>
		Task ToggleEventWebhookSignatureVerificationAsync(string id, bool enabled, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Sends a fake event notification post to the provided URL.
		/// </summary>
		/// <param name="id">The ID of the Event Webhook you want to retrieve.</param>
		/// <param name="url">The URL where you would like the test notification to be sent.</param>
		/// <param name="oAuthClientId">The client ID Twilio SendGrid sends to your OAuth server or service provider to generate an OAuth access token. When passing data in this parameter, you must also specify oauThokenUrl.</param>
		/// <param name="oAuthClientSecret">This value is needed only once to create an access token. SendGrid will store this secret, allowing you to update your Client ID and Token URL without passing the secret to SendGrid again. When passing data in this field, you must also specify oAuthClientId and oAuthTokenUrl.</param>
		/// <param name="oAuthTokenUrl">The URL where Twilio SendGrid sends the Client ID and Client Secret to generate an access token. This should be your OAuth server or service provider. When passing data in this parameter, you must also include oAuthClientId.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task SendEventTestAsync(string id, string url, string oAuthClientId = null, string oAuthClientSecret = null, string oAuthTokenUrl = null, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Create inbound parse settings for a hostname.
		/// </summary>
		/// <param name="hostname">A specific and unique domain or sub-domain that you have created to use exclusively to parse your incoming email. For example, parse.yourdomain.com.</param>
		/// <param name="url">The public URL where you would like SendGrid to POST the data parsed from your email. Any emails sent with the given hostname provided (whose MX records have been updated to point to SendGrid) will be parsed and POSTed to this URL.</param>
		/// <param name="spamCheck">Indicates if you would like SendGrid to check the content parsed from your emails for spam before POSTing them to your domain.</param>
		/// <param name="sendRaw">Indicates if you would like SendGrid to post the original MIME-type content of your parsed email. When this parameter is set to "false", SendGrid will send a JSON payload of the content of your email.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="InboundParseWebhookSettings" />.
		/// </returns>
		Task<InboundParseWebhookSettings> CreateInboundParseWebhookSettingsAsync(string hostname, string url, bool spamCheck = false, bool sendRaw = false, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get all the inbound parse webhook settings.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="InboundParseWebhookSettings" />.
		/// </returns>
		Task<InboundParseWebhookSettings[]> GetAllInboundParseWebhookSettingsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the inbound parse webhook settings for a specific hostname.
		/// </summary>
		/// <param name="hostname">The hostname associated with the inbound parse setting that you would like to retrieve.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="InboundParseWebhookSettings" />.
		/// </returns>
		Task<InboundParseWebhookSettings> GetInboundParseWebhookSettingsAsync(string hostname, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Update the inbound parse settings for a specific hostname.
		/// </summary>
		/// <param name="hostname">A specific and unique domain or subdomain that you have created to use exclusively to parse your incoming email. For example, parse.yourdomain.com.</param>
		/// <param name="url">The public URL where you would like SendGrid to POST the data parsed from your email. Any emails sent with the given hostname provided (whose MX records have been updated to point to SendGrid) will be parsed and POSTed to this URL.</param>
		/// <param name="spamCheck">Indicates if you would like SendGrid to check the content parsed from your emails for spam before POSTing them to your domain.</param>
		/// <param name="sendRaw">Indicates if you would like SendGrid to post the original MIME-type content of your parsed email. When this parameter is set to "false", SendGrid will send a JSON payload of the content of your email.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="InboundParseWebhookSettings" />.
		/// </returns>
		Task UpdateInboundParseWebhookSettingsAsync(string hostname, Parameter<string> url = default, Parameter<bool> spamCheck = default, Parameter<bool> sendRaw = default, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete the inbound parse webhook settings for a specific hostname.
		/// </summary>
		/// <param name="hostname">The hostname associated with the inbound parse setting that you want to delete.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteInboundParseWebhookSettingsAsync(string hostname, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the signed events public key.
		/// </summary>
		/// <param name="id">The ID of the Event Webhook you want to retrieve.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The public key.
		/// </returns>
		Task<string> GetSignedEventsPublicKeyAsync(string id, CancellationToken cancellationToken = default);
	}
}
