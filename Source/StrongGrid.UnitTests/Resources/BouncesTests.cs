using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Resources;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class BouncesTests
	{
		#region FIELDS

		internal const string ENDPOINT = "suppression/bounces";

		internal const string SINGLE_BOUNCE_JSON = @"{
			""created"": 1443651125,
			""email"": ""testemail1@test.com"",
			""reason"": ""550 5.1.1 The email account that you tried to reach does not exist. Please try double-checking the recipient's email address for typos or unnecessary spaces. Learn more at  https://support.google.com/mail/answer/6596 o186si2389584ioe.63 - gsmtp "",
			""status"": ""5.1.1""
		}";
		internal const string MULTIPLE_BOUNCES_JSON = @"[
			{
				""created"": 1443651125,
				""email"": ""testemail1@test.com"",
				""reason"": ""550 5.1.1 The email account that you tried to reach does not exist. Please try double-checking the recipient's email address for typos or unnecessary spaces. Learn more at  https://support.google.com/mail/answer/6596 o186si2389584ioe.63 - gsmtp "",
				""status"": ""5.1.1""
			},
			{
				""created"": 1433800303,
				""email"": ""testemail2@testing.com"",
				""reason"": ""550 5.1.1 <testemail2@testing.com>: Recipient address rejected: User unknown in virtual alias table "",
				""status"": ""5.1.1""
			}
		]";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<Bounce>(SINGLE_BOUNCE_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.CreatedOn.ShouldBe(new System.DateTime(2015, 9, 30, 22, 12, 5, 0, System.DateTimeKind.Utc));
			result.EmailAddress.ShouldBe("testemail1@test.com");
			result.Reason.ShouldBe("550 5.1.1 The email account that you tried to reach does not exist. Please try double-checking the recipient\'s email address for typos or unnecessary spaces. Learn more at  https://support.google.com/mail/answer/6596 o186si2389584ioe.63 - gsmtp ");
			result.Status.ShouldBe("5.1.1");
		}

		[Fact]
		public async Task GetAllAsync_between_startdate_and_enddate()
		{
			// Arrange
			var start = new DateTime(2015, 6, 8, 0, 0, 0, DateTimeKind.Utc);
			var end = new DateTime(2015, 9, 30, 23, 59, 59, DateTimeKind.Utc);

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT) + $"?start_time={start.ToUnixTime()}&end_time={end.ToUnixTime()}").Respond("application/json", MULTIPLE_BOUNCES_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var bounces = new Bounces(client);

			// Act
			var result = await bounces.GetAllAsync(start, end, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public async Task GetAsync_by_email()
		{
			// Arrange
			var email = "bounce1@test.com";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, email)).Respond("application/json", MULTIPLE_BOUNCES_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var bounces = new Bounces(client);

			// Act
			var result = await bounces.GetAsync(email, null, CancellationToken.None).ConfigureAwait(false);

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
			var bounces = new Bounces(client);

			// Act
			await bounces.DeleteAllAsync(null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task DeleteAsync_by_single_email()
		{
			// Arrange
			var email = "email1@test.com";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, email)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var bounces = new Bounces(client);

			// Act
			await bounces.DeleteAsync(email, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task DeleteAsync_by_multiple_emails()
		{
			// Arrange
			var emails = new[] { "email1@test.com", "email2@test.com" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var bounces = new Bounces(client);

			// Act
			await bounces.DeleteAsync(emails, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}
	}
}
