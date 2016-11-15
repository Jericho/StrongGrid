using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class BatchesTests
	{
		#region FIELDS

		private const string ENDPOINT = "/mail/batch";
		private MockRepository _mockRepository;
		private Mock<IClient> _mockClient;

		private const string SINGLE_BATCH_JSON = @"{
			'batch_id': 'BATCH_ID_1',
			'status': 'cancel'
		}";
		private const string MULTIPLE_BATCHES_JSON = @"[
			{
				'batch_id': 'BATCH_ID_1',
				'status': 'cancel'
			},
			{
				'batch_id': 'BATCH_ID_2',
				'status': 'pause'
			}
		]";

		#endregion

		private Batches CreateBatches()
		{
			return new Batches(_mockClient.Object, ENDPOINT);

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
		public void GenerateBatchId()
		{
			// Arrange
			_mockClient
				.Setup(c => c.PostAsync(ENDPOINT, (JObject)null, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_BATCH_JSON) })
				.Verifiable();

			var batches = CreateBatches();

			// Act
			var batchId = batches.GenerateBatchIdAsync().Result;

			// Assert
			Assert.IsFalse(string.IsNullOrEmpty(batchId));
		}

		[TestMethod]
		public void ValidateBatchId_true()
		{
			// Arrange
			var batchId = "ABC123";

			var apiResponse = @"{
				'batch_id': 'ABC123'
			}";

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{batchId}", It.IsAny<CancellationToken>()))
					.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
					.Verifiable();

			var batches = CreateBatches();

			// Act
			var result = batches.ValidateBatchIdAsync(batchId).Result;

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void ValidateBatchId_false()
		{
			// Arrange
			var batchId = "ABC123";

			var apiResponse = @"{
				'errors': [
					{
						'field': null,
						'message': 'invalid batch id'
					}
				]
			}";

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{batchId}", It.IsAny<CancellationToken>()))
					.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
					.Verifiable();

			var batches = CreateBatches();

			// Act
			var result = batches.ValidateBatchIdAsync(batchId).Result;

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void Cancel()
		{
			// Arrange
			var batchId = "YOUR_BATCH_ID";

			_mockClient
				.Setup(c => c.PostAsync("/user/scheduled_sends", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_BATCH_JSON) })
				.Verifiable();

			var batches = CreateBatches();

			// Act
			batches.Cancel(batchId).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Pause()
		{
			// Arrange
			var batchId = "YOUR_BATCH_ID";

			_mockClient
				.Setup(c => c.PostAsync("/user/scheduled_sends", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_BATCH_JSON) })
				.Verifiable();

			var batches = CreateBatches();

			// Act
			batches.Pause(batchId).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			_mockClient
				.Setup(c => c.GetAsync("/user/scheduled_sends", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_BATCHES_JSON) })
				.Verifiable();

			var batches = CreateBatches();

			// Act
			var result = batches.GetAllAsync().Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
		}

		[TestMethod]
		public void Resume()
		{
			// Arrange
			var batchId = "YOUR_BATCH_ID";

			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{batchId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var batches = CreateBatches();

			// Act
			batches.Resume(batchId).Wait(CancellationToken.None);

			// Assert
		}
	}
}
