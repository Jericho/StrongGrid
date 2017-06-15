using StrongGrid.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// You can invite teammates or they can request access to certain scopes and you can accept
	/// or deny these requests
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/teammates.html
	/// </remarks>
	public interface ITeammates
	{
		/// <summary>
		/// Retrieve a list of all recent access requests.
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of access requests
		/// </returns>
		Task<AccessRequest[]> GetAccessRequestsAsync(int limit = 10, int offset = 0, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Deny an attempt to access your account.
		/// </summary>
		/// <param name="requestId">The ID of the request that you want to deny.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DenyAccessRequestAsync(string requestId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Approve an attempt to access your account.
		/// </summary>
		/// <param name="requestId">The ID of the request that you want to approve.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task ApproveAccessRequestAsync(string requestId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Resend a teammate invite
		/// </summary>
		/// <param name="token">The token for the invite that you want to resend.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		/// <remarks>
		/// Teammate invitations will expire after 7 days.
		/// Resending an invite will reset the expiration date.
		/// </remarks>
		Task ResendInvitationAsync(string token, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve a list of all pending teammate invitations
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An array of <see cref="TeammateInvitation" />.</returns>
		/// <remarks>
		/// Each teammate invitation is valid for 7 days.
		/// Users may resend the invite to refresh the expiration date.
		/// </remarks>
		Task<TeammateInvitation[]> GetAllPendingInvitationsAsync(CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete a pending teammate invite
		/// </summary>
		/// <param name="token">The token for the invite that you want to delete.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		/// <remarks>
		/// Each teammate invitation is valid for 7 days.
		/// Users may resend the invite to refresh the expiration date.
		/// </remarks>
		Task DeleteInvitationAsync(string token, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Send a teammate invitation via email with a predefined set of scopes, or permissions.
		/// A teammate invite will expire after 7 days, but you may resend the invite at any time
		/// to reset the expiration date.
		/// </summary>
		/// <param name="email">The email address of the teammate</param>
		/// <param name="scopes">The scopes, or permissions, the teammate will be granted</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		/// <remarks>
		/// Essentials, Legacy Lite, and Free Trial users may create up to one teammate per account.
		/// There is not a teammate limit for Pro and higher plans.
		/// </remarks>
		Task<TeammateInvitation> InviteTeammateAsync(string email, IEnumerable<string> scopes, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Send a teammate invitation via email with admin permissions.
		/// A teammate invite will expire after 7 days, but you may resend the invite at any time
		/// to reset the expiration date.
		/// </summary>
		/// <param name="email">The email address of the teammate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		/// <remarks>
		/// Essentials, Legacy Lite, and Free Trial users may create up to one teammate per account.
		/// There is not a teammate limit for Pro and higher plans.
		/// </remarks>
		Task<TeammateInvitation> InviteTeammateAsAdminAsync(string email, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve a list of all current teammates
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An array of <see cref="Teammate" />.</returns>
		Task<Teammate[]> GetAllTeammatesAsync(int limit = 10, int offset = 0, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve a specific teammate by username
		/// </summary>
		/// <param name="username">The teammate username</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The <see cref="Teammate" />.</returns>
		Task<Teammate> GetTeammateAsync(string username, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve a specific teammate by username
		/// </summary>
		/// <param name="username">The teammate username</param>
		/// <param name="scopes">The permissions to asign to the teammate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The <see cref="Teammate" />.</returns>
		Task<Teammate> UpdateTeammatePermissionsAsync(string username, IEnumerable<string> scopes, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete a teammate
		/// </summary>
		/// <param name="username">The teammate username</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteTeammateAsync(string username, CancellationToken cancellationToken = default(CancellationToken));
	}
}
