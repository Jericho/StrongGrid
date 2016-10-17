using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	public class User
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Constructs the SendGrid Users object.
		/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/user.html
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint, do not prepend slash</param>
		public User(IClient client, string endpoint = "/user/profile")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Get your user profile
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/user.html</returns>
		public async Task<UserProfile> GetProfileAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(_endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var profile = JObject.Parse(responseContent).ToObject<UserProfile>();
			return profile;
		}

		/// <summary>
		/// Update your user profile
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/user.html</returns>
		public async Task<UserProfile> UpdateProfileAsync(string address = null, string city = null, string company = null, string country = null, string firstName = null, string lastName = null, string phone = null, string state = null, string website = null, string zip = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = CreateJObjectForUserProfile(address, city, company, country, firstName, lastName, phone, state, website, zip);
			var response = await _client.PatchAsync(_endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var profile = JObject.Parse(responseContent).ToObject<UserProfile>();
			return profile;
		}

		/// <summary>
		/// Get your user account
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/user.html</returns>
		public async Task<Account> GetAccountAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/user/account", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var account = JObject.Parse(responseContent).ToObject<Account>();
			return account;
		}

		/// <summary>
		/// Retrieve the email address on file for your account
		/// </summary>
		/// <returns>https://sendgrid.com/docs/API_Reference/Web_API_v3/user.html</returns>
		public async Task<string> GetEmailAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/user/email", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// {
			//  "email": "test@example.com"
			// }
			// We use a dynamic object to get rid of the 'email' property and simply return a string
			dynamic dynamicObject = JObject.Parse(responseContent);
			var email = dynamicObject.email;
			return email;
		}

		private static JObject CreateJObjectForUserProfile(string address = null, string city = null, string company = null, string country = null, string firstName = null, string lastName = null, string phone = null, string state = null, string website = null, string zip = null)
		{
			var result = new JObject();
			if (!string.IsNullOrEmpty(address)) result.Add("address", address);
			if (!string.IsNullOrEmpty(city)) result.Add("city", city);
			if (!string.IsNullOrEmpty(company)) result.Add("company", company);
			if (!string.IsNullOrEmpty(country)) result.Add("country", country);
			if (!string.IsNullOrEmpty(firstName)) result.Add("first_name", firstName);
			if (!string.IsNullOrEmpty(lastName)) result.Add("last_name", lastName);
			if (!string.IsNullOrEmpty(phone)) result.Add("phone", phone);
			if (!string.IsNullOrEmpty(state)) result.Add("state", state);
			if (!string.IsNullOrEmpty(website)) result.Add("website", website);
			if (!string.IsNullOrEmpty(zip)) result.Add("zip", zip);
			return result;
		}

	}
}
