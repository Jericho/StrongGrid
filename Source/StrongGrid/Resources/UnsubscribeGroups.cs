using Pathoschild.Http.Client;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage suppression groups.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.IUnsubscribeGroups" />
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Suppression_Management/groups.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public class UnsubscribeGroups : IUnsubscribeGroups
	{
		private const string _endpoint = "asm/groups";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="UnsubscribeGroups" /> class.
		/// See.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal UnsubscribeGroups(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Retrieve all suppression groups associated with the user.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="SuppressionGroup" />.
		/// </returns>
		public Task<SuppressionGroup[]> GetAllAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<SuppressionGroup[]>();
		}

		/// <summary>
		/// Retrieve the suppression groups that match the specified ids.
		/// </summary>
		/// <param name="groupIds">The Ids of the desired groups.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="SuppressionGroup" />.
		/// </returns>
		public Task<SuppressionGroup[]> GetMultipleAsync(IEnumerable<int> groupIds, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (groupIds == null) throw new ArgumentNullException(nameof(groupIds));
			if (!groupIds.Any()) throw new ArgumentException("You must specify at least one group id", nameof(groupIds));

			var request = _client
				.GetAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken);

			foreach (var id in groupIds)
			{
				request.WithArgument("id", id);
			}

			return request.AsObject<SuppressionGroup[]>();
		}

		/// <summary>
		/// Get information on a single suppression group.
		/// </summary>
		/// <param name="groupId">ID of the suppression group to delete.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SuppressionGroup" />.
		/// </returns>
		public Task<SuppressionGroup> GetAsync(long groupId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/{groupId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<SuppressionGroup>();
		}

		/// <summary>
		/// Create a new suppression group.
		/// </summary>
		/// <param name="name">The name of the new suppression group.</param>
		/// <param name="description">A description of the suppression group.</param>
		/// <param name="isDefault">Default value is false.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SuppressionGroup" />.
		/// </returns>
		public Task<SuppressionGroup> CreateAsync(string name, string description, bool isDefault, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

			var data = new StrongGridJsonObject();
			data.AddProperty("name", name);
			data.AddProperty("description", description);
			data.AddProperty("is_default", isDefault);

			return _client
				.PostAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<SuppressionGroup>();
		}

		/// <summary>
		/// Update an existing suppression group.
		/// </summary>
		/// <param name="groupId">The group identifier.</param>
		/// <param name="name">The name of the new suppression group.</param>
		/// <param name="description">A description of the suppression group.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="SuppressionGroup" />.
		/// </returns>
		public Task<SuppressionGroup> UpdateAsync(long groupId, Parameter<string> name = default, Parameter<string> description = default, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new StrongGridJsonObject();
			data.AddProperty("name", name.Value);
			data.AddProperty("description", description);

			return _client
				.PatchAsync($"{_endpoint}/{groupId}")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<SuppressionGroup>();
		}

		/// <summary>
		/// Delete a suppression group.
		/// </summary>
		/// <param name="groupId">ID of the suppression group to delete.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(long groupId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/{groupId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}
	}
}
