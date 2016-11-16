using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using StrongGrid.Model;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class ApiKeysTests
	{
		#region FIELDS

		private const string ENDPOINT = "/api_keys";

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

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<ApiKey>(SINGLE_API_KEY_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.Key.ShouldBe("SG.xxxxxxxx.yyyyyyyy");
			result.KeyId.ShouldBe("xxxxxxxx");
			result.Name.ShouldBe("My API Key");
			result.Scopes.ShouldBe(new[] { "mail.send", "alerts.create", "alerts.read" });
		}

		[Fact]
		public void Create()
		{
			// Arrange
			var name = "My API Key";
			var scopes = new[] { "mail.send", "alerts.create", "alerts.read" };

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_API_KEY_JSON) })
				.Verifiable();

			var apiKeys = new ApiKeys(mockClient.Object, ENDPOINT);

			// Act
			var result = apiKeys.CreateAsync(name, scopes, CancellationToken.None).Result;

			// Assert
			mockRepository.VerifyAll();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void Get()
		{
			// Arrange
			var keyId = "xxxxxxxx";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{keyId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_API_KEY_JSON) })
				.Verifiable();

			var apiKeys = new ApiKeys(mockClient.Object, ENDPOINT);

			// Act
			var result = apiKeys.GetAsync(keyId, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetAll()
		{
			// Arrange
			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_API_KEY_JSON) })
				.Verifiable();

			var apiKeys = new ApiKeys(mockClient.Object, ENDPOINT);

			// Act
			var result = apiKeys.GetAllAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public void Delete()
		{
			// Arrange
			var keyId = "xxxxxxxx";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{keyId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
				.Verifiable();

			var apiKeys = new ApiKeys(mockClient.Object, ENDPOINT);

			// Act
			apiKeys.DeleteAsync(keyId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void Update_with_scopes()
		{
			// Arrange
			var keyId = "xxxxxxxx";
			var name = "My API Key";
			var scopes = new[] { "mail.send", "alerts.create", "alerts.read" };

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PutAsync($"{ENDPOINT}/{keyId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_API_KEY_JSON) })
				.Verifiable();

			var apiKeys = new ApiKeys(mockClient.Object, ENDPOINT);

			// Act
			var result = apiKeys.UpdateAsync(keyId, name, scopes, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void Update_without_scopes()
		{
			// Arrange
			var keyId = "xxxxxxxx";
			var name = "My API Key";
			var scopes = (string[])null;

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync($"{ENDPOINT}/{keyId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_API_KEY_JSON) })
				.Verifiable();

			var apiKeys = new ApiKeys(mockClient.Object, ENDPOINT);

			// Act
			var result = apiKeys.UpdateAsync(keyId, name, scopes, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}
	}
}
