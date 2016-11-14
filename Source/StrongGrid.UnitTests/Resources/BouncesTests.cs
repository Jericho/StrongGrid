using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class BouncesTests
	{
		#region FIELDS

		private const string ENDPOINT = "/suppression/bounces";
		private MockRepository _mockRepository;
		private Mock<IClient> _mockClient;

		private const string SINGLE_BOUNCE_JSON = @"{
			'created': 1443651125,
			'email': 'testemail1@test.com',
			'reason': '550 5.1.1 The email account that you tried to reach does not exist. Please try double-checking the recipient\'s email address for typos or unnecessary spaces. Learn more at  https://support.google.com/mail/answer/6596 o186si2389584ioe.63 - gsmtp ',
			'status': '5.1.1'
		}";
		private const string MULTIPLE_BOUNCES_JSON = @"[
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

		#endregion

		private Bounces CreateBounces()
		{
			return new Bounces(_mockClient.Object, ENDPOINT);

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
			var result = JsonConvert.DeserializeObject<Bounce>(SINGLE_BOUNCE_JSON);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(new System.DateTime(2015, 9, 30, 22, 12, 5, 0, System.DateTimeKind.Utc), result.CreatedOn);
			Assert.AreEqual("testemail1@test.com", result.EmailAddress);
			Assert.AreEqual("550 5.1.1 The email account that you tried to reach does not exist. Please try double-checking the recipient\'s email address for typos or unnecessary spaces. Learn more at  https://support.google.com/mail/answer/6596 o186si2389584ioe.63 - gsmtp ", result.Reason);
			Assert.AreEqual("5.1.1", result.Status);
		}

		[TestMethod]
		public void Get_between_startdate_and_enddate()
		{
			// Arrange
			var start = new DateTime(2015, 6, 8, 0, 0, 0, DateTimeKind.Utc);
			var end = new DateTime(2015, 9, 30, 23, 59, 59, DateTimeKind.Utc);

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}?start_time={start.ToUnixTime()}&end_time={end.ToUnixTime()}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_BOUNCES_JSON) })
				.Verifiable();

			var bounces = CreateBounces();

			// Act
			var result = bounces.GetAsync(start, end, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
		}

		[TestMethod]
		public void Get_by_email()
		{
			// Arrange
			var email = "bounce1@test.com";

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{email}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_BOUNCES_JSON) }).
				Verifiable();

			var bounces = CreateBounces();

			// Act
			var result = bounces.GetAsync(email, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
		}

		[TestMethod]
		public void DeleteAll()
		{
			// Arrange
			_mockClient
				.Setup(c => c.DeleteAsync(ENDPOINT, It.Is<JObject>(o => o["delete_all"].ToObject<bool>()), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var bounces = CreateBounces();

			// Act
			bounces.DeleteAllAsync(CancellationToken.None).Wait();

			// Assert
		}

		[TestMethod]
		public void Delete_by_single_email()
		{
			// Arrange
			var email = "email1@test.com";

			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{email}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var bounces = CreateBounces();

			// Act
			bounces.DeleteAsync(email, CancellationToken.None).Wait();

			// Assert
		}

		[TestMethod]
		public void Delete_by_multiple_emails()
		{
			// Arrange
			var emails = new[] { "email1@test.com", "email2@test.com" };

			_mockClient
				.Setup(c => c.DeleteAsync(ENDPOINT, It.Is<JObject>(o => o["emails"].ToObject<JArray>().Count == emails.Length), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var bounces = CreateBounces();

			// Act
			bounces.DeleteAsync(emails, CancellationToken.None).Wait();

			// Assert
		}
	}
}
