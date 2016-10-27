using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class BatchesTests
	{
		private const string ENDPOINT = "/mail/batch";

		[TestMethod]
		public void GenerateBatchId()
		{
			// Arrange
			var apiResponse = @"{
				'batch_id': 'qwerty'
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync(ENDPOINT, (JObject)null, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var batches = new Batches(mockClient.Object);

			// Act
			var batchId = batches.GenerateBatchIdAsync().Result;

			// Assert
			Assert.IsFalse(string.IsNullOrEmpty(batchId));
			Assert.AreEqual("qwerty", batchId);
		}

		[TestMethod]
		public void Cancel()
		{
			// Arrange
			var batchId = "YOUR_BATCH_ID";

			var apiResponse = @"{
				'batch_id': 'YOUR_BATCH_ID',
				'status': 'cancel'
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync("/user/scheduled_sends", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var batches = new Batches(mockClient.Object);

			// Act
			batches.Cancel(batchId).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Pause()
		{
			// Arrange
			var batchId = "YOUR_BATCH_ID";

			var apiResponse = @"{
				'batch_id': 'YOUR_BATCH_ID',
				'status': 'pause'
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync("/user/scheduled_sends", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var batches = new Batches(mockClient.Object);

			// Act
			batches.Cancel(batchId).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			var apiResponse = @"[
				{
					'batch_id': 'BATCH_ID_1',
					'status': 'cancel'
				},
				{
					'batch_id': 'BATCH_ID_2',
					'status': 'pause'
				}
			]";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync("/user/scheduled_sends", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var batches = new Batches(mockClient.Object);

			// Act
			var result = batches.GetAllAsync().Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual(BatchStatus.Canceled, result[0].Status);
			Assert.AreEqual(BatchStatus.Paused, result[1].Status);
		}

		[TestMethod]
		public void Resume()
		{
			// Arrange
			var batchId = "YOUR_BATCH_ID";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync($"{ENDPOINT}/{batchId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var batches = new Batches(mockClient.Object);

			// Act
			batches.Resume(batchId).Wait(CancellationToken.None);

			// Assert
		}
	}
}
