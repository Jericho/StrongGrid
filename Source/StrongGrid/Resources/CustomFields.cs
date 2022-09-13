using Pathoschild.Http.Client;
using StrongGrid.Json;
using StrongGrid.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage custom fields.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.ICustomFields" />
	/// <remarks>
	/// See <a href="https://sendgrid.api-docs.io/v3.0/custom-fields">SendGrid documentation</a> for more information.
	/// </remarks>
	public class CustomFields : ICustomFields
	{
		private const string _endpoint = "marketing/field_definitions";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomFields" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal CustomFields(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Create a custom field.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The <see cref="FieldMetadata">metadata</see> about the new field.</returns>
		public Task<CustomFieldMetadata> CreateAsync(string name, FieldType type, CancellationToken cancellationToken = default)
		{
			var data = new StrongGridJsonObject();
			data.AddProperty("name", name);
			data.AddProperty("field_type", type.ToEnumString());

			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<CustomFieldMetadata>();
		}

		/// <summary>
		/// Retrieve all custom and reserved fields.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="FieldMetadata">metadata</see> about the fields.
		/// </returns>
		public async Task<FieldMetadata[]> GetAllAsync(CancellationToken cancellationToken = default)
		{
			var response = await _client
				.GetAsync(_endpoint)
				.WithCancellationToken(cancellationToken)
				.AsResponse()
				.ConfigureAwait(false);

			var reservedFields = await response.AsObject<ReservedFieldMetadata[]>("reserved_fields").ConfigureAwait(false);

			// The 'custom_fields' property is omitted when there are no custom fields.
			// Therefore it's important NOT to throw an exception if this property is missing.
			// That's why the `throwIfPropertyIsMissing' parameter is set to false
			var customFields = await response.AsObject<CustomFieldMetadata[]>("custom_fields", false).ConfigureAwait(false);

			return reservedFields.Union(customFields ?? Array.Empty<FieldMetadata>()).ToArray();
		}

		/// <summary>
		/// Update the name of a custom field.
		/// </summary>
		/// <param name="fieldId">The field identifier.</param>
		/// <param name="name">The new name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="FieldMetadata">metadata</see> about the field.
		/// </returns>
		public Task<CustomFieldMetadata> UpdateAsync(string fieldId, string name = null, CancellationToken cancellationToken = default)
		{
			var data = new StrongGridJsonObject();
			data.AddProperty("id", fieldId);
			data.AddProperty("name", name);

			return _client
				.PatchAsync($"{_endpoint}/{fieldId}")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<CustomFieldMetadata>();
		}

		/// <summary>
		/// Delete a custom field.
		/// </summary>
		/// <param name="fieldId">The field identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string fieldId, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/{fieldId}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}
	}
}
