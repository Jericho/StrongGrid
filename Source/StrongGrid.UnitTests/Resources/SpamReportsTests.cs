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
	public class SpamReportsTests
	{
		#region FIELDS

		private const string ENDPOINT = "/suppression/spam_reports";
		private MockRepository _mockRepository;
		private Mock<IClient> _mockClient;

		private const string SINGLE_SPAM_REPORT_JSON = @"[
				{
					'created': 1454433146,
					'email': 'test1@example.com',
					'ip': '10.89.32.5'
				}
		]";
		private const string MULTIPLE_SPAM_REPORTS_JSON = @"[
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

		#endregion

		private SpamReports CreateSpamReports()
		{
			return new SpamReports(_mockClient.Object, ENDPOINT);

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
			var result = JsonConvert.DeserializeObject<SpamReport[]>(SINGLE_SPAM_REPORT_JSON);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual(new DateTime(2016, 2, 2, 17, 12, 26, DateTimeKind.Utc), result[0].CreatedOn);
			Assert.AreEqual("test1@example.com", result[0].Email);
			Assert.AreEqual("10.89.32.5", result[0].IpAddress);
		}

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}?start_time=&end_time=&limit=25&offset=0", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_SPAM_REPORTS_JSON) })
				.Verifiable();

			var spamReports = CreateSpamReports();

			// Act
			var result = spamReports.GetAllAsync().Result;

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

			var spamReports = CreateSpamReports();

			// Act
			spamReports.DeleteAllAsync().Wait(CancellationToken.None);

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

			var spamReports = CreateSpamReports();

			// Act
			spamReports.DeleteMultipleAsync(emailAddresses).Wait(CancellationToken.None);

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

			var spamReports = CreateSpamReports();

			// Act
			spamReports.DeleteAsync(emailAddress).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var emailAddress = "test1@example.com";

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{emailAddress}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_SPAM_REPORT_JSON) })
				.Verifiable();

			var spamReports = CreateSpamReports();

			// Act
			var result = spamReports.GetAsync(emailAddress, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
		}
	}
}
