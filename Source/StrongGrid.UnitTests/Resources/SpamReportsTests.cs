using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Model;
using StrongGrid.UnitTests;
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
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, $"{ENDPOINT}?start_time=&end_time=&limit=25&offset=0").Respond("application/json", MULTIPLE_SPAM_REPORTS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var spamReports = new SpamReports(client, ENDPOINT);

			// Act
			var result = spamReports.GetAllAsync().Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public void DeleteAll()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, ENDPOINT).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var spamReports = new SpamReports(client, ENDPOINT);

			// Act
			spamReports.DeleteAllAsync().Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void DeleteMultiple()
		{
			// Arrange
			var emailAddresses = new[] { "email1@test.com", "email2@test.com" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, ENDPOINT).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var spamReports = new SpamReports(client, ENDPOINT);

			// Act
			spamReports.DeleteMultipleAsync(emailAddresses).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void Delete()
		{
			// Arrange
			var emailAddress = "spam1@test.com";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, $"{ENDPOINT}/{emailAddress}").Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var spamReports = new SpamReports(client, ENDPOINT);

			// Act
			spamReports.DeleteAsync(emailAddress).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void Get()
		{
			// Arrange
			var emailAddress = "test1@example.com";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, $"{ENDPOINT}/{emailAddress}").Respond("application/json", SINGLE_SPAM_REPORT_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var spamReports = new SpamReports(client, ENDPOINT);

			// Act
			var result = spamReports.GetAsync(emailAddress, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
		}
	}
}
