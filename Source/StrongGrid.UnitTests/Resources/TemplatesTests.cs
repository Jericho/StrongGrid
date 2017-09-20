using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.Resources;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class TemplatesTests
	{
		#region FIELDS

		private const string ENDPOINT = "templates";

		private const string SINGLE_TEMPLATE_JSON = @"{
			'id': 'e8ac01d5-a07a-4a71-b14c-4721136fe6aa',
			'name': 'example template name',
			'versions': [
				{
					'id': 'de37d11b-082a-42c0-9884-c0c143015a47',
					'user_id': 1234,
					'template_id': 'd51480ba-ca3f-465c-bc3e-ceb71d73c38d',
					'active': 1,
					'name': 'example version',
					'html_content': '<%body%><strong>Click to Reset</strong>',
					'plain_content': 'Click to Reset<%body%>',
					'subject': '<%subject%>',
					'updated_at': '2014-05-22 20:05:21'
				}
			]
		}";
		private const string MULTIPLE_TEMPLATES_JSON = @"{
			'templates': [
				{
					'id': 'e8ac01d5-a07a-4a71-b14c-4721136fe6aa',
					'name': 'example template name',
					'versions': [
						{
							'id': '5997fcf6-2b9f-484d-acd5-7e9a99f0dc1f',
							'template_id': '9c59c1fb-931a-40fc-a658-50f871f3e41c',
							'active': 1,
							'name': 'example version name',
							'updated_at': '2014-03-19 18:56:33'
						}
					]
				}
			]
		}";
		private const string SINGLE_TEMPLATE_VERSION_JSON = @"{
			'id': '8aefe0ee-f12b-4575-b5b7-c97e21cb36f3',
			'template_id': 'ddb96bbc-9b92-425e-8979-99464621b543',
			'active': 1,
			'name': 'example_version_name',
			'html_content': '<%body%>',
			'plain_content': '<%body%>',
			'subject': '<%subject%>',
			'updated_at': '2014-03-19 18:56:33'
		}";

		#endregion

		[Fact]
		public void Parse_Template_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<Template>(SINGLE_TEMPLATE_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.Id.ShouldBe("e8ac01d5-a07a-4a71-b14c-4721136fe6aa");
			result.Name.ShouldBe("example template name");
			result.Versions.Length.ShouldBe(1);
		}

		[Fact]
		public void Parse_TemplateVersion_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<TemplateVersion>(SINGLE_TEMPLATE_VERSION_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.HtmlContent.ShouldBe("<%body%>");
			result.Id.ShouldBe("8aefe0ee-f12b-4575-b5b7-c97e21cb36f3");
			result.IsActive.ShouldBe(true);
			result.Name.ShouldBe("example_version_name");
			result.TemplateId.ShouldBe("ddb96bbc-9b92-425e-8979-99464621b543");
			result.TextContent.ShouldBe("<%body%>");
			result.UpdatedOn.ShouldBe(new DateTime(2014, 3, 19, 18, 56, 33, DateTimeKind.Utc));
		}

		[Fact]
		public async Task CreateAsync()
		{
			// Arrange
			var name = "My template";
			var scopes = new[] { "mail.send", "alerts.create", "alerts.read" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_TEMPLATE_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var templates = new Templates(client);

			// Act
			var result = await templates.CreateAsync(name, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetAsync()
		{
			// Arrange
			var templateId = "e8ac01d5-a07a-4a71-b14c-4721136fe6aa";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, templateId)).Respond("application/json", SINGLE_TEMPLATE_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var templates = new Templates(client);

			// Act
			var result = await templates.GetAsync(templateId, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Id.ShouldBe(templateId);
			result.Name.ShouldBe("example template name");
		}

		[Fact]
		public async Task GetAllAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", MULTIPLE_TEMPLATES_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var templates = new Templates(client);

			// Act
			var result = await templates.GetAllAsync(CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
		}

		[Fact]
		public async Task DeleteAsync()
		{
			// Arrange
			var templateId = "xxxxxxxx";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, templateId)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var templates = new Templates(client);

			// Act
			await templates.DeleteAsync(templateId, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task UpdateAsync()
		{
			// Arrange
			var templateId = "733ba07f-ead1-41fc-933a-3976baa23716";
			var name = "new_example_name";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT, templateId)).Respond("application/json", SINGLE_TEMPLATE_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var templates = new Templates(client);

			// Act
			var result = await templates.UpdateAsync(templateId, name, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task CreateVersionAsync()
		{
			// Arrange
			var templateId = "ddb96bbc-9b92-425e-8979-99464621b543";
			var name = "example_version_name";
			var subject = "<%subject%>";
			var htmlContent = "<%body%>";
			var textContent = "<%body%>";
			var isActive = true;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, templateId, "versions")).Respond("application/json", SINGLE_TEMPLATE_VERSION_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var templates = new Templates(client);

			// Act
			var result = await templates.CreateVersionAsync(templateId, name, subject, htmlContent, textContent, isActive, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task ActivateVersionAsync()
		{
			// Arrange
			var templateId = "e3a61852-1acb-4b32-a1bc-b44b3814ab78";
			var versionId = "8aefe0ee-f12b-4575-b5b7-c97e21cb36f3";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, templateId, "versions", versionId, "activate")).Respond("application/json", SINGLE_TEMPLATE_VERSION_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var templates = new Templates(client);

			// Act
			var result = await templates.ActivateVersionAsync(templateId, versionId, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetVersionAsync()
		{
			// Arrange
			var templateId = "d51480ca-ca3f-465c-bc3e-ceb71d73c38d";
			var versionId = "5997fcf6-2b9f-484d-acd5-7e9a99f0dc1f";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, templateId, "versions", versionId)).Respond("application/json", SINGLE_TEMPLATE_VERSION_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var templates = new Templates(client);

			// Act
			var result = await templates.GetVersionAsync(templateId, versionId, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task UpdateVersionAsync()
		{
			// Arrange
			var templateId = "ddb96bbc-9b92-425e-8979-99464621b543";
			var versionId = "8aefe0ee-f12b-4575-b5b7-c97e21cb36f3";
			var name = "updated_example_name";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT, templateId, "versions", versionId)).Respond("application/json", SINGLE_TEMPLATE_VERSION_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var templates = new Templates(client);

			// Act
			var result = await templates.UpdateVersionAsync(templateId, versionId, name, null, null, null, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task DeleteVersionAsync()
		{
			// Arrange
			var templateId = "ddb96bbc-9b92-425e-8979-99464621b543";
			var versionId = "8aefe0ee-f12b-4575-b5b7-c97e21cb36f3";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, templateId, "versions", versionId)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var templates = new Templates(client);

			// Act
			await templates.DeleteVersionAsync(templateId, versionId, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}
	}
}
