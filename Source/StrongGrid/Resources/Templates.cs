using Pathoschild.Http.Client;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage templates.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.ITemplates" />
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Transactional_Templates/templates.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public class Templates : ITemplates
	{
		private const string _endpoint = "templates";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Templates" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal Templates(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Create a template.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type of template.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Template" />.
		/// </returns>
		public Task<Template> CreateAsync(string name, TemplateType type, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

			var data = new StrongGridJsonObject();
			data.AddProperty("name", name);
			data.AddProperty("generation", type.ToEnumString());

			return _client
				.PostAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<Template>();
		}

		/// <summary>
		/// Retrieve all templates.
		/// </summary>
		/// <param name="type">The type of template.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Template" />.
		/// </returns>
		public Task<Template[]> GetAllAsync(TemplateType type, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync(_endpoint)
				.WithArgument("generations", type.ToEnumString())
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<Template[]>("templates");
		}

		/// <summary>
		/// Retrieve a template.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Template" />.
		/// </returns>
		public Task<Template> GetAsync(string templateId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(templateId)) throw new ArgumentNullException(nameof(templateId));

			return _client
				.GetAsync($"{_endpoint}/{templateId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<Template>();
		}

		/// <summary>
		/// Update a template.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Template" />.
		/// </returns>
		public Task<Template> UpdateAsync(string templateId, string name, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(templateId)) throw new ArgumentNullException(nameof(templateId));
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

			var data = new StrongGridJsonObject();
			data.AddProperty("name", name);

			return _client
				.PatchAsync($"{_endpoint}/{templateId}")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<Template>();
		}

		/// <summary>
		/// Delete a template.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string templateId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(templateId)) throw new ArgumentNullException(nameof(templateId));

			return _client
				.DeleteAsync($"{_endpoint}/{templateId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Create a template version.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="isActive">if set to <c>true</c> [is active].</param>
		/// <param name="editorType">The type of editor.</param>
		/// <param name="testData">For dynamic templates only, the mock data that will be used for template preview and test sends.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateVersion" />.
		/// </returns>
		public Task<TemplateVersion> CreateVersionAsync(string templateId, string name, string subject, string htmlContent, string textContent, bool isActive, Parameter<EditorType?> editorType = default, Parameter<object> testData = default, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(templateId)) throw new ArgumentNullException(nameof(templateId));

			var data = new StrongGridJsonObject();
			data.AddProperty("name", name);
			data.AddProperty("subject", subject);
			data.AddProperty("html_content", htmlContent);
			data.AddProperty("plain_content", textContent);
			data.AddProperty("active", isActive ? 1 : 0);
			data.AddProperty("editor", editorType);
			data.AddProperty("test_data", JsonSerializer.Serialize(testData));

			return _client
				.PostAsync($"{_endpoint}/{templateId}/versions")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<TemplateVersion>();
		}

		/// <summary>
		/// Activate a version.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateVersion" />.
		/// </returns>
		public Task<TemplateVersion> ActivateVersionAsync(string templateId, string versionId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(templateId)) throw new ArgumentNullException(nameof(templateId));

			return _client
				.PostAsync($"{_endpoint}/{templateId}/versions/{versionId}/activate")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<TemplateVersion>();
		}

		/// <summary>
		/// Retrieve a template version.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateVersion" />.
		/// </returns>
		public Task<TemplateVersion> GetVersionAsync(string templateId, string versionId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(templateId)) throw new ArgumentNullException(nameof(templateId));
			if (string.IsNullOrEmpty(versionId)) throw new ArgumentNullException(nameof(versionId));

			return _client
				.GetAsync($"{_endpoint}/{templateId}/versions/{versionId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<TemplateVersion>();
		}

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
		/// <param name="testData">For dynamic templates only, the mock data that will be used for template preview and test sends.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateVersion" />.
		/// </returns>
		public Task<TemplateVersion> UpdateVersionAsync(string templateId, string versionId, Parameter<string> name = default, Parameter<string> subject = default, Parameter<string> htmlContent = default, Parameter<string> textContent = default, Parameter<bool> isActive = default, Parameter<object> testData = default, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(templateId)) throw new ArgumentNullException(nameof(templateId));
			if (string.IsNullOrEmpty(versionId)) throw new ArgumentNullException(nameof(versionId));

			var data = new StrongGridJsonObject();
			data.AddProperty("name", name);
			data.AddProperty("subject", subject);
			data.AddProperty("html_content", htmlContent);
			data.AddProperty("plain_content", textContent);
			data.AddProperty("active", isActive ? 1 : 0);
			data.AddProperty("test_data", testData);

			return _client
				.PatchAsync($"{_endpoint}/{templateId}/versions/{versionId}")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<TemplateVersion>();
		}

		/// <summary>
		/// Delete a template version.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteVersionAsync(string templateId, string versionId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(templateId)) throw new ArgumentNullException(nameof(templateId));
			if (string.IsNullOrEmpty(versionId)) throw new ArgumentNullException(nameof(versionId));

			return _client
				.DeleteAsync($"{_endpoint}/{templateId}/versions/{versionId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}
	}
}
