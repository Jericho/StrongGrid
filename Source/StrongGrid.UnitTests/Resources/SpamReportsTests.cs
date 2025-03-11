using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Resources;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class SpamReportsTests
	{
		#region FIELDS

		internal const string ENDPOINT = "suppression/spam_reports";

		internal const string SINGLE_SPAM_REPORT_JSON = @"[
				{
					""created"": 1454433146,
					""email"": ""test1@example.com"",
					""ip"": ""10.89.32.5""
				}
		]";
		internal const string MULTIPLE_SPAM_REPORTS_JSON = @"[
			{
				""created"": 1443651141,
				""email"": ""user1@example.com"",
				""ip"": ""10.63.202.100""
			},
			{
				""created"": 1443651154,
				""email"": ""user2@example.com"",
				""ip"": ""10.63.202.100""
			}
		]";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<SpamReport[]>(SINGLE_SPAM_REPORT_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].CreatedOn.ShouldBe(new DateTime(2016, 2, 2, 17, 12, 26, DateTimeKind.Utc));
			result[0].Email.ShouldBe("test1@example.com");
			result[0].IpAddress.ShouldBe("10.89.32.5");
		}

		[Fact]
		public async Task GetAllAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT) + $"?limit=25&offset=0").Respond("application/json", MULTIPLE_SPAM_REPORTS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var spamReports = new SpamReports(client);

			// Act
			var result = await spamReports.GetAllAsync(cancellationToken: TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public async Task DeleteAllAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var spamReports = new SpamReports(client);

			// Act
			await spamReports.DeleteAllAsync(cancellationToken: TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task DeleteMultipleAsync()
		{
			// Arrange
			var emailAddresses = new[] { "email1@test.com", "email2@test.com" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var spamReports = new SpamReports(client);

			// Act
			await spamReports.DeleteMultipleAsync(emailAddresses, cancellationToken: TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task DeleteAsync()
		{
			// Arrange
			var emailAddress = "spam1@test.com";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, emailAddress)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var spamReports = new SpamReports(client);

			// Act
			await spamReports.DeleteAsync(emailAddress, cancellationToken: TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task GetAsync()
		{
			// Arrange
			var emailAddress = "test1@example.com";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, emailAddress)).Respond("application/json", SINGLE_SPAM_REPORT_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var spamReports = new SpamReports(client);

			// Act
			var result = await spamReports.GetAsync(emailAddress, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
		}
	}
}
