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
		#region FIELDS

		private const string ENDPOINT = "/mail";
		private MockRepository _mockRepository;
		private Mock<IClient> _mockClient;

		#endregion

		private Mail CreateMail()
		{
			return new Mail(_mockClient.Object, ENDPOINT);
		}

		[TestInitialize]
		public void TestInitialize()
		{
			_mockRepository = new MockRepository(MockBehavior.Strict);
			_mockClient = _mockRepository.Create<IClient>();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			_mockRepository.VerifyAll();
		}

		[TestMethod]
		public void Send()
		{
			// Arrange

			_mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/send", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Accepted))
				.Verifiable();

			var mail = CreateMail();

			// Act
			mail.SendAsync(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, CancellationToken.None).Wait();

			// Assert
		}

		[TestMethod]
		public void SendToSingleRecipient()
		{
			// Arrange

			_mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/send", It.Is<JObject>(o => o["personalizations"].ToObject<MailPersonalization[]>().Length == 1), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Accepted))
				.Verifiable();

			var mail = CreateMail();

			// Act
			mail.SendToSingleRecipientAsync(null, null, null, null, null).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void SendToMultipleRecipients()
		{
			// Arrange

			_mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/send", It.Is<JObject>(o => o["personalizations"].ToObject<MailPersonalization[]>().Length == 3), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Accepted))
				.Verifiable();

			var recipients = new[]
			{
				new MailAddress("a@a.com", "a"),
				new MailAddress("b@b.com", "b"),
				new MailAddress("c@c.com", "c")
			};
			var mail = CreateMail();

			// Act
			mail.SendToMultipleRecipientsAsync(recipients, null, null, null, null).Wait(CancellationToken.None);

			// Assert
		}
	}
}
