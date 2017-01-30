using Moq;
using Newtonsoft.Json.Linq;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Model;
using StrongGrid.UnitTests;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class MailTests
	{
		#region FIELDS

		private const string ENDPOINT = "/mail";

		#endregion

		[Fact]
		public void Send()
		{
			// Arrange

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, $"{ENDPOINT}/send").Respond(HttpStatusCode.Accepted);

			var client = Utils.GetFluentClient(mockHttp);
			var mail = new Mail(client, ENDPOINT);

			// Act
			mail.SendAsync(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void SendToSingleRecipient()
		{
			// Arrange

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, $"{ENDPOINT}/send").Respond(HttpStatusCode.Accepted);

			var client = Utils.GetFluentClient(mockHttp);
			var mail = new Mail(client, ENDPOINT);

			// Act
			mail.SendToSingleRecipientAsync(null, null, null, null, null).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void SendToMultipleRecipients()
		{
			// Arrange

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, $"{ENDPOINT}/send").Respond(HttpStatusCode.Accepted);

			var recipients = new[]
			{
				new MailAddress("a@a.com", "a"),
				new MailAddress("b@b.com", "b"),
				new MailAddress("c@c.com", "c")
			};

			var client = Utils.GetFluentClient(mockHttp);
			var mail = new Mail(client, ENDPOINT);

			// Act
			mail.SendToMultipleRecipientsAsync(recipients, null, null, null, null).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}
	}
}
