using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class MailTests
	{
		private const string ENDPOINT = "/mail";

		[TestMethod]
		public void Send()
		{
			// Arrange
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync($"{ENDPOINT}/send", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Accepted));

			var mail = new Mail(mockClient.Object);

			// Act
			mail.SendAsync(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, CancellationToken.None).Wait();

			// Assert
		}

		[TestMethod]
		public void SendToSingleRecipient()
		{
			// Arrange
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync($"{ENDPOINT}/send", It.Is<JObject>(o => o["personalizations"].ToObject<MailPersonalization[]>().Length == 1), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Accepted));

			var mail = new Mail(mockClient.Object);

			// Act
			mail.SendToSingleRecipientAsync(null, null, null, null, null).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void SendToMultipleRecipients()
		{
			// Arrange
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync($"{ENDPOINT}/send", It.Is<JObject>(o => o["personalizations"].ToObject<MailPersonalization[]>().Length == 3), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Accepted));

			var recipients = new[]
			{
				new MailAddress("a@a.com", "a"),
				new MailAddress("b@b.com", "b"),
				new MailAddress("c@c.com", "c")
			};
			var mail = new Mail(mockClient.Object);

			// Act
			mail.SendToMultipleRecipientsAsync(recipients, null, null, null, null).Wait(CancellationToken.None);

			// Assert
		}
	}
}
