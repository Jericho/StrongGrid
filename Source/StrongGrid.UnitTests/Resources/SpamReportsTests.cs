using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using StrongGrid.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class SpamReportsTests
	{
		#region FIELDS

		private const string ENDPOINT = "/suppression/spam_reports";

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

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<SpamReport[]>(SINGLE_SPAM_REPORT_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].CreatedOn.ShouldBe(new DateTime(2016, 2, 2, 17, 12, 26, DateTimeKind.Utc));
			result[0].Email.ShouldBe("test1@example.com");
			result[0].IpAddress.ShouldBe("10.89.32.5");
		}

		[Fact]
		public void GetAll()
		{
			// Arrange
			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}?start_time=&end_time=&limit=25&offset=0", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_SPAM_REPORTS_JSON) })
				.Verifiable();

			var spamReports = new SpamReports(mockClient.Object, ENDPOINT);

			// Act
			var result = spamReports.GetAllAsync().Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public void DeleteAll()
		{
			// Arrange
			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var spamReports = new SpamReports(mockClient.Object, ENDPOINT);

			// Act
			spamReports.DeleteAllAsync().Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void DeleteMultiple()
		{
			// Arrange
			var emailAddresses = new[] { "email1@test.com", "email2@test.com" };

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync(ENDPOINT, It.Is<JObject>(o => o["emails"].ToObject<string[]>().Length == emailAddresses.Length), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var spamReports = new SpamReports(mockClient.Object, ENDPOINT);

			// Act
			spamReports.DeleteMultipleAsync(emailAddresses).Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void Delete()
		{
			// Arrange
			var emailAddress = "spam1@test.com";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{emailAddress}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var spamReports = new SpamReports(mockClient.Object, ENDPOINT);

			// Act
			spamReports.DeleteAsync(emailAddress).Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void Get()
		{
			// Arrange
			var emailAddress = "test1@example.com";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{emailAddress}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_SPAM_REPORT_JSON) })
				.Verifiable();

			var spamReports = new SpamReports(mockClient.Object, ENDPOINT);

			// Act
			var result = spamReports.GetAsync(emailAddress, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
		}
	}
}
