using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Resources;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
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
		private const string MULTIPLE_BATCHES_SINGLE_ITEM_JSON = @"[
			{
				'batch_id': 'BATCH_ID_1',
				'status': 'cancel'
			}
		]";
		private const string EMPTY_BATCHES_JSON = @"[
		]";

		#endregion

		[Fact]
		public async Task GenerateBatchIdAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_BATCH_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var batches = new Batches(client);

			// Act
			var batchId = await batches.GenerateBatchIdAsync().ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			string.IsNullOrEmpty(batchId).ShouldBeFalse();
		}

		[Fact]
		public async Task ValidateBatchIdAsync_true()
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
			var result = await batches.ValidateBatchIdAsync(batchId).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBeTrue();
		}

		[Fact]
		public async Task ValidateBatchIdAsync_false()
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
			var result = await batches.ValidateBatchIdAsync(batchId).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBeFalse();
		}

		[Fact]
		public async Task ValidateBatchIdAsync_problem()
		{
			// Arrange
			var batchId = "ABC123";

			var apiResponse = @"{
				'errors': [
					{
						'field': null,
						'message': 'an error has occured'
					}
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, batchId)).Respond(HttpStatusCode.BadRequest, "application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var batches = new Batches(client);

			// Act
			var result = await Should.ThrowAsync<Exception>(async () => await batches.ValidateBatchIdAsync(batchId).ConfigureAwait(false)).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();

			result.Message.ShouldBe("an error has occured");

		}

		[Fact]
		public async Task CancelAsync()
		{
			// Arrange
			var batchId = "YOUR_BATCH_ID";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri("user/scheduled_sends")).Respond("application/json", SINGLE_BATCH_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var batches = new Batches(client);

			// Act
			await batches.Cancel(batchId).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task PauseAsync()
		{
			// Arrange
			var batchId = "YOUR_BATCH_ID";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri("user/scheduled_sends")).Respond("application/json", SINGLE_BATCH_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var batches = new Batches(client);

			// Act
			await batches.Pause(batchId).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task GetAllAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("user/scheduled_sends")).Respond("application/json", MULTIPLE_BATCHES_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var batches = new Batches(client);

			// Act
			var result = await batches.GetAllAsync().ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public async Task GetAsync()
		{
			// Arrange
			var batchId = "YOUR_BATCH_ID";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("user/scheduled_sends", batchId)).Respond("application/json", MULTIPLE_BATCHES_SINGLE_ITEM_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var batches = new Batches(client);

			// Act
			var result = await batches.GetAsync(batchId).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetAsync_doesnt_exist()
		{
			// Arrange
			var batchId = "YOUR_BATCH_ID";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("user/scheduled_sends", batchId)).Respond("application/json", EMPTY_BATCHES_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var batches = new Batches(client);

			// Act
			var result = await batches.GetAsync(batchId).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBeNull();
		}

		[Fact]
		public async Task ResumeAsync()
		{
			// Arrange
			var batchId = "YOUR_BATCH_ID";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, batchId)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var batches = new Batches(client);

			// Act
			await batches.Resume(batchId).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}
	}
}
