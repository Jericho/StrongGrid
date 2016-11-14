using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class ApiKeysTests
	{
		#region FIELDS

		private const string ENDPOINT = "/api_keys";
		private MockRepository _mockRepository;
		private Mock<IClient> _mockClient;

		private const string SINGLE_API_KEY_JSON = @"{
			'api_key': 'SG.xxxxxxxx.yyyyyyyy',
			'api_key_id': 'xxxxxxxx',
			'name': 'My API Key',
			'scopes': [
				'mail.send',
				'alerts.create',
				'alerts.read'
			]
		}";
		private const string MULTIPLE_API_KEY_JSON = @"{
			'result': [
				{
					'name': 'A New Hope',
					'api_key_id': 'xxxxxxxx'
				},
				{
					'name': 'Another key',
					'api_key_id': 'yyyyyyyy'
				}
			]
		}";

		#endregion

		private ApiKeys CreateApiKeys()
		{
			return new ApiKeys(_mockClient.Object, ENDPOINT);

		}

		[TestInitialize]
		public void TestInitialize()
		{
			_mockRepository = new MockRepository(MockBehavior.Strict);
			_mockClient = _mockRepository.Create<IClient>();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			_mockRepository.VerifyAll();
		}

		[TestMethod]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<ApiKey>(SINGLE_API_KEY_JSON);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual("SG.xxxxxxxx.yyyyyyyy", result.Key);
			Assert.AreEqual("xxxxxxxx", result.KeyId);
			Assert.AreEqual("My API Key", result.Name);
			CollectionAssert.AreEqual(new[] { "mail.send", "alerts.create", "alerts.read" }, result.Scopes);
		}

		[TestMethod]
		public void Create()
		{
			// Arrange
			var name = "My API Key";
			var scopes = new[] { "mail.send", "alerts.create", "alerts.read" };

			_mockClient
				.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_API_KEY_JSON) })
				.Verifiable();

			var apiKeys = CreateApiKeys();

			// Act
			var result = apiKeys.CreateAsync(name, scopes, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var keyId = "xxxxxxxx";

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{keyId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_API_KEY_JSON) })
				.Verifiable();

			var apiKeys = CreateApiKeys();

			// Act
			var result = apiKeys.GetAsync(keyId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			_mockClient
				.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_API_KEY_JSON) })
				.Verifiable();

			var apiKeys = CreateApiKeys();

			// Act
			var result = apiKeys.GetAllAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			var keyId = "xxxxxxxx";

			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{keyId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
				.Verifiable();

			var apiKeys = CreateApiKeys();

			// Act
			apiKeys.DeleteAsync(keyId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Update_with_scopes()
		{
			// Arrange
			var keyId = "xxxxxxxx";
			var name = "My API Key";
			var scopes = new[] { "mail.send", "alerts.create", "alerts.read" };

			_mockClient
				.Setup(c => c.PutAsync($"{ENDPOINT}/{keyId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_API_KEY_JSON) })
				.Verifiable();

			var apiKeys = CreateApiKeys();

			// Act
			var result = apiKeys.UpdateAsync(keyId, name, scopes, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Update_without_scopes()
		{
			// Arrange
			var keyId = "xxxxxxxx";
			var name = "My API Key";
			var scopes = (string[])null;

			_mockClient
				.Setup(c => c.PatchAsync($"{ENDPOINT}/{keyId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_API_KEY_JSON) })
				.Verifiable();

			var apiKeys = CreateApiKeys();

			// Act
			var result = apiKeys.UpdateAsync(keyId, name, scopes, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}
	}
}
