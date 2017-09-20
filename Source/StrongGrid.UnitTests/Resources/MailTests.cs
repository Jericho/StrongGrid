using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.Resources;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class MailTests
	{
		#region FIELDS

		private const string ENDPOINT = "mail";

		#endregion

		[Fact]
		public async Task SendAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "send")).Respond(HttpStatusCode.Accepted);

			var client = Utils.GetFluentClient(mockHttp);
			var mail = new Mail(client);

			// Act
			var result = await mail.SendAsync(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBeNull();
		}

		[Fact]
		public async Task SendAsync_response_with_message_id()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "send")).Respond((HttpRequestMessage request) =>
			{
				var response = new HttpResponseMessage(HttpStatusCode.OK);
				response.Headers.Add("X-Message-Id", "abc123");
				return response;
			});

			var client = Utils.GetFluentClient(mockHttp);
			var mail = new Mail(client);

			// Act
			var result = await mail.SendAsync(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBe("abc123");
		}

		[Fact]
		public async Task SendToSingleRecipientAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "send")).Respond(HttpStatusCode.Accepted);

			var client = Utils.GetFluentClient(mockHttp);
			var mail = new Mail(client);

			// Act
			var result = await mail.SendToSingleRecipientAsync(null, null, null, null, null).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBeNull();
		}

		[Fact]
		public async Task SendToMultipleRecipientsAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "send")).Respond(HttpStatusCode.Accepted);

			var recipients = new[]
			{
				new MailAddress("a@a.com", "a"),
				new MailAddress("b@b.com", "b"),
				new MailAddress("c@c.com", "c")
			};

			var client = Utils.GetFluentClient(mockHttp);
			var mail = new Mail(client);

			// Act
			var result = await mail.SendToMultipleRecipientsAsync(recipients, null, null, null, null).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBeNull();
		}
	}
}
