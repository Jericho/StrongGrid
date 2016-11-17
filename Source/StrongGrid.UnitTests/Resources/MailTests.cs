using Moq;
using Newtonsoft.Json.Linq;
using Shouldly;
using StrongGrid.Model;
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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/send", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Accepted))
				.Verifiable();

			var mail = new Mail(mockClient.Object, ENDPOINT);

			// Act
			mail.SendAsync(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, CancellationToken.None).Wait();

			// Assert
		}

		[Fact]
		public void SendToSingleRecipient()
		{
			// Arrange

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/send", It.Is<JObject>(o => o["personalizations"].ToObject<MailPersonalization[]>().Length == 1), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Accepted))
				.Verifiable();

			var mail = new Mail(mockClient.Object, ENDPOINT);

			// Act
			mail.SendToSingleRecipientAsync(null, null, null, null, null).Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void SendToMultipleRecipients()
		{
			// Arrange

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/send", It.Is<JObject>(o => o["personalizations"].ToObject<MailPersonalization[]>().Length == 3), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Accepted))
				.Verifiable();

			var recipients = new[]
			{
				new MailAddress("a@a.com", "a"),
				new MailAddress("b@b.com", "b"),
				new MailAddress("c@c.com", "c")
			};
			var mail = new Mail(mockClient.Object, ENDPOINT);

			// Act
			mail.SendToMultipleRecipientsAsync(recipients, null, null, null, null).Wait(CancellationToken.None);

			// Assert
		}
	}
}
