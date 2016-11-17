using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using StrongGrid.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class SenderIdentitiesTests
	{
		#region FIELDS

		private const string ENDPOINT = "/senders";

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

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<SenderIdentity>(SINGLE_SENDER_IDENTITY_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.Id.ShouldBe(1);
			result.Address1.ShouldBe("123 Elm St.");
			result.Address2.ShouldBe("Apt. 456");
			result.City.ShouldBe("Denver");
			result.State.ShouldBe("Colorado");
			result.Zip.ShouldBe("80202");
			result.Country.ShouldBe("United States");
			result.Verification.ShouldNotBeNull();
			result.Verification.IsCompleted.ShouldBeTrue();
			result.Verification.Reason.ShouldBe("");
			result.Locked.ShouldBe(false);
			result.ModifiedOn.ShouldBe(new DateTime(2015, 12, 11, 22, 16, 5, DateTimeKind.Utc));
			result.NickName.ShouldBe("My Sender ID");
			result.ReplyTo.ShouldNotBeNull();
			result.ReplyTo.Email.ShouldBe("replyto@example.com");
			result.ReplyTo.Name.ShouldBe("Example INC");
			result.CreatedOn.ShouldBe(new DateTime(2015, 12, 11, 22, 16, 5, DateTimeKind.Utc));
			result.From.ShouldNotBeNull();
			result.From.Email.ShouldBe("from@example.com");
			result.From.Name.ShouldBe("Example INC");
		}

		[Fact]
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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_SENDER_IDENTITY_JSON) })
				.Verifiable();

			var senderIdentities = new SenderIdentities(mockClient.Object, ENDPOINT);

			// Act
			var result = senderIdentities.CreateAsync(nickname, from, replyTo, address, address2, city, state, zip, country, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Id.ShouldBe(1);
		}

		[Fact]
		public void GetAll()
		{
			// Arrange
			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_SENDER_IDENTITIES_JSON) })
				.Verifiable();

			var senderIdentities = new SenderIdentities(mockClient.Object, ENDPOINT);

			// Act
			var result = senderIdentities.GetAllAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].Id.ShouldBe(1);
		}

		[Fact]
		public void Update()
		{
			// Arrange
			var identityId = 1;
			var nickname = "New nickname";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync($"{ENDPOINT}/{identityId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_SENDER_IDENTITY_JSON) })
				.Verifiable();

			var senderIdentities = new SenderIdentities(mockClient.Object, ENDPOINT);

			// Act
			var result = senderIdentities.UpdateAsync(identityId, nickname, null, null, null, null, null, null, null, null, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void Delete()
		{
			// Arrange
			var identityId = 1;

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{identityId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var senderIdentities = new SenderIdentities(mockClient.Object, ENDPOINT);

			// Act
			senderIdentities.DeleteAsync(identityId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void ResendVerification()
		{
			// Arrange
			var identityId = 1;

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/{identityId}/resend_verification", (JObject)null, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var senderIdentities = new SenderIdentities(mockClient.Object, ENDPOINT);

			// Act
			senderIdentities.ResendVerification(identityId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void Get()
		{
			// Arrange
			var identityId = 1;

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{identityId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_SENDER_IDENTITY_JSON) })
				.Verifiable();

			var senderIdentities = new SenderIdentities(mockClient.Object, ENDPOINT);

			// Act
			var result = senderIdentities.GetAsync(identityId, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Id.ShouldBe(identityId);
		}
	}
}
