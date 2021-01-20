using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows access to information about the current user.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.IUser" />
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/user.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public class User : IUser
	{
		private const string _endpoint = "user/profile";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="User" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal User(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Get your user profile.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="UserProfile" />.
		/// </returns>
		public Task<UserProfile> GetProfileAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<UserProfile>();
		}

		/// <summary>
		/// Update your user profile.
		/// </summary>
		/// <param name="address">The address.</param>
		/// <param name="city">The city.</param>
		/// <param name="company">The company.</param>
		/// <param name="country">The country.</param>
		/// <param name="firstName">The first name.</param>
		/// <param name="lastName">The last name.</param>
		/// <param name="phone">The phone.</param>
		/// <param name="state">The state.</param>
		/// <param name="website">The website.</param>
		/// <param name="zip">The zip.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="UserProfile" />.
		/// </returns>
		public Task<UserProfile> UpdateProfileAsync(
			Parameter<string> address = default,
			Parameter<string> city = default,
			Parameter<string> company = default,
			Parameter<string> country = default,
			Parameter<string> firstName = default,
			Parameter<string> lastName = default,
			Parameter<string> phone = default,
			Parameter<string> state = default,
			Parameter<string> website = default,
			Parameter<string> zip = default,
			string onBehalfOf = null,
			CancellationToken cancellationToken = default)
		{
			var data = CreateJObject(address, city, company, country, firstName, lastName, phone, state, website, zip);
			return _client
				.PatchAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<UserProfile>();
		}

		/// <summary>
		/// Get your user account.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Account" />.
		/// </returns>
		public Task<Account> GetAccountAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("user/account")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<Account>();
		}

		/// <summary>
		/// Retrieve the email address on file for your account.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The email address from your user profile.
		/// </returns>
		public Task<string> GetEmailAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("user/email")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<string>("email");
		}

		/// <summary>
		/// Update the email address on file for your account.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The email address from your user profile.
		/// </returns>
		public Task<string> UpdateEmailAsync(string email, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject();
			data.Add("email", email);

			return _client
				.PutAsync("user/email")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<string>("email");
		}

		/// <summary>
		/// Retrieve your account username.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The username from your user profile.
		/// </returns>
		public Task<string> GetUsernameAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("user/username")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<string>("username");
		}

		/// <summary>
		/// Update your account username.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The username from your user profile.
		/// </returns>
		public Task<string> UpdateUsernameAsync(string username, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject();
			data.Add("username", username);

			return _client
				.PutAsync("user/username")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<string>("username");
		}

		/// <summary>
		/// Retrieve the current credit balance for your account.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="UserCredits"/>.
		/// </returns>
		public Task<UserCredits> GetCreditsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("user/credits")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<UserCredits>();
		}

		/// <summary>
		/// Update the password for your account.
		/// </summary>
		/// <param name="oldPassword">The old password.</param>
		/// <param name="newPassword">The new password.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task UpdatePasswordAsync(string oldPassword, string newPassword, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject();
			data.Add("new_password", oldPassword);
			data.Add("old_password", newPassword);

			return _client
				.PutAsync("user/password")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// List all available scopes for a user.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of string representing the permissions (aka scopes).
		/// </returns>
		public Task<string[]> GetPermissionsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("scopes")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<string[]>("scopes");
		}

		private static JObject CreateJObject(
			Parameter<string> address = default,
			Parameter<string> city = default,
			Parameter<string> company = default,
			Parameter<string> country = default,
			Parameter<string> firstName = default,
			Parameter<string> lastName = default,
			Parameter<string> phone = default,
			Parameter<string> state = default,
			Parameter<string> website = default,
			Parameter<string> zip = default)
		{
			var result = new JObject();
			result.AddPropertyIfValue("address", address);
			result.AddPropertyIfValue("city", city);
			result.AddPropertyIfValue("company", company);
			result.AddPropertyIfValue("country", country);
			result.AddPropertyIfValue("first_name", firstName);
			result.AddPropertyIfValue("last_name", lastName);
			result.AddPropertyIfValue("phone", phone);
			result.AddPropertyIfValue("state", state);
			result.AddPropertyIfValue("website", website);
			result.AddPropertyIfValue("zip", zip);
			return result;
		}
	}
}
