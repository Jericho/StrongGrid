using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class SpamReportsTests
	{
		private const string ENDPOINT = "/suppression/spam_reports";

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			var apiResponse = @"[
				{
					'created': 1443651141,
					'email': 'user1@example.com',
					'ip': '10.63.202.100'
				},
				{
					'created': 1443651154,
					'email': 'user2@example.com',
					'ip': '10.63.202.100'
				}
			]";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync($"{ENDPOINT}?start_time=&end_time=&limit=25&offset=0", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var spamReports = new SpamReports(mockClient.Object);

			// Act
			var result = spamReports.GetAllAsync().Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual("user1@example.com", result[0].Email);
			Assert.AreEqual("10.63.202.100", result[1].IpAddress);
		}

		[TestMethod]
		public void DeleteAll()
		{
			// Arrange
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var spamReports = new SpamReports(mockClient.Object);

			// Act
			spamReports.DeleteAllAsync().Wait(CancellationToken.None);

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

			var spamReports = new SpamReports(mockClient.Object);

			// Act
			spamReports.DeleteMultipleAsync(emailAddresses).Wait(CancellationToken.None);

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

			var spamReports = new SpamReports(mockClient.Object);

			// Act
			spamReports.DeleteAsync(emailAddress).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var emailAddress = "test1@example.com";

			var apiResponse = @"[
				{
					'created': 1454433146,
					'email': 'test1@example.com',
					'ip': '10.89.32.5'
				}
			]";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync($"{ENDPOINT}/{emailAddress}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var spamReports = new SpamReports(mockClient.Object);

			// Act
			var result = spamReports.GetAsync(emailAddress, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual(emailAddress, result[0].Email);
			Assert.AreEqual("10.89.32.5", result[0].IpAddress);
		}
	}
}
