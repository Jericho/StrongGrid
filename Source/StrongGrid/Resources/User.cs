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
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/user.html
	/// </remarks>
	public class User : IUser
	{
		private const string _endpoint = "user/profile";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="User" /> class.
		/// </summary>
		/// <param name="client">The HTTP client</param>
		internal User(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Get your user profile
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="UserProfile" />.
		/// </returns>
		public Task<UserProfile> GetProfileAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync(_endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<UserProfile>();
		}

		/// <summary>
		/// Update your user profile
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
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="UserProfile" />.
		/// </returns>
		public Task<UserProfile> UpdateProfileAsync(
			Parameter<string> address = default(Parameter<string>),
			Parameter<string> city = default(Parameter<string>),
			Parameter<string> company = default(Parameter<string>),
			Parameter<string> country = default(Parameter<string>),
			Parameter<string> firstName = default(Parameter<string>),
			Parameter<string> lastName = default(Parameter<string>),
			Parameter<string> phone = default(Parameter<string>),
			Parameter<string> state = default(Parameter<string>),
			Parameter<string> website = default(Parameter<string>),
			Parameter<string> zip = default(Parameter<string>),
			CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = CreateJObject(address, city, company, country, firstName, lastName, phone, state, website, zip);
			return _client
				.PatchAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<UserProfile>();
		}

		/// <summary>
		/// Get your user account
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Account" />.
		/// </returns>
		public Task<Account> GetAccountAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync("user/account")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Account>();
		}

		/// <summary>
		/// Retrieve the email address on file for your account
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The email address from your user profile.
		/// </returns>
		public Task<string> GetEmailAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync("user/email")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<string>("email");
		}

		/// <summary>
		/// Update the email address on file for your account
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The email address from your user profile.
		/// </returns>
		public Task<string> UpdateEmailAsync(string email, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject();
			data.Add("email", email);

			return _client
				.PutAsync("user/email")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<string>("email");
		}

		/// <summary>
		/// Retrieve your account username
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The username from your user profile.
		/// </returns>
		public Task<string> GetUsernameAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync("user/username")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<string>("username");
		}

		/// <summary>
		/// Update your account username
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The username from your user profile.
		/// </returns>
		public Task<string> UpdateUsernameAsync(string username, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject();
			data.Add("username", username);

			return _client
				.PutAsync("user/username")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<string>("username");
		}

		/// <summary>
		/// Retrieve the current credit balance for your account
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="UserCredits"/>.
		/// </returns>
		public Task<UserCredits> GetCreditsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync("user/credits")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<UserCredits>();
		}

		/// <summary>
		/// Update the password for your account
		/// </summary>
		/// <param name="oldPassword">The old password.</param>
		/// <param name="newPassword">The new password.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task UpdatePasswordAsync(string oldPassword, string newPassword, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject();
			data.Add("new_password", oldPassword);
			data.Add("old_password", newPassword);

			return _client
				.PutAsync("user/password")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// List all available scopes for a user
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of string representing the permissions (aka scopes).
		/// </returns>
		public Task<string[]> GetPermissionsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync("scopes")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<string[]>("scopes");
		}

		private static JObject CreateJObject(
			Parameter<string> address = default(Parameter<string>),
			Parameter<string> city = default(Parameter<string>),
			Parameter<string> company = default(Parameter<string>),
			Parameter<string> country = default(Parameter<string>),
			Parameter<string> firstName = default(Parameter<string>),
			Parameter<string> lastName = default(Parameter<string>),
			Parameter<string> phone = default(Parameter<string>),
			Parameter<string> state = default(Parameter<string>),
			Parameter<string> website = default(Parameter<string>),
			Parameter<string> zip = default(Parameter<string>))
		{
			var result = new JObject();
			if (address.HasValue) result.Add("address", address.Value);
			if (city.HasValue) result.Add("city", city.Value);
			if (company.HasValue) result.Add("company", company.Value);
			if (country.HasValue) result.Add("country", country.Value);
			if (firstName.HasValue) result.Add("first_name", firstName.Value);
			if (lastName.HasValue) result.Add("last_name", lastName.Value);
			if (phone.HasValue) result.Add("phone", phone.Value);
			if (state.HasValue) result.Add("state", state.Value);
			if (website) result.Add("website", website.Value);
			if (zip.HasValue) result.Add("zip", zip.Value);
			return result;
		}
	}
}
