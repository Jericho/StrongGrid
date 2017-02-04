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
		private const string _endpoint = "contactdb/custom_fields";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomFields" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		public CustomFields(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Create a custom field.
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
				.WithBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<CustomFieldMetadata>();
		}

		/// <summary>
		/// Retrieve all custom fields.
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
		/// Retrieve a custom field.
		/// </summary>
		/// <param name="fieldId">The field identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="CustomFieldMetadata">metadata</see> about the field.
		/// </returns>
		public Task<CustomFieldMetadata> GetAsync(int fieldId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/{fieldId}")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<CustomFieldMetadata>();
		}

		/// <summary>
		/// Delete a custom field.
		/// </summary>
		/// <param name="fieldId">The field identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(int fieldId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync($"{_endpoint}/{fieldId}")
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Retrieve the reserved fields.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Field" />.
		/// </returns>
		public Task<Field[]> GetReservedFieldsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync("contactdb/reserved_fields")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Field[]>("reserved_fields");
		}
	}
}
