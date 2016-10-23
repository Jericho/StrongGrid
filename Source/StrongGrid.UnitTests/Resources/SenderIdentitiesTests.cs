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
	public class SenderIdentitiesTests
	{
		private const string ENDPOINT = "/senders";

		[TestMethod]
		public void Create()
		{
			// Arrange
			var nickname = "My Sender ID";
			var from = new MailAddress("from@example.com", "Example INC");
			var replyTo = new MailAddress("replyto@example.com", "Example INC");
			var address = "123 Elm St.";
			var address2 = "Apt. 456";
			var city = "Denver";
			var state = "Colorado";
			var zip = "80202";
			var country = "United States";

			var apiResponse = @"{
				'id': 1,
				'nickname': 'My Sender ID',
				'from': {
					'email': 'from@example.com',
					'name': 'Example INC'
				},
				'reply_to': {
					'email': 'replyto@example.com',
					'name': 'Example INC'
				},
				'address': '123 Elm St.',
				'address_2': 'Apt. 456',
				'city': 'Denver',
				'state': 'Colorado',
				'zip': '80202',
				'country': 'United States',
				'verified': { 'status': true, 'reason': '' },
				'updated_at': 1449872165,
				'created_at': 1449872165,
				'locked': false
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var senderIdentities = new SenderIdentities(mockClient.Object);

			// Act
			var result = senderIdentities.CreateAsync(nickname, from, replyTo, address, address2, city, state, zip, country, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Id);
		}

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			var apiResponse = @"[
				{
					'id': 1,
					'nickname': 'My Sender ID',
					'from': {
						'email': 'from@example.com',
						'name': 'Example INC'
					},
					'reply_to': {
						'email': 'replyto@example.com',
						'name': 'Example INC'
					},
					'address': '123 Elm St.',
					'address_2': 'Apt. 456',
					'city': 'Denver',
					'state': 'Colorado',
					'zip': '80202',
					'country': 'United States',
					'verified': { 'status': true, 'reason': '' },
					'updated_at': 1449872165,
					'created_at': 1449872165,
					'locked': false
				}
			]";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var senderIdentities = new SenderIdentities(mockClient.Object);

			// Act
			var result = senderIdentities.GetAllAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual(1, result[0].Id);
		}

		[TestMethod]
		public void Update()
		{
			// Arrange
			var identityId = 1;
			var nickname = "New nickname";

			var apiResponse = @"{
				'id': 1,
				'nickname': 'New nickname',
				'from': {
					'email': 'from@example.com',
					'name': 'Example INC'
				},
				'reply_to': {
					'email': 'replyto@example.com',
					'name': 'Example INC'
				},
				'address': '123 Elm St.',
				'address_2': 'Apt. 456',
				'city': 'Denver',
				'state': 'Colorado',
				'zip': '80202',
				'country': 'United States',
				'verified': { 'status': true, 'reason': '' },
				'updated_at': 1449872165,
				'created_at': 1449872165,
				'locked': false
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PatchAsync($"{ENDPOINT}/{identityId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var senderIdentities = new SenderIdentities(mockClient.Object);

			// Act
			var result = senderIdentities.UpdateAsync(identityId, nickname, null, null, null, null, null, null, null, null, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			var identityId = 1;

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync($"{ENDPOINT}/{identityId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var senderIdentities = new SenderIdentities(mockClient.Object);

			// Act
			senderIdentities.DeleteAsync(identityId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void ResendVerification()
		{
			// Arrange
			var identityId = 1;

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync($"{ENDPOINT}/{identityId}/resend_verification", (JObject)null, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var senderIdentities = new SenderIdentities(mockClient.Object);

			// Act
			senderIdentities.ResendVerification(identityId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var identityId = 1;
			var apiResponse = @"{
				'id': 1,
				'nickname': 'My Sender ID',
				'from': {
					'email': 'from@example.com',
					'name': 'Example INC'
				},
				'reply_to': {
					'email': 'replyto@example.com',
					'name': 'Example INC'
				},
				'address': '123 Elm St.',
				'address_2': 'Apt. 456',
				'city': 'Denver',
				'state': 'Colorado',
				'zip': '80202',
				'country': 'United States',
				'verified': { 'status': true, 'reason': '' },
				'updated_at': 1449872165,
				'created_at': 1449872165,
				'locked': false
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync($"{ENDPOINT}/{identityId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var senderIdentities = new SenderIdentities(mockClient.Object);

			// Act
			var result = senderIdentities.GetAsync(identityId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(identityId, result.Id);
		}
	}
}
