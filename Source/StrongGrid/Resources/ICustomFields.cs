using StrongGrid.Model;
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
	public interface ICustomFields
	{
		/// <summary>
		/// Create a custom field.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The <see cref="CustomFieldMetadata">metadata</see> about the new field.</returns>
		Task<CustomFieldMetadata> CreateAsync(string name, FieldType type, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve all custom fields.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="CustomFieldMetadata">metadata</see> about the fields.
		/// </returns>
		Task<CustomFieldMetadata[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve a custom field.
		/// </summary>
		/// <param name="fieldId">The field identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="CustomFieldMetadata">metadata</see> about the field.
		/// </returns>
		Task<CustomFieldMetadata> GetAsync(int fieldId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete a custom field.
		/// </summary>
		/// <param name="fieldId">The field identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(int fieldId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve the reserved fields.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Field" />.
		/// </returns>
		Task<Field[]> GetReservedFieldsAsync(CancellationToken cancellationToken = default(CancellationToken));
	}
}
