using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Resources;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class UserTests
	{
		internal const string ENDPOINT = "user/profile";

		internal const string SINGLE_PROFILE_JSON = @"{
			""address"": ""814 West Chapman Avenue"",
			""city"": ""Orange"",
			""company"": ""SendGrid"",
			""country"": ""US"",
			""first_name"": ""Test"",
			""last_name"": ""User"",
			""phone"": ""555-555-5555"",
			""state"": ""CA"",
			""website"": ""http://www.sendgrid.com"",
			""zip"": ""92868""
		}";

		private readonly ITestOutputHelper _outputHelper;

		public UserTests(ITestOutputHelper outputHelper)
		{
			_outputHelper = outputHelper;
		}

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<UserProfile>(SINGLE_PROFILE_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.Address.ShouldBe("814 West Chapman Avenue");
			result.City.ShouldBe("Orange");
			result.Company.ShouldBe("SendGrid");
			result.Country.ShouldBe("US");
			result.FirstName.ShouldBe("Test");
			result.LastName.ShouldBe("User");
			result.Phone.ShouldBe("555-555-5555");
			result.State.ShouldBe("CA");
			result.Website.ShouldBe("http://www.sendgrid.com");
			result.ZipCode.ShouldBe("92868");
		}

		[Fact]
		public async Task GetProfileAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_PROFILE_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var user = new User(client);

			// Act
			var result = await user.GetProfileAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetAccountAsync()
		{
			// Arrange
			var apiResponse = @"{
				""type"": ""free"",
				""reputation"": 99.7
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("user/account")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var user = new User(client);

			// Act
			var result = await user.GetAccountAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Type.ShouldBe("free");
			result.Reputation.ShouldBe(99.7F);
		}

		[Fact]
		public async Task UpdateProfileAsync()
		{
			// Arrange
			var city = "New York";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_PROFILE_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var user = new User(client);

			// Act
			var result = await user.UpdateProfileAsync(null, city, null, null, null, null, null, null, null, null, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetEmailAsync()
		{
			// Arrange
			var apiResponse = @"{
				""email"": ""test@example.com""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("user/email")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var user = new User(client);

			// Act
			var result = await user.GetEmailAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBe("test@example.com");
		}

		[Fact]
		public async Task UpdateEmailAsync()
		{
			// Arrange
			var email = "test@example.com";

			var apiResponse = @"{
				""email"": ""test@example.com""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Put, Utils.GetSendGridApiUri("user/email")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var user = new User(client);

			// Act
			var result = await user.UpdateEmailAsync(email, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBe("test@example.com");
		}

		[Fact]
		public async Task GetUsernameAsync()
		{
			// Arrange
			var apiResponse = @"{
				""username"": ""test_username""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("user/username")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var user = new User(client);

			// Act
			var result = await user.GetUsernameAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBe("test_username");
		}

		[Fact]
		public async Task UpdateUsernameAsync()
		{
			// Arrange
			var username = "test_username";

			var apiResponse = @"{
				""username"": ""test_username""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Put, Utils.GetSendGridApiUri("user/username")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var user = new User(client);

			// Act
			var result = await user.UpdateUsernameAsync(username, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBe(username);
		}

		[Fact]
		public async Task GetCreditsAsync()
		{
			// Arrange
			var apiResponse = @"{
				""remain"": 200,
				""total"": 200,
				""overage"": 0,
				""used"": 0,
				""last_reset"": ""2013-01-01"",
				""next_reset"": ""2013-02-01"",
				""reset_frequency"": ""monthly""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("user/credits")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var user = new User(client);

			// Act
			var result = await user.GetCreditsAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Remaining.ShouldBe(200);
			result.Total.ShouldBe(200);
			result.Overage.ShouldBe(0);
			result.Used.ShouldBe(0);
			result.ResetFrequency.ShouldBe("monthly");
			result.LastReset.ShouldBe(new DateTime(2013, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
			result.NextReset.ShouldBe(new DateTime(2013, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc));
		}

		[Fact]
		public async Task UpdatePasswordAsync()
		{
			// Arrange
			var oldPassword = "azerty";
			var newPassword = "qwerty";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Put, Utils.GetSendGridApiUri("user/password")).Respond(HttpStatusCode.OK);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var user = new User(client);

			// Act
			await user.UpdatePasswordAsync(oldPassword, newPassword, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task GetPermissionsAsync()
		{
			// Arrange
			var apiResponse = @"{
				""scopes"": [
					""aaa"",
					""bbb"",
					""ccc""
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("scopes")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var user = new User(client);

			// Act
			var result = await user.GetPermissionsAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(3);
		}
	}
}
