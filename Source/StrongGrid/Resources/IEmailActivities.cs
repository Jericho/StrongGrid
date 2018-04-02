using StrongGrid.Models;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows to search and download a CSV of your recent email event activity.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.IEmailActivities" />
	/// <remarks>
	/// See https://sendgrid.api-docs.io/v3.0/email-activity
	/// </remarks>
	public interface IEmailActivities
	{
		/// <summary>
		/// Get all of the details about the messages matching the criteria.
		/// </summary>
		/// <param name="limit">Number of IP activity entries to return.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="EmailMessageActivity" />.
		/// </returns>
		Task<EmailMessageActivity[]> SearchMessages(int limit = 20, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get all of the details about the specified message.
		/// </summary>
		/// <param name="messageId">The ID of the message that you want to see details.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="EmailMessageSummary" />.
		/// </returns>
		Task<EmailMessageSummary> GetMessageSummary(string messageId, CancellationToken cancellationToken = default(CancellationToken));
	}
}
