using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Resources;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class GlobalSuppressionTests
	{
		#region FIELDS

		internal const string ENDPOINT = "asm/suppressions/global";
		internal const string GLOBALLY_UNSUBSCRIBED = @"[
			{
				""email"": ""example@bogus.com"",
				""created"": 1422313607
			},
			{
				""email"": ""bogus@example.com"",
				""created"": 1422313607
			},
			{
				""email"": ""invalid@somewhere.com"",
				""created"": 1422313607
			}
		]";

		#endregion

		[Fact]
		public async Task GetAll()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("suppression/unsubscribes")).Respond((HttpRequestMessage request) =>
			{
				var response = new HttpResponseMessage(HttpStatusCode.OK);
				response.Headers.Add("link", "<https://api.sendgrid.com/v3/suppression/unsubscribes?limit=50&offset=0>; rel=\"next\"; title=\"1\", <https://api.sendgrid.com/v3/suppression/unsubscribes?limit=50&offset=0>; rel=\"prev\"; title=\"1\", <https://api.sendgrid.com/v3/suppression/unsubscribes?limit=50&offset=0>; rel=\"last\"; title=\"1\", <https://api.sendgrid.com/v3/suppression/unsubscribes?limit=50&offset=0>; rel=\"first\"; title=\"1\"");
				response.Content = new StringContent(GLOBALLY_UNSUBSCRIBED);
				return response;
			});

			var client = Utils.GetFluentClient(mockHttp);
			var globalSuppressions = new GlobalSuppressions(client);

			// Act
			var result = await globalSuppressions.GetAllAsync(null, null, null, 50, 0, null, CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Records.Length.ShouldBe(3);
		}

		[Fact]
		public async Task AddAsync()
		{
			// Arrange
			var emails = new[] { "test1@example.com", "test2@example.com" };
			var apiResponse = @"{
				""recipient_emails"": [
					""test1@example.com"",
					""test2@example.com""
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var globalSuppressions = new GlobalSuppressions(client);

			// Act
			await globalSuppressions.AddAsync(emails, null, CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task RemoveAsync()
		{
			// Arrange
			var email = "test1@example.com";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, email)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var globalSuppressions = new GlobalSuppressions(client);

			// Act
			await globalSuppressions.RemoveAsync(email, null, CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task IsUnsubscribedAsync_true()
		{
			// Arrange
			var email = "test1@example.com";

			var apiResponse = @"{
				""recipient_email"": ""test1@example.com""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, email)).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var globalSuppressions = new GlobalSuppressions(client);

			// Act
			var result = await globalSuppressions.IsUnsubscribedAsync(email, null, CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBeTrue();
		}

		[Fact]
		public async Task IsUnsubscribedAsync_false()
		{
			// Arrange
			var email = "test1@example.com";

			var apiResponse = @"{
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, email)).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var globalSuppressions = new GlobalSuppressions(client);

			// Act
			var result = await globalSuppressions.IsUnsubscribedAsync(email, null, CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBeFalse();
		}
	}
}
