using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.UnitTests;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class IpPoolsTests
	{
		#region FIELDS

		private const string ENDPOINT = "ips/pools";

		private const string SINGLE_IPPOOL_JSON = @"{
			'name': 'marketing',
			'ips': ['1.1.1.1','2.2.2.2','3.3.3.3']
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
			result.IpAddresses[0].ShouldBe("1.1.1.1");
			result.IpAddresses[1].ShouldBe("2.2.2.2");
			result.IpAddresses[2].ShouldBe("3.3.3.3");
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
			result.Name.ShouldBe(name);
		}

		[Fact]
		public async Task GetAllAsync()
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
			var result = await ipPools.GetAllAsync(CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
			result[0].Name.ShouldBe("marketing");
			result[1].Name.ShouldBe("transactional");
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
