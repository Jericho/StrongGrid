using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to create and manage sender identities for Marketing Campaigns.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.Legacy.ISenderIdentities" />
	/// <remarks>
	/// See <a href="https://sendgrid.api-docs.io/v3.0/senders">SendGrid documentation</a> for more information.
	/// </remarks>
	public class SenderIdentities : ISenderIdentities
	{
		private const string _endpoint = "marketing/senders";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="SenderIdentities" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal SenderIdentities(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

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
		public Task<SenderIdentity> CreateAsync(string nickname, MailAddress from, MailAddress replyTo, string address1, string address2, string city, string state, string zip, string country, CancellationToken cancellationToken = default)
		{
			var data = ConvertToExpando(nickname, from, replyTo, address1, address2, city, state, zip, country);

			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<SenderIdentity>();
		}

		/// <summary>
		/// Retrieve a sender identity.
		/// </summary>
		/// <param name="senderIdentityId">The sender identity identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SenderIdentity" />.
		/// </returns>
		public Task<SenderIdentity> GetAsync(long senderIdentityId, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/{senderIdentityId}")
				.WithCancellationToken(cancellationToken)
				.AsObject<SenderIdentity>();
		}

		/// <summary>
		/// Retrieve all sender identities.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="SenderIdentity" />.
		/// </returns>
		public Task<SenderIdentity[]> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync(_endpoint)
				.WithCancellationToken(cancellationToken)
				.AsObject<SenderIdentity[]>();
		}

		private static ExpandoObject ConvertToExpando(
			Parameter<string> nickname,
			Parameter<MailAddress> from,
			Parameter<MailAddress> replyTo,
			Parameter<string> address1,
			Parameter<string> address2,
			Parameter<string> city,
			Parameter<string> state,
			Parameter<string> zip,
			Parameter<string> country)
		{
			var result = new ExpandoObject();
			result.AddProperty("nickname", nickname);
			result.AddProperty("from", from);
			result.AddProperty("reply_to", replyTo);
			result.AddProperty("address", address1);
			result.AddProperty("address2", address2);
			result.AddProperty("city", city);
			result.AddProperty("state", state);
			result.AddProperty("zip", zip);
			result.AddProperty("country", country);
			return result;
		}
	}
}
