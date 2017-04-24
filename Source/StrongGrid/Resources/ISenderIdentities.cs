using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to create and manage sender identities for Marketing Campaigns.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/sender_identities.html
	/// </remarks>
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
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SenderIdentity" />.
		/// </returns>
		Task<SenderIdentity> CreateAsync(string nickname, MailAddress from, MailAddress replyTo, string address1, string address2, string city, string state, string zip, string country, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve all sender identities.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="SenderIdentity" />.
		/// </returns>
		Task<SenderIdentity[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve a sender identity.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SenderIdentity" />.
		/// </returns>
		Task<SenderIdentity> GetAsync(long senderId, CancellationToken cancellationToken = default(CancellationToken));

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
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SenderIdentity" />.
		/// </returns>
		Task<SenderIdentity> UpdateAsync(
			long senderId,
			Parameter<string> nickname = default(Parameter<string>),
			Parameter<MailAddress> from = default(Parameter<MailAddress>),
			Parameter<MailAddress> replyTo = default(Parameter<MailAddress>),
			Parameter<string> address1 = default(Parameter<string>),
			Parameter<string> address2 = default(Parameter<string>),
			Parameter<string> city = default(Parameter<string>),
			Parameter<string> state = default(Parameter<string>),
			Parameter<string> zip = default(Parameter<string>),
			Parameter<string> country = default(Parameter<string>),
			CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete a sender identity.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(long senderId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Resend the verification to a sender.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task ResendVerification(long senderId, CancellationToken cancellationToken = default(CancellationToken));
	}
}
