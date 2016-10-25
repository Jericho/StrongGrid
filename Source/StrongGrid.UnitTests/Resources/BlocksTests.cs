using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class BlocksTests
	{
		private const string ENDPOINT = "/suppression/blocks";

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			var apiResponse = @"[
				{
				'created': 1443651154,
				'email': 'user1@example.com',
				'reason': 'error dialing remote address: dial tcp 10.57.152.165:25: no route to host',
				'status': '4.0.0'
				}
			]";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync($"{ENDPOINT}?start_time=&end_time=&limit=25&offset=0", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var blocks = new Blocks(mockClient.Object);

			// Act
			var result = blocks.GetAllAsync().Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("user1@example.com", result[0].Email);
		}

		[TestMethod]
		public void DeleteAll()
		{
			// Arrange
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var blocks = new Blocks(mockClient.Object);

			// Act
			blocks.DeleteAllAsync().Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void DeleteMultiple()
		{
			// Arrange
			var emailAddresses = new[] { "email1@test.com", "email2@test.com" };

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync(ENDPOINT, It.Is<JObject>(o => o["emails"].ToObject<string[]>().Length == emailAddresses.Length), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var blocks = new Blocks(mockClient.Object);

			// Act
			blocks.DeleteMultipleAsync(emailAddresses).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			var emailAddress = "spam1@test.com";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync($"{ENDPOINT}/{emailAddress}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var blocks = new Blocks(mockClient.Object);

			// Act
			blocks.DeleteAsync(emailAddress).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var emailAddress = "user1@example.com";

			var apiResponse = @"{
				'created': 1443651154,
				'email': 'user1@example.com',
				'reason': 'error dialing remote address: dial tcp 10.57.152.165:25: no route to host',
				'status': '4.0.0'
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync($"{ENDPOINT}/{emailAddress}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var blocks = new Blocks(mockClient.Object);

			// Act
			var result = blocks.GetAsync(emailAddress, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(emailAddress, result.Email);
		}
	}
}
