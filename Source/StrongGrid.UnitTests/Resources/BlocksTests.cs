using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using StrongGrid.Model;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class BlocksTests
	{
		#region FIELDS

		private const string ENDPOINT = "/suppression/blocks";

		private const string SINGLE_BLOCK_JSON = @"{
			'created': 1443651154,
			'email': 'user1@example.com',
			'reason': 'error dialing remote address: dial tcp 10.57.152.165:25: no route to host',
			'status': '4.0.0'
		}";
		private const string MULTIPLE_BLOCKS_JSON = @"[
			{
				'created': 1443651154,
				'email': 'user1@example.com',
				'reason': 'error dialing remote address: dial tcp 10.57.152.165:25: no route to host',
				'status': '4.0.0'
			}
		]";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<Block>(SINGLE_BLOCK_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.CreatedOn.ShouldBe(new System.DateTime(2015, 9, 30, 22, 12, 34, 0, System.DateTimeKind.Utc));
			result.Email.ShouldBe("user1@example.com");
			result.Reason.ShouldBe("error dialing remote address: dial tcp 10.57.152.165:25: no route to host");
			result.Status.ShouldBe("4.0.0");
		}

		[Fact]
		public void GetAll()
		{
			// Arrange
			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}?start_time=&end_time=&limit=25&offset=0", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_BLOCKS_JSON) })
				.Verifiable();

			var blocks = new Blocks(mockClient.Object, ENDPOINT);

			// Act
			var result = blocks.GetAllAsync().Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].Email.ShouldBe("user1@example.com");
		}

		[Fact]
		public void DeleteAll()
		{
			// Arrange
			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var blocks = new Blocks(mockClient.Object, ENDPOINT);

			// Act
			blocks.DeleteAllAsync().Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void DeleteMultiple()
		{
			// Arrange
			var emailAddresses = new[] { "email1@test.com", "email2@test.com" };

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync(ENDPOINT, It.Is<JObject>(o => o["emails"].ToObject<string[]>().Length == emailAddresses.Length), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var blocks = new Blocks(mockClient.Object, ENDPOINT);

			// Act
			blocks.DeleteMultipleAsync(emailAddresses).Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void Delete()
		{
			// Arrange
			var emailAddress = "spam1@test.com";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{emailAddress}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var blocks = new Blocks(mockClient.Object, ENDPOINT);

			// Act
			blocks.DeleteAsync(emailAddress).Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void Get()
		{
			// Arrange
			var emailAddress = "user1@example.com";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{emailAddress}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_BLOCK_JSON) })
				.Verifiable();

			var blocks = new Blocks(mockClient.Object, ENDPOINT);

			// Act
			var result = blocks.GetAsync(emailAddress, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Email.ShouldBe(emailAddress);
		}
	}
}
