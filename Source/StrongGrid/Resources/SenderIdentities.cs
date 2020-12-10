using Pathoschild.Http.Client;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
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
		private const string _endpoint = "verified_senders";
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
			if (string.IsNullOrEmpty(nickname)) throw new ArgumentNullException(nameof(nickname));
			if (string.IsNullOrEmpty(address1)) throw new ArgumentNullException(nameof(address1));
			if (string.IsNullOrEmpty(city)) throw new ArgumentNullException(nameof(city));
			if (string.IsNullOrEmpty(country)) throw new ArgumentNullException(nameof(country));

			if (from == null) throw new ArgumentNullException(nameof(from));
			if (string.IsNullOrEmpty(from.Email)) throw new ArgumentException("You must provide the sender's email address", nameof(from.Email));
			if (string.IsNullOrEmpty(from.Name)) throw new ArgumentException("You must provide the sender's name", nameof(from.Name));

			if (replyTo == null) throw new ArgumentNullException(nameof(replyTo));
			if (string.IsNullOrEmpty(replyTo.Email)) throw new ArgumentException("You must provide the 'reply to' email address", nameof(replyTo.Email));

			var data = ConvertToJson(nickname, from, replyTo, address1, address2, city, state, zip, country);

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
				.AsObject<SenderIdentity[]>("results");
		}

		/// <summary>
		/// Determine which of SendGrid's verification processes have been completed for an account.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// Two boolean values: the first value indicates whether a sender had been verified and the second value indicates whether a domain has been verified.
		/// </returns>
		public async Task<(bool SenderVerified, bool DomainVerified)> GetCompletedStepsAsync(CancellationToken cancellationToken = default)
		{
			var completedSteps = await _client
				.GetAsync("verified_senders/steps_completed")
				.WithCancellationToken(cancellationToken)
				.AsRawJsonDocument()
				.ConfigureAwait(false);

			var results = completedSteps.RootElement.GetProperty("results");

			var senderVerified = results.GetProperty("sender_verified").GetBoolean();
			var domainVerified = results.GetProperty("domain_verified").GetBoolean();

			return (senderVerified, domainVerified);
		}

		private static StrongGridJsonObject ConvertToJson(
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
			var result = new StrongGridJsonObject();
			result.AddProperty("nickname", nickname);
			result.AddProperty("from_email", from.Value?.Email);
			result.AddProperty("from_name", from.Value?.Name);
			result.AddProperty("reply_to", replyTo);
			result.AddProperty("reply_to_name", replyTo.Value?.Name);
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
