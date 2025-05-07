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
	public class IpAddressesTests
	{
		internal const string ENDPOINT = "ips";

		internal const string SINGLE_ASSIGNED_IPADDRESS_JSON = @"{
			""ip"": ""192.168.1.1"",
			""pools"": [
				""pool1"",
				""pool2""
			],
			""whitelabeled"": false,
			""start_date"": 1409616000,
			""subusers"": [
				""tim@sendgrid.net""
			],
			""warmup"": false,
			""assigned_at"": 1482883200
		}";

		internal const string SINGLE_UNASSIGNED_IPADDRESS_JSON = @"{
			""ip"": ""208.115.214.22"",
			""pools"": [],
			""whitelabeled"": true,
			""rdns"": ""o1.email.burgermail.com"",
			""start_date"": 1409616000,
			""subusers"": [],
			""warmup"": false,
			""assigned_at"": 1482883200
		}";

		internal const string MULTIPLE_IPADDRESSES_JSON = "[" +
			SINGLE_ASSIGNED_IPADDRESS_JSON + "," +
			SINGLE_UNASSIGNED_IPADDRESS_JSON +
		"]";

		private readonly ITestOutputHelper _outputHelper;

		public IpAddressesTests(ITestOutputHelper outputHelper)
		{
			_outputHelper = outputHelper;
		}

		[Fact]
		public void Parse_single_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<IpAddress>(SINGLE_ASSIGNED_IPADDRESS_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.Address.ShouldBe("192.168.1.1");
			result.AssignedOn.ShouldBe(new DateTime(2016, 12, 28));
			result.Pools.Length.ShouldBe(2);
			result.Pools[0].ShouldBe("pool1");
			result.Pools[1].ShouldBe("pool2");
			result.ReverseDns.ShouldBeNull();
			result.Subusers.Length.ShouldBe(1);
			result.Subusers[0].ShouldBe("tim@sendgrid.net");
			result.Warmup.ShouldBeFalse();
			result.WarmupStartedOn.ShouldBe(new DateTime(2014, 9, 2));
			result.WhiteLabeled.ShouldBeFalse();
		}

		[Fact]
		public void Parse_multiple_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<IpAddress[]>(MULTIPLE_IPADDRESSES_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);

			result[1].Address.ShouldBe("208.115.214.22");
			result[1].AssignedOn.ShouldBe(new DateTime(2016, 12, 28));
			result[1].Pools.Length.ShouldBe(0);
			result[1].ReverseDns.ShouldBe("o1.email.burgermail.com");
			result[1].Subusers.Length.ShouldBe(0);
			result[1].Warmup.ShouldBeFalse();
			result[1].WarmupStartedOn.ShouldBe(new DateTime(2014, 9, 2));
			result[1].WhiteLabeled.ShouldBeTrue();
		}

		[Fact]
		public async Task AddAsync()
		{
			// Arrange
			var apiResponse = @"{
				""ips"": [
					{
						""ip"": ""1.2.3.4"",
						""subusers"": [ ""jdesautels"" ]
					}
				],
				""remaining_ips"":2,
				""warmup"": false
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var ipAddresses = new IpAddresses(client);

			// Act
			var result = await ipAddresses.AddAsync(2, new[] { "user", "subuser1" }, true, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.IpAddresses.ShouldNotBeNull();
			result.IpAddresses.Length.ShouldBe(1);
			result.IpAddresses[0].Address.ShouldBe("1.2.3.4");
			result.RemainingIpAddresses.ShouldBe(2);
			result.WarmingUp.ShouldBeFalse();
		}

		[Fact]
		public async Task GetRemainingCountAsync()
		{
			// Arrange
			var apiResponse = @"{
				""results"": [
					{
						""remaining"": 2,
						""period"": ""month"",
						""price_per_ip"": 20
					}
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "remaining")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var ipAddresses = new IpAddresses(client);

			// Act
			var result = await ipAddresses.GetRemainingCountAsync(TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Period.ShouldBe("month");
			result.PricePerIp.ShouldBe(20.0);
			result.Remaining.ShouldBe(2);
		}

		[Fact]
		public async Task GetAllAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", MULTIPLE_IPADDRESSES_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var ipAddresses = new IpAddresses(client);

			// Act
			var result = await ipAddresses.GetAllAsync(false, null, 10, 0, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public async Task GetAssignedAsync()
		{
			// Arrange
			var apiResponse = "[" +
				SINGLE_ASSIGNED_IPADDRESS_JSON +
			"]";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "assigned")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var ipAddresses = new IpAddresses(client);

			// Act
			var result = await ipAddresses.GetAssignedAsync(TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
		}

		[Fact]
		public async Task GetUnassignedAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT) + $"?exclude_whitelabels=false&limit=500&offset=0&sort_by_direction=asc").Respond("application/json", MULTIPLE_IPADDRESSES_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var ipAddresses = new IpAddresses(client);

			// Act
			var result = await ipAddresses.GetUnassignedAsync(TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
		}

		[Fact]
		public async Task GetWarmingUpAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "warmup")).Respond("application/json", MULTIPLE_IPADDRESSES_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var ipAddresses = new IpAddresses(client);

			// Act
			var result = await ipAddresses.GetWarmingUpAsync(TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public async Task GetWarmUpStatusAsync()
		{
			// Arrange
			var address = "192.168.1.1";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "warmup", address)).Respond("application/json", MULTIPLE_IPADDRESSES_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var ipAddresses = new IpAddresses(client);

			// Act
			var result = await ipAddresses.GetWarmupStatusAsync(address, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task StartWarmupAsync()
		{
			// Arrange
			var address = "192.168.1.1";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "warmup")).Respond(HttpStatusCode.OK);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var ipAddresses = new IpAddresses(client);

			// Act
			await ipAddresses.StartWarmupAsync(address, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task StopWarmupAsync()
		{
			// Arrange
			var address = "192.168.1.1";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, "warmup", address)).Respond(HttpStatusCode.OK);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var ipAddresses = new IpAddresses(client);

			// Act
			await ipAddresses.StopWarmupAsync(address, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}
	}
}
