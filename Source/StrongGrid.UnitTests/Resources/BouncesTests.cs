using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using StrongGrid.Utilities;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class BouncesTests
	{
		private const string ENDPOINT = "/suppression/bounces";

		[TestMethod]
		public void Get_between_startdate_and_enddate()
		{
			// Arrange
			var start = new DateTime(2015, 6, 8, 0, 0, 0, DateTimeKind.Utc);
			var end = new DateTime(2015, 9, 30, 23, 59, 59, DateTimeKind.Utc);

			var apiResponse = @"[
				{
					'created': 1443651125,
					'email': 'testemail1@test.com',
					'reason': '550 5.1.1 The email account that you tried to reach does not exist. Please try double-checking the recipient\'s email address for typos or unnecessary spaces. Learn more at  https://support.google.com/mail/answer/6596 o186si2389584ioe.63 - gsmtp ',
					'status': '5.1.1'
				},
				{
					'created': 1433800303,
					'email': 'testemail2@testing.com',
					'reason': '550 5.1.1 <testemail2@testing.com>: Recipient address rejected: User unknown in virtual alias table ',
					'status': '5.1.1'
				}
			]";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(ENDPOINT + "?start_time=" + start.ToUnixTime() + "&end_time=" + end.ToUnixTime(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var bounces = new Bounces(mockClient.Object);

			// Act
			var result = bounces.GetAsync(start, end, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual("testemail1@test.com", result[0].EmailAddress);
			Assert.AreEqual("5.1.1", result[0].Status);
		}

		[TestMethod]
		public void Get_by_email()
		{
			// Arrange
			var email = "bounce1@test.com";

			var apiResponse = @"[
				{
					'created': 1443651125,
					'email': 'bounce1@test.com',
					'reason': '550 5.1.1 The email account that you tried to reach does not exist. Please try double-checking the recipient\'s email address for typos or unnecessary spaces. Learn more at  https://support.google.com/mail/answer/6596 o186si2389584ioe.63 - gsmtp ',
					'status': '5.1.1'
				}
			]";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(ENDPOINT + "/" + email, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var bounces = new Bounces(mockClient.Object);

			// Act
			var result = bounces.GetAsync(email, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("bounce1@test.com", result[0].EmailAddress);
			Assert.AreEqual("5.1.1", result[0].Status);
		}

		[TestMethod]
		public void DeleteAll()
		{
			// Arrange
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync(ENDPOINT, It.Is<JObject>(o => o["delete_all"].ToObject<bool>()), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var bounces = new Bounces(mockClient.Object);

			// Act
			bounces.DeleteAllAsync(CancellationToken.None).Wait();

			// Assert
		}

		[TestMethod]
		public void Delete_by_single_email()
		{
			// Arrange
			var email = "email1@test.com";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync(ENDPOINT + "/" + email, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var bounces = new Bounces(mockClient.Object);

			// Act
			bounces.DeleteAsync(email, CancellationToken.None).Wait();

			// Assert
		}

		[TestMethod]
		public void Delete_by_multiple_emails()
		{
			// Arrange
			var emails = new[] { "email1@test.com", "email2@test.com" };

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync(ENDPOINT, It.Is<JObject>(o => o["emails"].ToObject<JArray>().Count == emails.Length), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var bounces = new Bounces(mockClient.Object);

			// Act
			bounces.DeleteAsync(emails, CancellationToken.None).Wait();

			// Assert
		}
	}
}
