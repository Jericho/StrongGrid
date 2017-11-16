using StrongGrid.Models;
using StrongGrid.Utilities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage suppression groups.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Suppression_Management/groups.html
	/// </remarks>
	public interface IUnsubscribeGroups
	{
		/// <summary>
		/// Retrieve all suppression groups associated with the user.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="SuppressionGroup" />.
		/// </returns>
		Task<SuppressionGroup[]> GetAllAsync(string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve the suppression groups that match the specified ids.
		/// </summary>
		/// <param name="groupIds">The Ids of the desired groups.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="SuppressionGroup" />.
		/// </returns>
		Task<SuppressionGroup[]> GetMultipleAsync(IEnumerable<int> groupIds, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get information on a single suppression group.
		/// </summary>
		/// <param name="groupId">ID of the suppression group to delete</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SuppressionGroup" />.
		/// </returns>
		Task<SuppressionGroup> GetAsync(int groupId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Create a new suppression group.
		/// </summary>
		/// <param name="name">The name of the new suppression group</param>
		/// <param name="description">A description of the suppression group</param>
		/// <param name="isDefault">Default value is false</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SuppressionGroup" />.
		/// </returns>
		Task<SuppressionGroup> CreateAsync(string name, string description, bool isDefault, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Update an existing suppression group.
		/// </summary>
		/// <param name="groupId">The group identifier.</param>
		/// <param name="name">The name of the new suppression group</param>
		/// <param name="description">A description of the suppression group</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="SuppressionGroup" />.
		/// </returns>
		Task<SuppressionGroup> UpdateAsync(int groupId, Parameter<string> name = default(Parameter<string>), Parameter<string> description = default(Parameter<string>), string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete a suppression group.
		/// </summary>
		/// <param name="groupId">ID of the suppression group to delete</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(int groupId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));
	}
}
