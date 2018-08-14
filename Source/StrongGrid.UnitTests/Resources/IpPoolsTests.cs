using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.Resources;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class IpPoolsTests
	{
		#region FIELDS

		private const string ENDPOINT = "ips/pools";

		private const string SINGLE_IPPOOL_JSON = @"{
			'name': 'marketing',
			'ips':
			[
				{ 'ip': '1.1.1.1', 'start_date': null, 'warmup': false },
				{ 'ip': '2.2.2.2', 'start_date': null, 'warmup': false },
				{ 'ip': '3.3.3.3', 'start_date': null, 'warmup': false }
			]
		}";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<IpPool>(SINGLE_IPPOOL_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.Name.ShouldBe("marketing");
			result.IpAddresses.Length.ShouldBe(3);
			result.IpAddresses[0].Address.ShouldBe("1.1.1.1");
			result.IpAddresses[1].Address.ShouldBe("2.2.2.2");
			result.IpAddresses[2].Address.ShouldBe("3.3.3.3");
		}

		[Fact]
		public async Task CreateAsync()
		{
			// Arrange
			var name = "marketing";

			var apiResponse = @"{
				'name': 'marketing'
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var ipPools = new IpPools(client);

			// Act
			var result = await ipPools.CreateAsync(name, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.ShouldBe(name);
		}

		[Fact]
		public async Task GetAllNamesAsync()
		{
			// Arrange
			var apiResponse = @"[
				{
					'name': 'marketing'
				},
				{
					'name': 'transactional'
				}
			]";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var ipPools = new IpPools(client);

			// Act
			var result = await ipPools.GetAllNamesAsync(CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
			result[0].ShouldBe("marketing");
			result[1].ShouldBe("transactional");
		}

		[Fact]
		public async Task GetAsync()
		{
			// Arrange
			var ipPoolName = "marketing";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, ipPoolName)).Respond("application/json", SINGLE_IPPOOL_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var ipPools = new IpPools(client);

			// Act
			var result = await ipPools.GetAsync(ipPoolName, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task UpdateAsync()
		{
			// Arrange
			var oldName = "Old Name";
			var newName = "New Name";

			var apiResponse = @"{
				'name': 'New Name'
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Put, Utils.GetSendGridApiUri(ENDPOINT, oldName)).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var ipPools = new IpPools(client);

			// Act
			await ipPools.UpdateAsync(oldName, newName, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task DeleteAsync()
		{
			// Arrange
			var name = "marketing";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, name)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var ipPools = new IpPools(client);

			// Act
			await ipPools.DeleteAsync(name, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task AddAdressAsync()
		{
			// Arrange
			var name = "test1";
			var address = "0.0.0.0";

			var apiResponse = @"{
			  'ip': '000.00.00.0',
			  'pools': [
				'test1'
			  ],
			  'start_date': 1409616000,
			  'warmup': true
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, name, "ips")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var ipPools = new IpPools(client);

			// Act
			await ipPools.AddAddressAsync(name, address, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task RemoveAdressAsync()
		{
			// Arrange
			var name = "test1";
			var address = "0.0.0.0";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, name, "ips", address)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var ipPools = new IpPools(client);

			// Act
			await ipPools.RemoveAddressAsync(name, address, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}
	}
}
