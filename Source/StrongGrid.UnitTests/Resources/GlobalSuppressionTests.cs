using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.UnitTests;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class GlobalSuppressionTests
	{
		#region FIELDS

		private const string ENDPOINT = "asm/suppressions/global";

		#endregion

		[Fact]
		public void Add()
		{
			// Arrange
			var emails = new[] { "test1@example.com", "test2@example.com" };
			var apiResponse = @"{
				'recipient_emails': [
					'test1@example.com',
					'test2@example.com'
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var globalSuppressions = new GlobalSuppressions(client);

			// Act
			globalSuppressions.AddAsync(emails, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void Delete()
		{
			// Arrange
			var email = "test1@example.com";


			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, email)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var globalSuppressions = new GlobalSuppressions(client);

			// Act
			globalSuppressions.RemoveAsync(email, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void IsUnsubscribed_true()
		{
			// Arrange
			var email = "test1@example.com";

			var apiResponse = @"{
				'recipient_email': 'test1@example.com'
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, email)).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var globalSuppressions = new GlobalSuppressions(client);

			// Act
			var result = globalSuppressions.IsUnsubscribedAsync(email, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBeTrue();
		}

		[Fact]
		public void IsUnsubscribed_false()
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
			var result = globalSuppressions.IsUnsubscribedAsync(email, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBeFalse();
		}
	}
}
