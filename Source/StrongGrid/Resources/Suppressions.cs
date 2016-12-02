using Newtonsoft.Json.Linq;
using StrongGrid;
using StrongGrid.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Suppressions are email addresses that can be added to groups to prevent certain types of emails
	/// from being delivered to those addresses.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Suppression_Management/suppressions.html
	/// </remarks>
	public class Suppressions
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Suppressions" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public Suppressions(IClient client, string endpoint = "/asm/groups")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Get suppressed addresses for a given group.
		/// </summary>
		/// <param name="groupId">ID of the suppression group</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of string representing the email addresses
		/// </returns>
		public async Task<string[]> GetUnsubscribedAddressesAsync(int groupId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(string.Format("{0}/{1}/suppressions", _endpoint, groupId), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var suppressedAddresses = JArray.Parse(responseContent).ToObject<string[]>();
			return suppressedAddresses;
		}

		/// <summary>
		/// Add recipient address to the suppressions list for a given group.
		/// If the group has been deleted, this request will add the address to the global suppression.
		/// </summary>
		/// <param name="groupId">ID of the suppression group</param>
		/// <param name="email">Email address to add to the suppression group</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task AddAddressToUnsubscribeGroupAsync(int groupId, string email, CancellationToken cancellationToken = default(CancellationToken))
		{
			return AddAddressToUnsubscribeGroupAsync(groupId, new[] { email }, cancellationToken);
		}

		/// <summary>
		/// Add recipient addresses to the suppressions list for a given group.
		/// If the group has been deleted, this request will add the address to the global suppression.
		/// </summary>
		/// <param name="groupId">ID of the suppression group</param>
		/// <param name="emails">Email addresses to add to the suppression group</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public async Task AddAddressToUnsubscribeGroupAsync(int groupId, IEnumerable<string> emails, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject(new JProperty("recipient_emails", JArray.FromObject(emails.ToArray())));
			var response = await _client.PostAsync(string.Format("{0}/{1}/suppressions", _endpoint, groupId), data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Delete a recipient email from the suppressions list for a group.
		/// </summary>
		/// <param name="groupId">ID of the suppression group to delete</param>
		/// <param name="email">Email address to remove from the suppression group</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public async Task RemoveAddressFromSuppressionGroupAsync(int groupId, string email, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.DeleteAsync(string.Format("{0}/{1}/suppressions/{2}", _endpoint, groupId, email), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}
	}
}
