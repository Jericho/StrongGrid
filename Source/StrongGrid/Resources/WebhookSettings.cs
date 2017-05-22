using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Model;
using StrongGrid.Utilities;
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
	public class WebhookSettings : IWebhookSettings
	{
		private const string _endpoint = "user/webhooks/event/settings";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="WebhookSettings" /> class.
		/// </summary>
		/// <param name="client">The HTTP client</param>
		internal WebhookSettings(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Get the current Event Webhook settings.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EventWebhookSettings" />.
		/// </returns>
		public Task<EventWebhookSettings> GetAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync(_endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<EventWebhookSettings>();
		}

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
		public Task<EventWebhookSettings> UpdateAsync(
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
			CancellationToken cancellationToken = default(CancellationToken))
		{
			var eventWebhookSettings = new EventWebhookSettings
			{
				Enabled = enabled,
				Url = url,
				Bounce = bounce,
				Click = click,
				Deferred = deferred,
				Delivered = delivered,
				Dropped = dropped,
				GroupResubscribe = groupResubscribe,
				GroupUnsubscribe = groupUnsubscribe,
				Open = open,
				Processed = processed,
				SpamReport = spamReport,
				Unsubscribe = unsubscribe
			};
			var data = JObject.FromObject(eventWebhookSettings);
			return _client
				.PatchAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<EventWebhookSettings>();
		}

		/// <summary>
		/// Sends a fake event notification post to the provided URL.
		/// </summary>
		/// <param name="url">the event notification endpoint url</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task SendTestAsync(string url, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "url", url }
			};

			return _client
				.PostAsync("user/webhooks/event/test")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}
	}
}
