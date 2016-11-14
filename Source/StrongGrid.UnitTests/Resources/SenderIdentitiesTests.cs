using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class SenderIdentitiesTests
	{
		#region FIELDS

		private const string ENDPOINT = "/senders";
		private MockRepository _mockRepository;
		private Mock<IClient> _mockClient;

		private const string SINGLE_SENDER_IDENTITY_JSON = @"{
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
		private const string MULTIPLE_SENDER_IDENTITIES_JSON = @"[
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

		#endregion

		private SenderIdentities CreateSenderIdentities()
		{
			return new SenderIdentities(_mockClient.Object, ENDPOINT);

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
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<SenderIdentity>(SINGLE_SENDER_IDENTITY_JSON);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Id);
			Assert.AreEqual("123 Elm St.", result.Address1);
			Assert.AreEqual("Apt. 456", result.Address2);
			Assert.AreEqual("Denver", result.City);
			Assert.AreEqual("Colorado", result.State);
			Assert.AreEqual("80202", result.Zip);
			Assert.AreEqual("United States", result.Country);
			Assert.IsNotNull(result.Verification);
			Assert.IsTrue(result.Verification.IsCompleted);
			Assert.AreEqual("", result.Verification.Reason);
			Assert.AreEqual(false, result.Locked);
			Assert.AreEqual(new DateTime(2015, 12, 11, 22, 16, 5, DateTimeKind.Utc), result.ModifiedOn);
			Assert.AreEqual("My Sender ID", result.NickName);
			Assert.IsNotNull(result.ReplyTo);
			Assert.AreEqual("replyto@example.com", result.ReplyTo.Email);
			Assert.AreEqual("Example INC", result.ReplyTo.Name);
			Assert.AreEqual(new DateTime(2015, 12, 11, 22, 16, 5, DateTimeKind.Utc), result.CreatedOn);
			Assert.IsNotNull(result.From);
			Assert.AreEqual("from@example.com", result.From.Email);
			Assert.AreEqual("Example INC", result.From.Name);
		}

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

			_mockClient
				.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_SENDER_IDENTITY_JSON) })
				.Verifiable();

			var senderIdentities = CreateSenderIdentities();

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
			_mockClient
				.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_SENDER_IDENTITIES_JSON) })
				.Verifiable();

			var senderIdentities = CreateSenderIdentities();

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

			_mockClient
				.Setup(c => c.PatchAsync($"{ENDPOINT}/{identityId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_SENDER_IDENTITY_JSON) })
				.Verifiable();

			var senderIdentities = CreateSenderIdentities();

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

			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{identityId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var senderIdentities = CreateSenderIdentities();

			// Act
			senderIdentities.DeleteAsync(identityId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void ResendVerification()
		{
			// Arrange
			var identityId = 1;

			_mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/{identityId}/resend_verification", (JObject)null, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var senderIdentities = CreateSenderIdentities();

			// Act
			senderIdentities.ResendVerification(identityId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var identityId = 1;

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{identityId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_SENDER_IDENTITY_JSON) })
				.Verifiable();

			var senderIdentities = CreateSenderIdentities();

			// Act
			var result = senderIdentities.GetAsync(identityId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(identityId, result.Id);
		}
	}
}
