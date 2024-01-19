using StrongGrid.Models.Webhooks;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid
{
	/// <summary>
	/// Interface for the SendGrid webhook parser which supports both 'Events' and 'Inbound emails'.
	/// </summary>
	public interface IWebhookParser
	{
		/// <summary>
		/// Parses the events webhook asynchronously.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An array of <see cref="Event">events</see>.</returns>
		Task<Event[]> ParseEventsWebhookAsync(Stream stream, CancellationToken cancellationToken = default);

		/// <summary>
		/// Parses the events webhook.
		/// </summary>
		/// <param name="requestBody">The content submitted by SendGrid's WebHook.</param>
		/// <returns>An array of <see cref="Event">events</see>.</returns>
		Event[] ParseEventsWebhook(string requestBody);

		/// <summary>Parses the inbound email webhook asynchronously.</summary>
		/// <param name="stream">The stream.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The <see cref="InboundEmail"/>.</returns>
		Task<InboundEmail> ParseInboundEmailWebhookAsync(Stream stream, CancellationToken cancellationToken = default);

		/// <summary>
		/// Parses the inbound email webhook.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns>The <see cref="InboundEmail"/>.</returns>
		[Obsolete("Use the async version of this method, it can read the content of the stream much more efficiently.")]
		InboundEmail ParseInboundEmailWebhook(Stream stream);
	}
}
