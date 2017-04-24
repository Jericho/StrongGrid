using StrongGrid.Model;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to set and check webhook settings.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Webhooks/event.html
	/// </remarks>
	public interface IWebhookSettings
	{
		/// <summary>
		/// Get the current Event Webhook settings.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EventWebhookSettings" />.
		/// </returns>
		Task<EventWebhookSettings> GetAsync(CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Change the Event Webhook settings
		/// </summary>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="url">the webhook endpoint url</param>
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
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EventWebhookSettings" />.
		/// </returns>
		Task<EventWebhookSettings> UpdateAsync(
			bool enabled,
			string url,
			bool bounce = default(bool),
			bool click = default(bool),
			bool deferred = default(bool),
			bool delivered = default(bool),
			bool dropped = default(bool),
			bool groupResubscribe = default(bool),
			bool groupUnsubscribe = default(bool),
			bool open = default(bool),
			bool processed = default(bool),
			bool spamReport = default(bool),
			bool unsubscribe = default(bool),
			CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Sends a fake event notification post to the provided URL.
		/// </summary>
		/// <param name="url">the event notification endpoint url</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task SendTestAsync(string url, CancellationToken cancellationToken = default(CancellationToken));
	}
}
