using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.UnitTests;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class BatchesTests
	{
		#region FIELDS

		private const string BASE_URI = "https://api.sendgrid.com";
		private const string ENDPOINT = "mail/batch";

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
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_BATCH_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var batches = new Batches(client);

			// Act
			var batchId = batches.GenerateBatchIdAsync().Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, batchId)).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var batches = new Batches(client);

			// Act
			var result = batches.ValidateBatchIdAsync(batchId).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, batchId)).Respond(HttpStatusCode.BadRequest, "application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var batches = new Batches(client);

			// Act
			var result = batches.ValidateBatchIdAsync(batchId).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBeFalse();
		}

		[Fact]
		public void Cancel()
		{
			// Arrange
			var batchId = "YOUR_BATCH_ID";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri("user/scheduled_sends")).Respond("application/json", SINGLE_BATCH_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var batches = new Batches(client);

			// Act
			batches.Cancel(batchId).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void Pause()
		{
			// Arrange
			var batchId = "YOUR_BATCH_ID";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri("user/scheduled_sends")).Respond("application/json", SINGLE_BATCH_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var batches = new Batches(client);

			// Act
			batches.Pause(batchId).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void GetAll()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("user/scheduled_sends")).Respond("application/json", MULTIPLE_BATCHES_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var batches = new Batches(client);

			// Act
			var result = batches.GetAllAsync().Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public void Resume()
		{
			// Arrange
			var batchId = "YOUR_BATCH_ID";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, batchId)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var batches = new Batches(client);

			// Act
			batches.Resume(batchId).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}
	}
}
