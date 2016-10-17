using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class ApiKeysTests
	{
		private const string ENDPOINT = "/api_keys";

		[TestMethod]
		public void Create()
		{
			// Arrange
			var name = "My API Key";
			var scopes = new[] { "mail.send", "alerts.create", "alerts.read" };

			var apiResponse = @"{
				'api_key': 'SG.xxxxxxxx.yyyyyyyy',
				'api_key_id': 'xxxxxxxx',
				'name': 'My API Key',
				'scopes': [
					'mail.send',
					'alerts.create',
					'alerts.read'
				]
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var apiKeys = new ApiKeys(mockClient.Object);

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
			var apiResponse = @"{
				'api_key_id': 'xxxxxxxx',
				'name': 'My API Key',
				'scopes': [
					'mail.send',
					'alerts.create',
					'alerts.read'
				]
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(ENDPOINT + "/" + keyId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var apiKeys = new ApiKeys(mockClient.Object);

			// Act
			var result = apiKeys.GetAsync(keyId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(keyId, result.KeyId);
		}

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			var apiResponse = @"{
				'result': [
					{
						'name': 'A New Hope',
						'api_key_id': 'xxxxxxxx'
					}
				]
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var apiKeys = new ApiKeys(mockClient.Object);

			// Act
			var result = apiKeys.GetAllAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("xxxxxxxx", result[0].KeyId);
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			var keyId = "xxxxxxxx";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync(ENDPOINT + "/" + keyId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var apiKeys = new ApiKeys(mockClient.Object);

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

			var apiResponse = @"{
				'api_key': 'SG.xxxxxxxx.yyyyyyyy',
				'api_key_id': 'xxxxxxxx',
				'name': 'My API Key',
				'scopes': [
					'mail.send',
					'alerts.create',
					'alerts.read'
				]
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PutAsync(ENDPOINT + "/" + keyId, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var apiKeys = new ApiKeys(mockClient.Object);

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

			var apiResponse = @"{
				'api_key': 'SG.xxxxxxxx.yyyyyyyy',
				'api_key_id': 'xxxxxxxx',
				'name': 'My API Key',
				'scopes': [
					'mail.send',
					'alerts.create',
					'alerts.read'
				]
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PatchAsync(ENDPOINT + "/" + keyId, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var apiKeys = new ApiKeys(mockClient.Object);

			// Act
			var result = apiKeys.UpdateAsync(keyId, name, scopes, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}
	}
}
