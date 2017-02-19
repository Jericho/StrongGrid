using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Model;
using StrongGrid.Utilities;
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
	public class UnsubscribeGroups
	{
		private const string _endpoint = "asm/groups";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="UnsubscribeGroups" /> class.
		/// See
		/// </summary>
		/// <param name="client">The HTTP client</param>
		public UnsubscribeGroups(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Retrieve all suppression groups associated with the user.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="SuppressionGroup" />.
		/// </returns>
		public Task<SuppressionGroup[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync(_endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SuppressionGroup[]>();
		}

		/// <summary>
		/// Get information on a single suppression group.
		/// </summary>
		/// <param name="groupId">ID of the suppression group to delete</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SuppressionGroup" />.
		/// </returns>
		public Task<SuppressionGroup> GetAsync(int groupId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/{groupId}")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SuppressionGroup>();
		}

		/// <summary>
		/// Create a new suppression group.
		/// </summary>
		/// <param name="name">The name of the new suppression group</param>
		/// <param name="description">A description of the suppression group</param>
		/// <param name="isDefault">Default value is false</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="SuppressionGroup" />.
		/// </returns>
		public Task<SuppressionGroup> CreateAsync(string name, string description, bool isDefault, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "name", name },
				{ "description", description },
				{ "is_default", isDefault }
			};
			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SuppressionGroup>();
		}

		/// <summary>
		/// Update an existing suppression group.
		/// </summary>
		/// <param name="groupId">The group identifier.</param>
		/// <param name="name">The name of the new suppression group</param>
		/// <param name="description">A description of the suppression group</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="SuppressionGroup" />.
		/// </returns>
		public Task<SuppressionGroup> UpdateAsync(int groupId, Parameter<string> name = default(Parameter<string>), Parameter<string> description = default(Parameter<string>), CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject();
			if (name.HasValue) data.Add("name", name.Value);
			if (description.HasValue) data.Add("description", description.Value);

			return _client
				.PatchAsync($"{_endpoint}/{groupId}")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SuppressionGroup>();
		}

		/// <summary>
		/// Delete a suppression group.
		/// </summary>
		/// <param name="groupId">ID of the suppression group to delete</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(int groupId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync($"{_endpoint}/{groupId}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}
	}
}
