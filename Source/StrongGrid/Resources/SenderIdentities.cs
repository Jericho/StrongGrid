using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/sender_identities.html
	/// </remarks>
	public class SenderIdentities
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="SenderIdentities" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public SenderIdentities(IClient client, string endpoint = "/senders")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Creates the asynchronous.
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
		/// <returns></returns>
		public async Task<SenderIdentity> CreateAsync(string nickname, MailAddress from, MailAddress replyTo, string address1, string address2, string city, string state, string zip, string country, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = CreateJObjectForSenderIdentity(nickname, from, replyTo, address1, address2, city, state, zip, country);

			var response = await _client.PostAsync(_endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var senderIdentity = JObject.Parse(responseContent).ToObject<SenderIdentity>();
			return senderIdentity;
		}

		/// <summary>
		/// Gets all asynchronous.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task<SenderIdentity[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(_endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Contrary to what the documentation says, the response looks like this:
			//  [
			//    {
			//      'id': 1,
			//      'nickname': 'My Sender ID',
			//      'from': {
			//        'email': 'from@example.com',
			//        'name': 'Example INC'
			//      },
			//      'reply_to': {
			//        'email': 'replyto@example.com',
			//        'name': 'Example INC'
			//      },
			//      'address': '123 Elm St.',
			//      'address_2': 'Apt. 456',
			//      'city': 'Denver',
			//      'state': 'Colorado',
			//      'zip': '80202',
			//      'country': 'United States',
			//      'verified': { 'status': true, 'reason': '' },
			//      'updated_at': 1449872165,
			//      'created_at': 1449872165,
			//      'locked': false
			//    }
			//  ]
			var senderIdentities = JArray.Parse(responseContent).ToObject<SenderIdentity[]>();
			return senderIdentities;
		}

		/// <summary>
		/// Gets the asynchronous.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task<SenderIdentity> GetAsync(long senderId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(string.Format("{0}/{1}", _endpoint, senderId), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var segment = JObject.Parse(responseContent).ToObject<SenderIdentity>();
			return segment;
		}

		/// <summary>
		/// Updates the asynchronous.
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
		/// <returns></returns>
		public async Task<Segment> UpdateAsync(long senderId, string nickname = null, MailAddress from = null, MailAddress replyTo = null, string address1 = null, string address2 = null, string city = null, string state = null, string zip = null, string country = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = CreateJObjectForSenderIdentity(nickname, from, replyTo, address1, address2, city, state, zip, country);

			var response = await _client.PatchAsync(string.Format("{0}/{1}", _endpoint, senderId), data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var segment = JObject.Parse(responseContent).ToObject<Segment>();
			return segment;
		}

		/// <summary>
		/// Deletes the asynchronous.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task DeleteAsync(long senderId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.DeleteAsync(_endpoint + "/" + senderId, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Resends the verification.
		/// </summary>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task ResendVerification(long senderId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.PostAsync(string.Format("{0}/{1}/resend_verification", _endpoint, senderId), (JObject)null, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		private static JObject CreateJObjectForSenderIdentity(string nickname = null, MailAddress from = null, MailAddress replyTo = null, string address1 = null, string address2 = null, string city = null, string state = null, string zip = null, string country = null)
		{
			var result = new JObject();
			if (!string.IsNullOrEmpty(nickname)) result.Add("nickname", nickname);
			if (from != null) result.Add("from", JToken.FromObject(from));
			if (replyTo != null) result.Add("reply_to", JToken.FromObject(replyTo));
			if (!string.IsNullOrEmpty(address1)) result.Add("address", address1);
			if (!string.IsNullOrEmpty(address2)) result.Add("address2", address2);
			if (!string.IsNullOrEmpty(city)) result.Add("city", city);
			if (!string.IsNullOrEmpty(state)) result.Add("state", state);
			if (!string.IsNullOrEmpty(zip)) result.Add("zip", zip);
			if (!string.IsNullOrEmpty(country)) result.Add("country", country);
			return result;
		}
	}
}
