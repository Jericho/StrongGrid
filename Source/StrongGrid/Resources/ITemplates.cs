using StrongGrid.Model;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage templates.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Transactional_Templates/templates.html
	/// </remarks>
	public interface ITemplates
	{
		/// <summary>
		/// Create a template.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Template" />.
		/// </returns>
		Task<Template> CreateAsync(string name, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve all templates.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Template" />.
		/// </returns>
		Task<Template[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve a template.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Template" />.
		/// </returns>
		Task<Template> GetAsync(string templateId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Update a template.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Template" />.
		/// </returns>
		Task<Template> UpdateAsync(string templateId, string name, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete a template.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(string templateId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Create a template version.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="isActive">if set to <c>true</c> [is active].</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateVersion" />.
		/// </returns>
		Task<TemplateVersion> CreateVersionAsync(string templateId, string name, string subject, string htmlContent, string textContent, bool isActive, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Activate a version.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateVersion" />.
		/// </returns>
		Task<TemplateVersion> ActivateVersionAsync(string templateId, string versionId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve a template version.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateVersion" />.
		/// </returns>
		Task<TemplateVersion> GetVersionAsync(string templateId, string versionId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Update a template version.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="isActive">The is active.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateVersion" />.
		/// </returns>
		Task<TemplateVersion> UpdateVersionAsync(string templateId, string versionId, string name = null, string subject = null, string htmlContent = null, string textContent = null, bool? isActive = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete a template version.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteVersionAsync(string templateId, string versionId, CancellationToken cancellationToken = default(CancellationToken));
	}
}
