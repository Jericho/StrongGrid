using Pathoschild.Http.Client;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
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
	public class Designs : IDesigns
	{
		private const string _endpoint = "designs";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Designs" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal Designs(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Get an existing design.
		/// </summary>
		/// <param name="id">The design identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Design" />.
		/// </returns>
		public Task<Design> GetAsync(string id, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

			return _client
				.GetAsync($"{_endpoint}/{id}")
				.WithCancellationToken(cancellationToken)
				.AsObject<Design>();
		}

		/// <summary>
		/// Get a pre-built design.
		/// </summary>
		/// <param name="id">The design identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Design" />.
		/// </returns>
		public Task<Design> GetPrebuiltAsync(string id, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

			return _client
				.GetAsync($"{_endpoint}/pre-builts/{id}")
				.WithCancellationToken(cancellationToken)
				.AsObject<Design>();
		}

		/// <summary>
		/// Retrieve all designs, except the pre-built Twilio SendGrid designs.
		/// </summary>
		/// <param name="recordsPerPage">The number of records per page.</param>
		/// <param name="pageToken">The token corresponding to a specific page of results, as provided by metadata.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Design" />.
		/// </returns>
		public Task<PaginatedResponse<Design>> GetAllAsync(int recordsPerPage = 100, string pageToken = null, CancellationToken cancellationToken = default)
		{
			var request = _client
				.GetAsync(_endpoint)
				.WithArgument("page_size", recordsPerPage)
				.WithArgument("summary", "false")
				.WithCancellationToken(cancellationToken);

			if (!string.IsNullOrEmpty(pageToken)) request.WithArgument("page_token", pageToken);

			return request.AsPaginatedResponse<Design>("result");
		}

		/// <summary>
		/// Retrieve all pre-built designs.
		/// </summary>
		/// <param name="recordsPerPage">The number of records per page.</param>
		/// <param name="pageToken">The token corresponding to a specific page of results, as provided by metadata.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Alert" />.
		/// </returns>
		public Task<PaginatedResponse<Design>> GetAllPrebuiltAsync(int recordsPerPage = 100, string pageToken = null, CancellationToken cancellationToken = default)
		{
			var request = _client
				.GetAsync($"{_endpoint}/pre-builts")
				.WithArgument("page_size", recordsPerPage)
				.WithArgument("summary", "false")
				.WithCancellationToken(cancellationToken);

			if (!string.IsNullOrEmpty(pageToken)) request.WithArgument("page_token", pageToken);

			return request.AsPaginatedResponse<Design>("result");
		}

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
		public Task<Design> DuplicateAsync(string id, string name = null, EditorType? editorType = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

			var data = ConvertToJson(name, editorType);
			return _client
				.PostAsync($"{_endpoint}/{id}")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<Design>();
		}

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
		public Task<Design> DuplicatePrebuiltAsync(string id, string name = null, EditorType? editorType = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

			var data = ConvertToJson(name, editorType);
			return _client
				.PostAsync($"{_endpoint}/pre-builts/{id}")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<Design>();
		}

		/// <summary>
		/// Delete a design.
		/// </summary>
		/// <param name="id">The identifier of the design.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

			return _client
				.DeleteAsync($"{_endpoint}/{id}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

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
		public Task<Design> CreateAsync(string name, string htmlContent, Parameter<string> plainContent = default, Parameter<bool> generatePlainContent = default, Parameter<string> subject = default, EditorType editorType = EditorType.Code, Parameter<string[]> categories = default, CancellationToken cancellationToken = default)
		{
			var data = ConvertToJson(name, editorType, htmlContent, plainContent, generatePlainContent, subject, categories);
			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<Design>();
		}

		/// <summary>
		/// Update an existing design.
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
		public Task<Design> UpdateAsync(string id, Parameter<string> name = default, Parameter<string> htmlContent = default, Parameter<string> plainContent = default, Parameter<bool> generatePlainContent = default, Parameter<string> subject = default, Parameter<string[]> categories = default, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

			var data = ConvertToJson(name, default, htmlContent, plainContent, generatePlainContent, subject, categories);
			return _client
				.PatchAsync($"{_endpoint}/{id}")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<Design>();
		}

		private static StrongGridJsonObject ConvertToJson(string name, EditorType? editorType = null, Parameter<string> htmlContent = default, Parameter<string> plainContent = default, Parameter<bool> generatePlainContent = default, Parameter<string> subject = default, Parameter<string[]> categories = default)
		{
			var result = new StrongGridJsonObject();
			result.AddProperty("name", name);
			result.AddProperty("editor", editorType);
			result.AddProperty("html_content", htmlContent);
			result.AddProperty("plain_content", plainContent);
			result.AddProperty("generate_plain_content", generatePlainContent);
			result.AddProperty("subject", subject);
			result.AddProperty("categories", categories);
			return result;
		}
	}
}
