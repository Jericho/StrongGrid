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
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/user.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface IUser
	{
		/// <summary>
		/// Get your user profile.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="UserProfile" />.
		/// </returns>
		Task<UserProfile> GetProfileAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

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
		Task<UserProfile> UpdateProfileAsync(
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
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Get your user account.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Account" />.
		/// </returns>
		Task<Account> GetAccountAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve the email address on file for your account.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The email address from your user profile.
		/// </returns>
		Task<string> GetEmailAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Update the email address on file for your account.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The email address from your user profile.
		/// </returns>
		Task<string> UpdateEmailAsync(string email, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve your account username.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The username from your user profile.
		/// </returns>
		Task<string> GetUsernameAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Update your account username.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The username from your user profile.
		/// </returns>
		Task<string> UpdateUsernameAsync(string username, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve the current credit balance for your account.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="UserCredits"/>.
		/// </returns>
		Task<UserCredits> GetCreditsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

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
		Task UpdatePasswordAsync(string oldPassword, string newPassword, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// List all available scopes for a user.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of string representing the permissions (aka scopes).
		/// </returns>
		Task<string[]> GetPermissionsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);
	}
}
