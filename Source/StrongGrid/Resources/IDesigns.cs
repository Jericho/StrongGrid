using StrongGrid.Models;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage Designs.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.api-docs.io/v3.0/designs-api">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface IDesigns
	{
		/// <summary>
		/// Get an existing design.
		/// </summary>
		/// <param name="id">The design identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Design" />.
		/// </returns>
		Task<Design> GetAsync(string id, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get a pre-built design.
		/// </summary>
		/// <param name="id">The design identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Design" />.
		/// </returns>
		Task<Design> GetPrebuiltAsync(string id, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve all designs.
		/// </summary>
		/// <param name="recordsPerPage">The number of records per page.</param>
		/// <param name="pageToken">The token corresponding to a specific page of results, as provided by metadata.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Alert" />.
		/// </returns>
		Task<PaginatedResponse<Design>> GetAllAsync(int recordsPerPage = 100, string pageToken = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve all pre-built designs.
		/// </summary>
		/// <param name="recordsPerPage">The number of records per page.</param>
		/// <param name="pageToken">The token corresponding to a specific page of results, as provided by metadata.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Alert" />.
		/// </returns>
		Task<PaginatedResponse<Design>> GetAllPrebuiltAsync(int recordsPerPage = 100, string pageToken = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Create a new design by duplicating an existing design.
		/// </summary>
		/// <param name="id">The identifier of the design to be duplicated.</param>
		/// <param name="name">The name of the design that will be created.</param>
		/// <param name="editorType">The editor used in the UI.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Design" />.
		/// </returns>
		Task<Design> DuplicateAsync(string id, string name, EditorType? editorType, CancellationToken cancellationToken = default);

		/// <summary>
		/// Create a new design by duplicating a pre-built design.
		/// </summary>
		/// <param name="id">The identifier of the design to be duplicated.</param>
		/// <param name="name">The name of the design that will be created.</param>
		/// <param name="editorType">The editor used in the UI.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Design" />.
		/// </returns>
		Task<Design> DuplicatePrebuiltAsync(string id, string name, EditorType? editorType, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete a design.
		/// </summary>
		/// <param name="id">The identifier of the design.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(string id, CancellationToken cancellationToken = default);

		/// <summary>
		/// Create a new design.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="htmlContent">The HTML content.</param>
		/// <param name="plainContent">The plain text content.</param>
		/// <param name="generatePlainContent">If true, plain_content is always generated from html_content. If false, plain_content is not altered.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="editorType">The editor used in the UI.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Design" />.
		/// </returns>
		Task<Design> CreateAsync(string name, string htmlContent, Parameter<string> plainContent = default, Parameter<bool> generatePlainContent = default, Parameter<string> subject = default, EditorType editorType = EditorType.Code, Parameter<string[]> categories = default, CancellationToken cancellationToken = default);

		/// <summary>
		/// Update an existing desaign.
		/// </summary>
		/// <param name="id">The identifier of the design.</param>
		/// <param name="name">The name.</param>
		/// <param name="htmlContent">The HTML content.</param>
		/// <param name="plainContent">The plain text content.</param>
		/// <param name="generatePlainContent">If true, plain_content is always generated from html_content. If false, plain_content is not altered.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Design" />.
		/// </returns>
		Task<Design> UpdateAsync(string id, Parameter<string> name = default, Parameter<string> htmlContent = default, Parameter<string> plainContent = default, Parameter<bool> generatePlainContent = default, Parameter<string> subject = default, Parameter<string[]> categories = default, CancellationToken cancellationToken = default);
	}
}
