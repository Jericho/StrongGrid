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
		[TestMethod]
		public void Create()
		{
			// Arrange
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
			mockClient.Setup(c => c.PostAsync("/api_keys", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var apiKeys = new ApiKeys(mockClient.Object);
			var name = "My API Key";
			var scopes = new[] { "mail.send", "alerts.create", "alerts.read" };

			// Act
			var result = apiKeys.CreateAsync(name, scopes, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}
	}
}
