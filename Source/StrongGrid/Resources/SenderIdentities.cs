using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
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
	public class SenderIdentities : ISenderIdentities
	{
		private const string _endpoint = "senders";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="SenderIdentities" /> class.
		/// </summary>
		/// <param name="client">The HTTP client</param>
		public SenderIdentities(Pathoschild.Http.Client.IClient client)
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
		public Task<SenderIdentity> CreateAsync(string nickname, MailAddress from, MailAddress replyTo, string address1, string address2, string city, string state, string zip, string country, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = CreateJObject(nickname, from, replyTo, address1, address2, city, state, zip, country);

			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SenderIdentity>();
		}

		/// <summary>
		/// Retrieve all sender identities.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="SenderIdentity" />.
		/// </returns>
		public Task<SenderIdentity[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync(_endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SenderIdentity[]>();
		}

		/// <summary>
		/// Retrieve a sender identity.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SenderIdentity" />.
		/// </returns>
		public Task<SenderIdentity> GetAsync(long senderId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/{senderId}")
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
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SenderIdentity" />.
		/// </returns>
		public Task<SenderIdentity> UpdateAsync(
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
			CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = CreateJObject(nickname, from, replyTo, address1, address2, city, state, zip, country);

			return _client
				.PatchAsync($"{_endpoint}/{senderId}")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SenderIdentity>();
		}

		/// <summary>
		/// Delete a sender identity.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(long senderId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync($"{_endpoint}/{senderId}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Resend the verification to a sender.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task ResendVerification(long senderId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.PostAsync($"{_endpoint}/{senderId}/resend_verification")
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
			if (nickname.HasValue) result.Add("nickname", nickname.Value);
			if (from.HasValue) result.Add("from", from.Value == null ? null : JToken.FromObject(from.Value));
			if (replyTo.HasValue) result.Add("reply_to", from.Value == null ? null : JToken.FromObject(replyTo.Value));
			if (address1.HasValue) result.Add("address", address1.Value);
			if (address2.HasValue) result.Add("address2", address2.Value);
			if (city.HasValue) result.Add("city", city.Value);
			if (state.HasValue) result.Add("state", state.Value);
			if (zip.HasValue) result.Add("zip", zip.Value);
			if (country.HasValue) result.Add("country", country.Value);
			return result;
		}
	}
}
