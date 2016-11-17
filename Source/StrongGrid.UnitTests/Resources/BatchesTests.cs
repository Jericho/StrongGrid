using Moq;
using Newtonsoft.Json.Linq;
using Shouldly;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class BatchesTests
	{
		#region FIELDS

		private const string ENDPOINT = "/mail/batch";

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

		[Fact]
		public void GenerateBatchId()
		{
			// Arrange
			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync(ENDPOINT, (JObject)null, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_BATCH_JSON) })
				.Verifiable();

			var batches = new Batches(mockClient.Object, ENDPOINT);

			// Act
			var batchId = batches.GenerateBatchIdAsync().Result;

			// Assert
			string.IsNullOrEmpty(batchId).ShouldBeFalse();
		}

		[Fact]
		public void ValidateBatchId_true()
		{
			// Arrange
			var batchId = "ABC123";

			var apiResponse = @"{
				'batch_id': 'ABC123'
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{batchId}", It.IsAny<CancellationToken>()))
					.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
					.Verifiable();

			var batches = new Batches(mockClient.Object, ENDPOINT);

			// Act
			var result = batches.ValidateBatchIdAsync(batchId).Result;

			// Assert
			result.ShouldBeTrue();
		}

		[Fact]
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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{batchId}", It.IsAny<CancellationToken>()))
					.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
					.Verifiable();

			var batches = new Batches(mockClient.Object, ENDPOINT);

			// Act
			var result = batches.ValidateBatchIdAsync(batchId).Result;

			// Assert
			result.ShouldBeFalse();
		}

		[Fact]
		public void Cancel()
		{
			// Arrange
			var batchId = "YOUR_BATCH_ID";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync("/user/scheduled_sends", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_BATCH_JSON) })
				.Verifiable();

			var batches = new Batches(mockClient.Object, ENDPOINT);

			// Act
			batches.Cancel(batchId).Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void Pause()
		{
			// Arrange
			var batchId = "YOUR_BATCH_ID";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync("/user/scheduled_sends", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_BATCH_JSON) })
				.Verifiable();

			var batches = new Batches(mockClient.Object, ENDPOINT);

			// Act
			batches.Pause(batchId).Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void GetAll()
		{
			// Arrange
			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync("/user/scheduled_sends", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_BATCHES_JSON) })
				.Verifiable();

			var batches = new Batches(mockClient.Object, ENDPOINT);

			// Act
			var result = batches.GetAllAsync().Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public void Resume()
		{
			// Arrange
			var batchId = "YOUR_BATCH_ID";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{batchId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var batches = new Batches(mockClient.Object, ENDPOINT);

			// Act
			batches.Resume(batchId).Wait(CancellationToken.None);

			// Assert
		}
	}
}
