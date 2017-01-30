using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage custom fields.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/contactdb.html
	/// </remarks>
	public class CustomFields
	{
		private readonly string _endpoint;
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomFields" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public CustomFields(Pathoschild.Http.Client.IClient client, string endpoint = "/contactdb/custom_fields")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Creates the asynchronous.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The <see cref="CustomFieldMetadata">metadata</see> about the new field.</returns>
		public Task<CustomFieldMetadata> CreateAsync(string name, FieldType type, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "name", name },
				{ "type", JToken.Parse(JsonConvert.SerializeObject(type)).ToString() }
			};
			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<CustomFieldMetadata>();
		}

		/// <summary>
		/// Gets all asynchronous.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="CustomFieldMetadata">metadata</see> about the fields.
		/// </returns>
		public Task<CustomFieldMetadata[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync(_endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<CustomFieldMetadata[]>("custom_fields");
		}

		/// <summary>
		/// Gets the asynchronous.
		/// </summary>
		/// <param name="fieldId">The field identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="CustomFieldMetadata">metadata</see> about the field.
		/// </returns>
		public Task<CustomFieldMetadata> GetAsync(int fieldId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = $"{_endpoint}/{fieldId}";
			return _client
				.GetAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<CustomFieldMetadata>();
		}

		/// <summary>
		/// Deletes the asynchronous.
		/// </summary>
		/// <param name="fieldId">The field identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(int fieldId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = $"{_endpoint}/{fieldId}";
			return _client
				.DeleteAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Gets the reserved fields asynchronous.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Field" />.
		/// </returns>
		public Task<Field[]> GetReservedFieldsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = "/contactdb/reserved_fields";
			return _client
				.GetAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Field[]>("reserved_fields");
		}
	}
}
