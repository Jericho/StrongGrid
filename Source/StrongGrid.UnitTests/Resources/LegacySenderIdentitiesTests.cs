using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Resources.Legacy;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class LegacySenderIdentitiesTests
	{
		internal const string ENDPOINT = "senders";

		internal const string SINGLE_SENDER_IDENTITY_JSON = @"{
			""id"": 1,
			""nickname"": ""My Sender ID"",
			""from"": {
				""email"": ""from@example.com"",
				""name"": ""Example INC""
			},
			""reply_to"": {
				""email"": ""replyto@example.com"",
				""name"": ""Example INC""
			},
			""address"": ""123 Elm St."",
			""address_2"": ""Apt. 456"",
			""city"": ""Denver"",
			""state"": ""Colorado"",
			""zip"": ""80202"",
			""country"": ""United States"",
			""verified"": { ""status"": true, ""reason"": """" },
			""updated_at"": 1449872165,
			""created_at"": 1449872165,
			""locked"": false
		}";
		internal const string MULTIPLE_SENDER_IDENTITIES_JSON = @"[
			{
				""id"": 1,
				""nickname"": ""My Sender ID"",
				""from"": {
					""email"": ""from@example.com"",
					""name"": ""Example INC""
				},
				""reply_to"": {
					""email"": ""replyto@example.com"",
					""name"": ""Example INC""
				},
				""address"": ""123 Elm St."",
				""address_2"": ""Apt. 456"",
				""city"": ""Denver"",
				""state"": ""Colorado"",
				""zip"": ""80202"",
				""country"": ""United States"",
				""verified"": { ""status"": true, ""reason"": """" },
				""updated_at"": 1449872165,
				""created_at"": 1449872165,
				""locked"": false
			}
		]";

		private readonly ITestOutputHelper _outputHelper;

		public LegacySenderIdentitiesTests(ITestOutputHelper outputHelper)
		{
			_outputHelper = outputHelper;
		}

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<StrongGrid.Models.Legacy.SenderIdentity>(SINGLE_SENDER_IDENTITY_JSON, JsonFormatter.DeserializerOptions);

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
		public async Task CreateAsync()
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_SENDER_IDENTITY_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var senderIdentities = new SenderIdentities(client);

			// Act
			var result = await senderIdentities.CreateAsync(nickname, from, replyTo, address, address2, city, state, zip, country, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Id.ShouldBe(1);
		}

		[Fact]
		public async Task GetAllAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", MULTIPLE_SENDER_IDENTITIES_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var senderIdentities = new SenderIdentities(client);

			// Act
			var result = await senderIdentities.GetAllAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].Id.ShouldBe(1);
		}

		[Fact]
		public async Task UpdateAsync()
		{
			// Arrange
			var identityId = 1;
			var nickname = "New nickname";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT, identityId)).Respond("application/json", SINGLE_SENDER_IDENTITY_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var senderIdentities = new SenderIdentities(client);

			// Act
			var result = await senderIdentities.UpdateAsync(identityId, nickname, null, null, null, null, null, null, null, null, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task DeleteAsync()
		{
			// Arrange
			var identityId = 1;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, identityId)).Respond(HttpStatusCode.NoContent);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var senderIdentities = new SenderIdentities(client);

			// Act
			await senderIdentities.DeleteAsync(identityId, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task ResendVerificationAsync()
		{
			// Arrange
			var identityId = 1;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, identityId, "resend_verification")).Respond(HttpStatusCode.NoContent);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var senderIdentities = new SenderIdentities(client);

			// Act
			await senderIdentities.ResendVerification(identityId, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task GetAsync()
		{
			// Arrange
			var identityId = 1;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, identityId)).Respond("application/json", SINGLE_SENDER_IDENTITY_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var senderIdentities = new SenderIdentities(client);

			// Act
			var result = await senderIdentities.GetAsync(identityId, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Id.ShouldBe(identityId);
		}
	}
}
