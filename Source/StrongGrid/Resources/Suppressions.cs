using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Models;
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
	/// <seealso cref="StrongGrid.Resources.ISuppressions" />
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Suppression_Management/suppressions.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public class Suppressions : ISuppressions
	{
		private const string _endpoint = "asm";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Suppressions" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal Suppressions(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Get all suppressions.
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Suppression"/>.
		/// </returns>
		public Task<Suppression[]> GetAllAsync(int limit = 50, int offset = 0, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/suppressions")
				.WithArgument("limit", limit)
				.WithArgument("offset", offset)
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<Suppression[]>();
		}

		/// <summary>
		/// Get all unsubscribe groups that the given email address has been added to.
		/// </summary>
		/// <param name="email">Email address to search for across all groups.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Suppression"/>.
		/// </returns>
		public async Task<SuppressionGroup[]> GetUnsubscribedGroupsAsync(string email, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var result = await _client
				.GetAsync($"{_endpoint}/suppressions/{email}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<JObject[]>("suppressions")
				.ConfigureAwait(false);

			// SendGrid returns all the groups with a boolean property called "suppressed" indicating
			// if the specified email address is in the group or not. Therefore we need to filter the
			// result of the call to only include the groups where this boolean property is 'true'
			var unsubscribedFrom = result
				.Where(item => (bool)item["suppressed"])
				.Select(item => item.ToObject<SuppressionGroup>())
				.ToArray();

			return unsubscribedFrom;
		}

		/// <summary>
		/// Get suppressed addresses for a given group.
		/// </summary>
		/// <param name="groupId">ID of the suppression group.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of string representing the email addresses.
		/// </returns>
		public Task<string[]> GetUnsubscribedAddressesAsync(long groupId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/groups/{groupId}/suppressions")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<string[]>();
		}

		/// <summary>
		/// Add recipient address to the suppressions list for a given group.
		/// If the group has been deleted, this request will add the address to the global suppression.
		/// </summary>
		/// <param name="groupId">ID of the suppression group.</param>
		/// <param name="email">Email address to add to the suppression group.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task AddAddressToUnsubscribeGroupAsync(long groupId, string email, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return AddAddressToUnsubscribeGroupAsync(groupId, new[] { email }, onBehalfOf, cancellationToken);
		}

		/// <summary>
		/// Add recipient addresses to the suppressions list for a given group.
		/// If the group has been deleted, this request will add the address to the global suppression.
		/// </summary>
		/// <param name="groupId">ID of the suppression group.</param>
		/// <param name="emails">Email addresses to add to the suppression group.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task AddAddressToUnsubscribeGroupAsync(long groupId, IEnumerable<string> emails, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject(new JProperty("recipient_emails", JArray.FromObject(emails.ToArray())));
			return _client
				.PostAsync($"{_endpoint}/groups/{groupId}/suppressions")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Delete a recipient email from the suppressions list for a group.
		/// </summary>
		/// <param name="groupId">ID of the suppression group to delete.</param>
		/// <param name="email">Email address to remove from the suppression group.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task RemoveAddressFromSuppressionGroupAsync(long groupId, string email, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/groups/{groupId}/suppressions/{email}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Check if a recipient address is in the given suppression group.
		/// </summary>
		/// <param name="groupId">ID of the suppression group.</param>
		/// <param name="email">email address to check.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the email address is in the global suppression group; otherwise, <c>false</c>.
		/// </returns>
		public async Task<bool> IsSuppressedAsync(long groupId, string email, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject
			{
				{ "recipient_emails", JArray.FromObject(new[] { email }) }
			};
			var result = await _client
				.PostAsync($"{_endpoint}/groups/{groupId}/suppressions/search")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<string[]>()
				.ConfigureAwait(false);

			// The response contains an array with the email addresses found to be in the suppression group.
			// Therefore, we simply need to check for the presence of the email in this array
			return result.Contains(email);
		}
	}
}
