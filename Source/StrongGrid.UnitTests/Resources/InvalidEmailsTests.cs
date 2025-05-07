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
	public class InvalidEmailsTests
	{
		internal const string ENDPOINT = "suppression/invalid_emails";

		internal const string SINGLE_INVALID_EMAIL_JSON = @"{
			""created"": 1454433146,
			""email"": ""test1@example.com"",
			""reason"": ""Mail domain mentioned in email address is unknown""
		}";
		internal const string MULTIPLE_INVALID_EMAILS_JSON = @"[
			{
				""created"": 1449953655,
				""email"": ""user1@example.com"",
				""reason"": ""Mail domain mentioned in email address is unknown""
			},
			{
				""created"": 1449939373,
				""email"": ""user1@example.com"",
				""reason"": ""Mail domain mentioned in email address is unknown""
			}
		]";

		private readonly ITestOutputHelper _outputHelper;

		public InvalidEmailsTests(ITestOutputHelper outputHelper)
		{
			_outputHelper = outputHelper;
		}

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<InvalidEmail>(SINGLE_INVALID_EMAIL_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.CreatedOn.ShouldBe(new DateTime(2016, 2, 2, 17, 12, 26, DateTimeKind.Utc));
			result.Email.ShouldBe("test1@example.com");
			result.Reason.ShouldBe("Mail domain mentioned in email address is unknown");
		}

		[Fact]
		public async Task GetAllAsync()
		{
			// Arrange
			var limit = 25;
			var offset = 0;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT) + $"?limit={limit}&offset={offset}").Respond("application/json", MULTIPLE_INVALID_EMAILS_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var invalidEmails = new InvalidEmails(client);

			// Act
			var result = await invalidEmails.GetAllAsync(null, null, limit, offset, null, TestContext.Current.CancellationToken);

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

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var invalidEmails = new InvalidEmails(client);

			// Act
			await invalidEmails.DeleteAllAsync(cancellationToken: TestContext.Current.CancellationToken);

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

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var invalidEmails = new InvalidEmails(client);

			// Act
			await invalidEmails.DeleteMultipleAsync(emailAddresses, cancellationToken: TestContext.Current.CancellationToken);

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

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var invalidEmails = new InvalidEmails(client);

			// Act
			await invalidEmails.DeleteAsync(emailAddress, cancellationToken: TestContext.Current.CancellationToken);

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
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, emailAddress)).Respond("application/json", MULTIPLE_INVALID_EMAILS_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var invalidEmails = new InvalidEmails(client);

			// Act
			var result = await invalidEmails.GetAsync(emailAddress, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}
	}
}
