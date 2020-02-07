using StrongGrid.Models;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources.Legacy
{
	/// <summary>
	/// Allows you to manage custom fields.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/contactdb.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface ICustomFields
	{
		/// <summary>
		/// Create a custom field.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The <see cref="Models.Legacy.CustomFieldMetadata">metadata</see> about the new field.</returns>
		Task<Models.Legacy.CustomFieldMetadata> CreateAsync(string name, FieldType type, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve all custom fields.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Models.Legacy.CustomFieldMetadata">metadata</see> about the fields.
		/// </returns>
		Task<Models.Legacy.CustomFieldMetadata[]> GetAllAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve a custom field.
		/// </summary>
		/// <param name="fieldId">The field identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Models.Legacy.CustomFieldMetadata">metadata</see> about the field.
		/// </returns>
		Task<Models.Legacy.CustomFieldMetadata> GetAsync(long fieldId, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete a custom field.
		/// </summary>
		/// <param name="fieldId">The field identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(long fieldId, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve the reserved fields.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Field" />.
		/// </returns>
		Task<Field[]> GetReservedFieldsAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);
	}
}
