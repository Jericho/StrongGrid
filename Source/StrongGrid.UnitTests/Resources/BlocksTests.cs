using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class BlocksTests
	{
		#region FIELDS

		private const string ENDPOINT = "/suppression/blocks";
		private MockRepository _mockRepository;
		private Mock<IClient> _mockClient;

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

		private Blocks CreateBlocks()
		{
			return new Blocks(_mockClient.Object, ENDPOINT);

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
			var result = JsonConvert.DeserializeObject<Block>(SINGLE_BLOCK_JSON);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(new System.DateTime(2015, 9, 30, 22, 12, 34, 0, System.DateTimeKind.Utc), result.CreatedOn);
			Assert.AreEqual("user1@example.com", result.Email);
			Assert.AreEqual("error dialing remote address: dial tcp 10.57.152.165:25: no route to host", result.Reason);
			Assert.AreEqual("4.0.0", result.Status);
		}

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}?start_time=&end_time=&limit=25&offset=0", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_BLOCKS_JSON) })
				.Verifiable();

			var blocks = CreateBlocks();

			// Act
			var result = blocks.GetAllAsync().Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("user1@example.com", result[0].Email);
		}

		[TestMethod]
		public void DeleteAll()
		{
			// Arrange
			_mockClient
				.Setup(c => c.DeleteAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var blocks = CreateBlocks();

			// Act
			blocks.DeleteAllAsync().Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void DeleteMultiple()
		{
			// Arrange
			var emailAddresses = new[] { "email1@test.com", "email2@test.com" };

			_mockClient
				.Setup(c => c.DeleteAsync(ENDPOINT, It.Is<JObject>(o => o["emails"].ToObject<string[]>().Length == emailAddresses.Length), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var blocks = CreateBlocks();

			// Act
			blocks.DeleteMultipleAsync(emailAddresses).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			var emailAddress = "spam1@test.com";

			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{emailAddress}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var blocks = CreateBlocks();

			// Act
			blocks.DeleteAsync(emailAddress).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var emailAddress = "user1@example.com";

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{emailAddress}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_BLOCK_JSON) })
				.Verifiable();

			var blocks = CreateBlocks();

			// Act
			var result = blocks.GetAsync(emailAddress, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(emailAddress, result.Email);
		}
	}
}
