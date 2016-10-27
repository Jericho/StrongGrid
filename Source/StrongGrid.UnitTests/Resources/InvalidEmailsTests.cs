using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class InvalidEmailsTests
	{
		private const string ENDPOINT = "/suppression/invalid_emails";

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			var apiResponse = @"[
				{
					'created': 1449953655,
					'email': 'user1@example.com',
					'reason': 'Mail domain mentioned in email address is unknown'
				},
				{
					'created': 1449939373,
					'email': 'user1@example.com',
					'reason': 'Mail domain mentioned in email address is unknown'
				}
			]";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync($"{ENDPOINT}?start_time=&end_time=&limit=25&offset=0", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var invalidEmails = new InvalidEmails(mockClient.Object);

			// Act
			var result = invalidEmails.GetAllAsync().Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual("user1@example.com", result[0].Email);
		}

		[TestMethod]
		public void DeleteAll()
		{
			// Arrange
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var invalidEmails = new InvalidEmails(mockClient.Object);

			// Act
			invalidEmails.DeleteAllAsync().Wait(CancellationToken.None);

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

			var invalidEmails = new InvalidEmails(mockClient.Object);

			// Act
			invalidEmails.DeleteMultipleAsync(emailAddresses).Wait(CancellationToken.None);

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

			var invalidEmails = new InvalidEmails(mockClient.Object);

			// Act
			invalidEmails.DeleteAsync(emailAddress).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var emailAddress = "test1@example.com";

			var apiResponse = @"{
				'created': 1454433146,
				'email': 'test1@example.com',
				'reason': 'Mail domain mentioned in email address is unknown'
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync($"{ENDPOINT}/{emailAddress}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var invalidEmails = new InvalidEmails(mockClient.Object);

			// Act
			var result = invalidEmails.GetAsync(emailAddress, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(emailAddress, result.Email);
		}
	}
}
