using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Templates are re-usable email layouts, that may be created and interacted with through the API.
	/// These are intended to be a specific type of message, such as ‘Weekly Product Update’.
	/// Templates may have multiple versions with different content, these may be changed and activated
	/// through the API. These allow split testing, multiple languages of the same template, etc.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Transactional_Templates/templates.html
	/// </remarks>
	public class Templates
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Templates" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public Templates(IClient client, string endpoint = "/templates")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Creates the asynchronous.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Template" />.
		/// </returns>
		public async Task<Template> CreateAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "name", name }
			};
			var response = await _client.PostAsync(_endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var template = JObject.Parse(responseContent).ToObject<Template>();
			return template;
		}

		/// <summary>
		/// Gets all asynchronous.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Template" />.
		/// </returns>
		public async Task<Template[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(_endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "templates": [
			//     {
			//       "id": "e8ac01d5-a07a-4a71-b14c-4721136fe6aa",
			//       "name": "example template name",
			//       "versions": [
			//         {
			//           "id": "5997fcf6-2b9f-484d-acd5-7e9a99f0dc1f",
			//           "template_id": "9c59c1fb-931a-40fc-a658-50f871f3e41c",
			//           "active": 1,
			//           "name": "example version name",
			//           "updated_at": "2014-03-19 18:56:33"
			//         }
			//       ]
			//     }
			//   ]
			// }
			// We use a dynamic object to get rid of the 'templates' property and simply return an array of templates
			dynamic dynamicObject = JObject.Parse(responseContent);
			dynamic dynamicArray = dynamicObject.templates;

			var templates = dynamicArray.ToObject<Template[]>();
			return templates;
		}

		/// <summary>
		/// Gets the asynchronous.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Template" />.
		/// </returns>
		public async Task<Template> GetAsync(string templateId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(string.Format("{0}/{1}", _endpoint, templateId), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var template = JObject.Parse(responseContent).ToObject<Template>();
			return template;
		}

		/// <summary>
		/// Updates the asynchronous.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Template" />.
		/// </returns>
		public async Task<Template> UpdateAsync(string templateId, string name, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "name", name }
			};
			var response = await _client.PatchAsync(string.Format("{0}/{1}", _endpoint, templateId), data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var template = JObject.Parse(responseContent).ToObject<Template>();
			return template;
		}

		/// <summary>
		/// Deletes the asynchronous.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public async Task DeleteAsync(string templateId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.DeleteAsync(string.Format("{0}/{1}", _endpoint, templateId), cancellationToken);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Creates the version asynchronous.
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
		public async Task<TemplateVersion> CreateVersionAsync(string templateId, string name, string subject, string htmlContent, string textContent, bool isActive, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "name", name },
				{ "subject", subject },
				{ "html_content", htmlContent },
				{ "plain_content", textContent },
				{ "active", isActive ? 1 : 0 }
			};
			var response = await _client.PostAsync(string.Format("{0}/{1}/versions", _endpoint, templateId), data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var templateVersion = JObject.Parse(responseContent).ToObject<TemplateVersion>();
			return templateVersion;
		}

		/// <summary>
		/// Activates the version asynchronous.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateVersion" />.
		/// </returns>
		public async Task<TemplateVersion> ActivateVersionAsync(string templateId, string versionId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.PostAsync(string.Format("{0}/{1}/versions/{2}/activate", _endpoint, templateId, versionId), (JObject)null, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var templateVersion = JObject.Parse(responseContent).ToObject<TemplateVersion>();
			return templateVersion;
		}

		/// <summary>
		/// Gets the version asynchronous.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="TemplateVersion" />.
		/// </returns>
		public async Task<TemplateVersion> GetVersionAsync(string templateId, string versionId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(string.Format("{0}/{1}/versions/{2}", _endpoint, templateId, versionId), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var templateVersion = JObject.Parse(responseContent).ToObject<TemplateVersion>();
			return templateVersion;
		}

		/// <summary>
		/// Updates the version asynchronous.
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
		public async Task<TemplateVersion> UpdateVersionAsync(string templateId, string versionId, string name = null, string subject = null, string htmlContent = null, string textContent = null, bool? isActive = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject();
			if (!string.IsNullOrEmpty(name)) data.Add("name", name);
			if (!string.IsNullOrEmpty(subject)) data.Add("subject", subject);
			if (!string.IsNullOrEmpty(htmlContent)) data.Add("html_content", htmlContent);
			if (!string.IsNullOrEmpty(textContent)) data.Add("plain_content", textContent);
			if (isActive.HasValue) data.Add("active", isActive.Value ? 1 : 0);

			var response = await _client.PatchAsync(string.Format("{0}/{1}/versions/{2}", _endpoint, templateId, versionId), data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var templateVersion = JObject.Parse(responseContent).ToObject<TemplateVersion>();
			return templateVersion;
		}

		/// <summary>
		/// Deletes the version asynchronous.
		/// </summary>
		/// <param name="templateId">The template identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public async Task DeleteVersionAsync(string templateId, string versionId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.DeleteAsync(string.Format("{0}/{1}/versions/{2}", _endpoint, templateId, versionId), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}
	}
}
