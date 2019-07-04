using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
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

			var personalizations = new[]
			{
				new MailPersonalization()
				{
					To = new[] { new MailAddress("bob@example.com", "Bob Smith") }
				}
			};

			// Act
			var result = await mail.SendAsync(personalizations, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, MailPriority.Normal, CancellationToken.None).ConfigureAwait(false);

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

			var personalizations = new[]
			{
				new MailPersonalization()
				{
					To = new[] { new MailAddress("bob@example.com", "Bob Smith") }
				}
			};

			// Act
			var result = await mail.SendAsync(personalizations, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, MailPriority.Normal, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBe("abc123");
		}

		[Fact]
		// Up until v0.57.1 this would cause a ArgumentNull exception. See GH-286. Fixed in v0.58.0
		public async Task SendAsync_request_with_bcc_only()
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

			var personalizations = new[]
			{
				new MailPersonalization()
				{
					Bcc = new[] { new MailAddress("bob@example.com", "Bob Smith"), new MailAddress("bob@example.com", "Bob Smith") }
				}
			};

			// Act
			var result = await mail.SendAsync(personalizations, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, MailPriority.Normal, CancellationToken.None).ConfigureAwait(false);

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

		[Fact]
		public async Task EmailExceedsSizeLimit()
		{
			// Arrange
			var from = new MailAddress("bob@exmaple.com", "Bob Smith");
			var subject = new string('a', 10 * 1024 * 1024);
			var recipients = new List<MailAddress>();
			for (int i = 0; i < 999; i++)
			{
				recipients.Add(new MailAddress($"{i}@{i}.com", $"{i} {i}"));
			}
			var personalizations = recipients.Select(r => new MailPersonalization { To = new[] { r } });
			var contents = new[]
			{
				new MailContent("text/plain", new string('b', 10 * 1024 * 1024)),
				new MailContent("text/html", new string('v', 10 * 1024 * 1024))
			};

			var mail = new Mail(null);

			// Act
			var result = await Should.ThrowAsync<Exception>(async () => await mail.SendAsync(personalizations, subject, contents, from).ConfigureAwait(false));

			// Assert
		}
	}
}
