using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.Resources;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class AccessManagementTests
	{
		#region FIELDS

		internal const string ENDPOINT = "access_settings";

		internal const string SINGLE_ACCESS_ENTRY_JSON = @"{
			""allowed"": false,
			""auth_method"": ""basic"",
			""first_at"": 1444087966,
			""ip"": ""1.1.1.1"",
			""last_at"": 1444406672,
			""location"": ""Australia""
		}";
		internal const string MULTIPLE_ACCESS_ENTRIES_JSON = @"{
			""result"": [
				{
					""allowed"": false,
					""auth_method"": ""basic"",
					""first_at"": 1444087966,
					""ip"": ""1.1.1.1"",
					""last_at"": 1444406672,
					""location"": ""Australia""
				},
				{
					""allowed"": false,
					""auth_method"": ""basic"",
					""first_at"": 1444087505,
					""ip"": ""1.2.3.48"",
					""last_at"": 1444087505,
					""location"": ""Mukilteo, Washington""
				}
			]
		}";

		internal const string SINGLE_WHITELISTED_IP_JSON = @"{
			""id"": 1,
			""ip"": ""192.168.1.1/32"",
			""created_at"": 1441824715,
			""updated_at"": 1441824715
		}";
		internal const string MULTIPLE_WHITELISTED_IPS_JSON = @"{
			""result"": [
				{
					""id"": 1,
					""ip"": ""192.168.1.1/32"",
					""created_at"": 1441824715,
					""updated_at"": 1441824715
				},
				{
					""id"": 2,
					""ip"": ""192.168.1.2/32"",
					""created_at"": 1441824715,
					""updated_at"": 1441824715
				},
				{
					""id"": 3,
					""ip"": ""192.168.1.3/32"",
					""created_at"": 1441824715,
					""updated_at"": 1441824715
				}
			]
		}";

		#endregion

		[Fact]
		public void Parse_AccessEntry_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<AccessEntry>(SINGLE_ACCESS_ENTRY_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.Allowed.ShouldBeFalse();
			result.AuthorizationMethod.ShouldBe("basic");
			result.FirstAccessOn.ShouldBe(new DateTime(2015, 10, 5, 23, 32, 46, DateTimeKind.Utc));
			result.IpAddress.ShouldBe("1.1.1.1");
			result.LatestAccessOn.ShouldBe(new DateTime(2015, 10, 9, 16, 4, 32, DateTimeKind.Utc));
			result.Location.ShouldBe("Australia");
		}

		[Fact]
		public void Parse_WhitelistedIp_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<WhitelistedIp>(SINGLE_WHITELISTED_IP_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.Id.ShouldBe(1);
			result.IpAddress.ShouldBe("192.168.1.1/32");
			result.CreatedOn.ShouldBe(new DateTime(2015, 9, 9, 18, 51, 55, DateTimeKind.Utc));
			result.ModifiedOn.ShouldBe(new DateTime(2015, 9, 9, 18, 51, 55, DateTimeKind.Utc));
		}

		[Fact]
		public async Task GetAccessHistory()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "activity")).Respond("application/json", MULTIPLE_ACCESS_ENTRIES_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var accessManagement = new AccessManagement(client);

			// Act
			var result = await accessManagement.GetAccessHistoryAsync(20, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public async Task GetWhitelistedIpAddresses()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "whitelist")).Respond("application/json", MULTIPLE_WHITELISTED_IPS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var accessManagement = new AccessManagement(client);

			// Act
			var result = await accessManagement.GetWhitelistedIpAddressesAsync(null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(3);
		}

		[Fact]
		public async Task AddIpAddressToWhitelist()
		{
			// Arrange
			var ip = "1.1.1.1";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "whitelist")).Respond("application/json", MULTIPLE_WHITELISTED_IPS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var accessManagement = new AccessManagement(client);

			// Act
			var result = await accessManagement.AddIpAddressToWhitelistAsync(ip, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task AddIpAddressesToWhitelist()
		{
			// Arrange
			var ips = new[] { "1.1.1.1", "1.2.3.4", "5.6.7.8" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "whitelist")).Respond("application/json", MULTIPLE_WHITELISTED_IPS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var accessManagement = new AccessManagement(client);

			// Act
			var result = await accessManagement.AddIpAddressesToWhitelistAsync(ips, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(3);
		}

		[Fact]
		public async Task RemoveIpAddressFromWhitelistAsync()
		{
			// Arrange
			var id = 1111;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, "whitelist", id)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var accessManagement = new AccessManagement(client);

			// Act
			await accessManagement.RemoveIpAddressFromWhitelistAsync(id, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task RemoveIpAddressesFromWhitelistAsync()
		{
			// Arrange
			var ids = new long[] { 1111, 2222, 3333 };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, "whitelist")).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var accessManagement = new AccessManagement(client);

			// Act
			await accessManagement.RemoveIpAddressesFromWhitelistAsync(ids, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task GetWhitelistedIpAddressAsync()
		{
			// Arrange
			var id = 1111;

			var apiResponse = "{\"result\":" + SINGLE_WHITELISTED_IP_JSON + "}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "whitelist", id)).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var accessManagement = new AccessManagement(client);

			// Act
			var result = await accessManagement.GetWhitelistedIpAddressAsync(id, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}
	}
}
