using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class InvalidEmailsTests
	{
		#region FIELDS

		private const string ENDPOINT = "/suppression/invalid_emails";
		private MockRepository _mockRepository;
		private Mock<IClient> _mockClient;

		private const string SINGLE_INVALID_EMAIL_JSON = @"{
			'created': 1454433146,
			'email': 'test1@example.com',
			'reason': 'Mail domain mentioned in email address is unknown'
		}";
		private const string MULTIPLE_INVALID_EMAILS_JSON = @"[
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

		#endregion

		private InvalidEmails CreateInvalidEmails()
		{
			return new InvalidEmails(_mockClient.Object, ENDPOINT);

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
			var result = JsonConvert.DeserializeObject<InvalidEmail>(SINGLE_INVALID_EMAIL_JSON);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(new DateTime(2016, 2, 2, 17, 12, 26, DateTimeKind.Utc), result.CreatedOn);
			Assert.AreEqual("test1@example.com", result.Email);
			Assert.AreEqual("Mail domain mentioned in email address is unknown", result.Reason);
		}

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}?start_time=&end_time=&limit=25&offset=0", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_INVALID_EMAILS_JSON) })
				.Verifiable();

			var invalidEmails = CreateInvalidEmails();

			// Act
			var result = invalidEmails.GetAllAsync().Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
		}

		[TestMethod]
		public void DeleteAll()
		{
			// Arrange
			_mockClient
				.Setup(c => c.DeleteAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var invalidEmails = CreateInvalidEmails();

			// Act
			invalidEmails.DeleteAllAsync().Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void DeleteMultiple()
		{
			// Arrange
			var emailAddresses = new[] { "email1@test.com", "email2@test.com" };

			_mockClient
				.Setup(c => c.DeleteAsync(ENDPOINT, It.Is<JObject>(o => o["emails"].ToObject<string[]>().Length == emailAddresses.Length), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var invalidEmails = CreateInvalidEmails();

			// Act
			invalidEmails.DeleteMultipleAsync(emailAddresses).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			var emailAddress = "spam1@test.com";

			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{emailAddress}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var invalidEmails = CreateInvalidEmails();

			// Act
			invalidEmails.DeleteAsync(emailAddress).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var emailAddress = "test1@example.com";

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{emailAddress}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_INVALID_EMAIL_JSON) })
				.Verifiable();

			var invalidEmails = CreateInvalidEmails();

			// Act
			var result = invalidEmails.GetAsync(emailAddress, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}
	}
}
