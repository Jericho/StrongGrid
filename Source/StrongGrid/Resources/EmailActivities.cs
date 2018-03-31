using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to search and download a CSV of your recent email event activity.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.IEmailActivities" />
	/// <remarks>
	/// See https://sendgrid.api-docs.io/v3.0/email-activity
	/// </remarks>
	public class EmailActivities : IEmailActivities
	{
		private const string _endpoint = "messages";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="EmailActivities" /> class.
		/// </summary>
		/// <param name="client">The HTTP client</param>
		internal EmailActivities(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Get all of the details about the messages matching the criteria.
		/// </summary>
		/// <param name="limit">Number of IP activity entries to return.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		public Task<EmailActivitySummary[]> SearchMessages(int limit = 20, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync(_endpoint)
				.WithArgument("limit", limit)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<EmailActivitySummary[]>("messages");
		}
	}
}
