using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources.Legacy
{
	/// <summary>
	/// Allows you to create and manage sender identities for Marketing Campaigns.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/sender_identities.html">SendGrid documentation</a> for more information.
	/// </remarks>
	[Obsolete("The legacy client, legacy resources and legacy model classes are obsolete")]
	public interface ISenderIdentities
	{
		/// <summary>
		/// Create a sender identity.
		/// </summary>
		/// <param name="nickname">The nickname.</param>
		/// <param name="from">From.</param>
		/// <param name="replyTo">The reply to.</param>
		/// <param name="address1">The address1.</param>
		/// <param name="address2">The address2.</param>
		/// <param name="city">The city.</param>
		/// <param name="state">The state.</param>
		/// <param name="zip">The zip.</param>
		/// <param name="country">The country.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SenderIdentity" />.
		/// </returns>
		Task<SenderIdentity> CreateAsync(string nickname, MailAddress from, MailAddress replyTo, string address1, string address2, string city, string state, string zip, string country, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve all sender identities.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="SenderIdentity" />.
		/// </returns>
		Task<SenderIdentity[]> GetAllAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve a sender identity.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SenderIdentity" />.
		/// </returns>
		Task<SenderIdentity> GetAsync(long senderId, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Update a sender identity.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="nickname">The nickname.</param>
		/// <param name="from">From.</param>
		/// <param name="replyTo">The reply to.</param>
		/// <param name="address1">The address1.</param>
		/// <param name="address2">The address2.</param>
		/// <param name="city">The city.</param>
		/// <param name="state">The state.</param>
		/// <param name="zip">The zip.</param>
		/// <param name="country">The country.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SenderIdentity" />.
		/// </returns>
		Task<SenderIdentity> UpdateAsync(
			long senderId,
			Parameter<string> nickname = default,
			Parameter<MailAddress> from = default,
			Parameter<MailAddress> replyTo = default,
			Parameter<string> address1 = default,
			Parameter<string> address2 = default,
			Parameter<string> city = default,
			Parameter<string> state = default,
			Parameter<string> zip = default,
			Parameter<string> country = default,
			string onBehalfOf = null,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete a sender identity.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(long senderId, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Resend the verification to a sender.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task ResendVerification(long senderId, string onBehalfOf = null, CancellationToken cancellationToken = default);
	}
}
