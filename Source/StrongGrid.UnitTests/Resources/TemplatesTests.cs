using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class TemplatesTests
	{
		private const string ENDPOINT = "/templates";

		[TestMethod]
		public void Create()
		{
			// Arrange
			var name = "My template";
			var scopes = new[] { "mail.send", "alerts.create", "alerts.read" };

			var apiResponse = @"{
				'id': '733ba07f-ead1-41fc-933a-3976baa23716',
				'name': 'My template',
				'versions': []
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var templates = new Templates(mockClient.Object);

			// Act
			var result = templates.CreateAsync(name, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(name, result.Name);
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var templateId = "e8ac01d5-a07a-4a71-b14c-4721136fe6aa";
			var apiResponse = @"{
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

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync($"{ENDPOINT}/{templateId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var templates = new Templates(mockClient.Object);

			// Act
			var result = templates.GetAsync(templateId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(templateId, result.Id);
			Assert.AreEqual("example template name", result.Name);
		}

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			var apiResponse = @"{
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

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var templates = new Templates(mockClient.Object);

			// Act
			var result = templates.GetAllAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("example template name", result[0].Name);
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			var templateId = "xxxxxxxx";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync($"{ENDPOINT}/{templateId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var templates = new Templates(mockClient.Object);

			// Act
			templates.DeleteAsync(templateId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Update()
		{
			// Arrange
			var templateId = "733ba07f-ead1-41fc-933a-3976baa23716";
			var name = "new_example_name";

			var apiResponse = @"{
				'id': '733ba07f-ead1-41fc-933a-3976baa23716',
				'name': 'new_example_name',
				'versions': []
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PatchAsync($"{ENDPOINT}/{templateId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var templates = new Templates(mockClient.Object);

			// Act
			var result = templates.UpdateAsync(templateId, name, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(templateId, result.Id);
			Assert.AreEqual(name, result.Name);
		}

		[TestMethod]
		public void CreateVersion()
		{
			// Arrange
			var templateId = "ddb96bbc-9b92-425e-8979-99464621b543";
			var name = "example_version_name";
			var subject = "<%subject%>";
			var htmlContent = "<%body%>";
			var textContent = "<%body%>";
			var isActive = true;

			var apiResponse = @"{
				'id': '8aefe0ee-f12b-4575-b5b7-c97e21cb36f3',
				'template_id': 'ddb96bbc-9b92-425e-8979-99464621b543',
				'active': 1,
				'name': 'example_version_name',
				'html_content': '<%body%>',
				'plain_content': '<%body%>',
				'subject': '<%subject%>',
				'updated_at': '2014-03-19 18:56:33'
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync($"{ENDPOINT}/{templateId}/versions", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var templates = new Templates(mockClient.Object);

			// Act
			var result = templates.CreateVersionAsync(templateId, name, subject, htmlContent, textContent, isActive, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(templateId, result.TemplateId);
			Assert.AreEqual(name, result.Name);
			Assert.IsTrue(result.IsActive);
		}

		[TestMethod]
		public void ActivateVersion()
		{
			// Arrange
			var templateId = "e3a61852-1acb-4b32-a1bc-b44b3814ab78";
			var versionId = "8aefe0ee-f12b-4575-b5b7-c97e21cb36f3";

			var apiResponse = @"{
				'id': '8aefe0ee-f12b-4575-b5b7-c97e21cb36f3',
				'template_id': 'e3a61852-1acb-4b32-a1bc-b44b3814ab78',
				'active': 1,
				'name': 'example_version_name',
				'html_content': '<%body%>',
				'plain_content': '<%body%>',
				'subject': '<%subject%>',
				'updated_at': '2014-06-12 11:33:00'
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync($"{ENDPOINT}/{templateId}/versions/{versionId}/activate", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var templates = new Templates(mockClient.Object);

			// Act
			var result = templates.ActivateVersionAsync(templateId, versionId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(templateId, result.TemplateId);
			Assert.AreEqual(versionId, result.Id);
			Assert.IsTrue(result.IsActive);
		}

		[TestMethod]
		public void GetVersion()
		{
			// Arrange
			var templateId = "d51480ca-ca3f-465c-bc3e-ceb71d73c38d";
			var versionId = "5997fcf6-2b9f-484d-acd5-7e9a99f0dc1f";

			var apiResponse = @"{
				'id': '5997fcf6-2b9f-484d-acd5-7e9a99f0dc1f',
				'template_id': 'd51480ca-ca3f-465c-bc3e-ceb71d73c38d',
				'active': 1,
				'name': 'version 1 name',
				'html_content': '<%body%>',
				'plain_content': '<%body%>',
				'subject': '<%subject%>',
				'updated_at': '2014-03-19 18:56:33'
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync($"{ENDPOINT}/{templateId}/versions/{versionId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var templates = new Templates(mockClient.Object);

			// Act
			var result = templates.GetVersionAsync(templateId, versionId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(templateId, result.TemplateId);
			Assert.AreEqual(versionId, result.Id);
			Assert.IsTrue(result.IsActive);
		}

		[TestMethod]
		public void UpdateVersion()
		{
			// Arrange
			var templateId = "ddb96bbc-9b92-425e-8979-99464621b543";
			var versionId = "8aefe0ee-f12b-4575-b5b7-c97e21cb36f3";
			var name = "updated_example_name";

			var apiResponse = @"{
				'id': '8aefe0ee-f12b-4575-b5b7-c97e21cb36f3',
				'template_id': 'ddb96bbc-9b92-425e-8979-99464621b543',
				'active': 1,
				'name': 'updated_example_name',
				'html_content': '<%body%>',
				'plain_content': '<%body%>',
				'subject': '<%subject%>',
				'updated_at': '2014-03-19 18:56:33'
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PatchAsync($"{ENDPOINT}/{templateId}/versions/{versionId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var templates = new Templates(mockClient.Object);

			// Act
			var result = templates.UpdateVersionAsync(templateId, versionId, name, null, null, null, null, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(templateId, result.TemplateId);
			Assert.AreEqual(versionId, result.Id);
			Assert.AreEqual(name, result.Name);
			Assert.IsTrue(result.IsActive);
		}

		[TestMethod]
		public void DeleteVersion()
		{
			// Arrange
			var templateId = "ddb96bbc-9b92-425e-8979-99464621b543";
			var versionId = "8aefe0ee-f12b-4575-b5b7-c97e21cb36f3";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync($"{ENDPOINT}/{templateId}/versions/{versionId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var templates = new Templates(mockClient.Object);

			// Act
			templates.DeleteVersionAsync(templateId, versionId, CancellationToken.None).Wait();

			// Assert
		}
	}
}
