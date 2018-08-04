using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Utilities;
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
		public Task<Template> CreateAsync(string name, TemplateType type, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "name", name },
				{ "generation", JToken.Parse(JsonConvert.SerializeObject(type)).ToString() }
			};

			return _client
				.PostAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Template>();
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
		public Task<Template[]> GetAllAsync(TemplateType type, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync(_endpoint)
				.WithArgument("generations", JToken.Parse(JsonConvert.SerializeObject(type)).ToString())
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Template[]>("templates");
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
		public Task<Template> GetAsync(string templateId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/{templateId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Template>();
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
		public Task<Template> UpdateAsync(string templateId, string name, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "name", name }
			};
			return _client
				.PatchAsync($"{_endpoint}/{templateId}")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Template>();
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
		public Task DeleteAsync(string templateId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken))
		{
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
		public Task<TemplateVersion> CreateVersionAsync(string templateId, string name, string subject, string htmlContent, string textContent, bool isActive, Parameter<EditorType?> editorType = default(Parameter<EditorType?>), Parameter<object> testData = default(Parameter<object>), string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "name", name },
				{ "subject", subject },
				{ "html_content", htmlContent },
				{ "plain_content", textContent },
				{ "active", isActive ? 1 : 0 }
			};
			data.AddPropertyIfEnumValue("editor", editorType);
			data.AddPropertyIfValue("test_data", testData, value => JToken.Parse(JsonConvert.SerializeObject(value)).ToString());

			return _client
				.PostAsync($"{_endpoint}/{templateId}/versions")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<TemplateVersion>();
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
		public Task<TemplateVersion> ActivateVersionAsync(string templateId, string versionId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.PostAsync($"{_endpoint}/{templateId}/versions/{versionId}/activate")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<TemplateVersion>();
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
		public Task<TemplateVersion> GetVersionAsync(string templateId, string versionId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/{templateId}/versions/{versionId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<TemplateVersion>();
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
		/// <param name="editorType">The type of editor.</param>
		/// <param name="testData">For dynamic templates only, the mock data that will be used for template preview and test sends.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateVersion" />.
		/// </returns>
		public Task<TemplateVersion> UpdateVersionAsync(string templateId, string versionId, Parameter<string> name = default(Parameter<string>), Parameter<string> subject = default(Parameter<string>), Parameter<string> htmlContent = default(Parameter<string>), Parameter<string> textContent = default(Parameter<string>), Parameter<bool> isActive = default(Parameter<bool>), Parameter<EditorType?> editorType = default(Parameter<EditorType?>), Parameter<object> testData = default(Parameter<object>), string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject();
			data.AddPropertyIfValue("name", name);
			data.AddPropertyIfValue("subject", subject);
			data.AddPropertyIfValue("html_content", htmlContent);
			data.AddPropertyIfValue("plain_content", textContent);
			data.AddPropertyIfValue("active", isActive, value => JToken.FromObject(value ? 1 : 0));
			data.AddPropertyIfEnumValue("editor", editorType);
			data.AddPropertyIfValue("test_data", testData);

			return _client
				.PatchAsync($"{_endpoint}/{templateId}/versions/{versionId}")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<TemplateVersion>();
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
		public Task DeleteVersionAsync(string templateId, string versionId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync($"{_endpoint}/{templateId}/versions/{versionId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}
	}
}
