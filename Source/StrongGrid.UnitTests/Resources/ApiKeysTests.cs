using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Resources;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class ApiKeysTests
	{
		#region FIELDS

		internal const string ENDPOINT = "api_keys";

		internal const string SINGLE_API_KEY_JSON = @"{
			""api_key"": ""SG.xxxxxxxx.yyyyyyyy"",
			""api_key_id"": ""xxxxxxxx"",
			""name"": ""My API Key"",
			""scopes"": [
				""mail.send"",
				""alerts.create"",
				""alerts.read""
			]
		}";
		internal const string MULTIPLE_API_KEY_JSON = @"{
			""result"": [
				{
					""name"": ""A New Hope"",
					""api_key_id"": ""xxxxxxxx""
				},
				{
					""name"": ""Another key"",
					""api_key_id"": ""yyyyyyyy""
				}
			]
		}";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<ApiKey>(SINGLE_API_KEY_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.Key.ShouldBe("SG.xxxxxxxx.yyyyyyyy");
			result.KeyId.ShouldBe("xxxxxxxx");
			result.Name.ShouldBe("My API Key");
			result.Scopes.ShouldBe(new[] { "mail.send", "alerts.create", "alerts.read" });
		}

		[Fact]
		public async Task CreateAsync()
		{
			// Arrange
			var name = "My API Key";
			var scopes = new[] { "mail.send", "alerts.create", "alerts.read" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_API_KEY_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var apiKeys = new ApiKeys(client);

			// Act
			var result = await apiKeys.CreateAsync(name, scopes, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetAsync()
		{
			// Arrange
			var keyId = "xxxxxxxx";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, keyId)).Respond("application/json", SINGLE_API_KEY_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var apiKeys = new ApiKeys(client);

			// Act
			var result = await apiKeys.GetAsync(keyId, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetAllAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", MULTIPLE_API_KEY_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var apiKeys = new ApiKeys(client);

			// Act
			var result = await apiKeys.GetAllAsync(cancellationToken: CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public async Task DeleteAsync()
		{
			// Arrange
			var keyId = "xxxxxxxx";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, keyId)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var apiKeys = new ApiKeys(client);

			// Act
			await apiKeys.DeleteAsync(keyId, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task UpdateAsync_with_scopes()
		{
			// Arrange
			var keyId = "xxxxxxxx";
			var name = "My API Key";
			var scopes = new[] { "mail.send", "alerts.create", "alerts.read" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Put, Utils.GetSendGridApiUri(ENDPOINT, keyId)).Respond("application/json", SINGLE_API_KEY_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var apiKeys = new ApiKeys(client);

			// Act
			var result = await apiKeys.UpdateAsync(keyId, name, scopes, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task UpdateAsync_without_scopes()
		{
			// Arrange
			var keyId = "xxxxxxxx";
			var name = "My API Key";
			var scopes = (string[])null;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT, keyId)).Respond("application/json", SINGLE_API_KEY_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var apiKeys = new ApiKeys(client);

			// Act
			var result = await apiKeys.UpdateAsync(keyId, name, scopes, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task CreateWithBillingPermissionsAsync()
		{
			// Arrange
			var name = "API Key with billing permissions";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_API_KEY_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var apiKeys = new ApiKeys(client);

			// Act
			var result = await apiKeys.CreateWithBillingPermissionsAsync(name, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task CreateWithAllPermissionsAsync()
		{
			// Arrange
			var name = "My API Key with all permissions";
			var userScopesJson = @"{
				""scopes"": [
					""aaa"",
					""bbb"",
					""ccc""
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("scopes")).Respond("application/json", userScopesJson);
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_API_KEY_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var apiKeys = new ApiKeys(client);

			// Act
			var result = await apiKeys.CreateWithAllPermissionsAsync(name, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task CreateWithReadOnlyPermissionsAsync()
		{
			// Arrange
			var name = "My API Key with read-only permissions";
			var userScopesJson = @"{
				""scopes"": [
					""aaa"",
					""bbb"",
					""ccc""
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("scopes")).Respond("application/json", userScopesJson);
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_API_KEY_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var apiKeys = new ApiKeys(client);

			// Act
			var result = await apiKeys.CreateWithReadOnlyPermissionsAsync(name, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}
	}
}
