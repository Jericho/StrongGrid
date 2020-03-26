using StrongGrid.Models;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage custom fields.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.api-docs.io/v3.0/custom-fields">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface ICustomFields
	{
		/// <summary>
		/// Create a custom field.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The <see cref="FieldMetadata">metadata</see> about the new field.</returns>
		Task<FieldMetadata> CreateAsync(string name, FieldType type, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve all custom and reserved fields.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="FieldMetadata">metadata</see> about the fields.
		/// </returns>
		Task<FieldMetadata[]> GetAllAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Update the name of a custom field.
		/// </summary>
		/// <param name="fieldId">The field identifier.</param>
		/// <param name="name">The new name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="FieldMetadata">metadata</see> about the field.
		/// </returns>
		Task<FieldMetadata> UpdateAsync(string fieldId, string name = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete a custom field.
		/// </summary>
		/// <param name="fieldId">The field identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(string fieldId, CancellationToken cancellationToken = default);
	}
}
