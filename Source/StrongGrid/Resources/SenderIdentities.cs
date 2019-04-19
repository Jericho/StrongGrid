using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to create and manage sender identities for Marketing Campaigns.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.ISenderIdentities" />
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/sender_identities.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public class SenderIdentities : ISenderIdentities
	{
		private const string _endpoint = "senders";
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
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SenderIdentity" />.
		/// </returns>
		public Task<SenderIdentity> CreateAsync(string nickname, MailAddress from, MailAddress replyTo, string address1, string address2, string city, string state, string zip, string country, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = CreateJObject(nickname, from, replyTo, address1, address2, city, state, zip, country);

			return _client
				.PostAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SenderIdentity>();
		}

		/// <summary>
		/// Retrieve all sender identities.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="SenderIdentity" />.
		/// </returns>
		public Task<SenderIdentity[]> GetAllAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SenderIdentity[]>();
		}

		/// <summary>
		/// Retrieve a sender identity.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SenderIdentity" />.
		/// </returns>
		public Task<SenderIdentity> GetAsync(long senderId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/{senderId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SenderIdentity>();
		}

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
		public Task<SenderIdentity> UpdateAsync(
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
			CancellationToken cancellationToken = default)
		{
			var data = CreateJObject(nickname, from, replyTo, address1, address2, city, state, zip, country);

			return _client
				.PatchAsync($"{_endpoint}/{senderId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SenderIdentity>();
		}

		/// <summary>
		/// Delete a sender identity.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(long senderId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/{senderId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Resend the verification to a sender.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task ResendVerification(long senderId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.PostAsync($"{_endpoint}/{senderId}/resend_verification")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		private static JObject CreateJObject(
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
			var result = new JObject();
			result.AddPropertyIfValue("nickname", nickname);
			result.AddPropertyIfValue("from", from);
			result.AddPropertyIfValue("reply_to", from);
			result.AddPropertyIfValue("address", address1);
			result.AddPropertyIfValue("address2", address2);
			result.AddPropertyIfValue("city", city);
			result.AddPropertyIfValue("state", state);
			result.AddPropertyIfValue("zip", zip);
			result.AddPropertyIfValue("country", country);
			return result;
		}
	}
}
