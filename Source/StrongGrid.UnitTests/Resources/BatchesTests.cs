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
		internal const string ENDPOINT = "mail/batch";

		internal const string SINGLE_BATCH_JSON = @"{
			""batch_id"": ""BATCH_ID_1"",
			""status"": ""cancel""
		}";
		internal const string MULTIPLE_BATCHES_JSON = @"[
			{
				""batch_id"": ""BATCH_ID_1"",
				""status"": ""cancel""
			},
			{
				""batch_id"": ""BATCH_ID_2"",
				""status"": ""pause""
			}
		]";
		internal const string MULTIPLE_BATCHES_SINGLE_ITEM_JSON = @"[
			{
				""batch_id"": ""BATCH_ID_1"",
				""status"": ""cancel""
			}
		]";
		internal const string EMPTY_BATCHES_JSON = @"[
		]";

		private readonly ITestOutputHelper _outputHelper;

		public BatchesTests(ITestOutputHelper outputHelper)
		{
			_outputHelper = outputHelper;
		}

		[Fact]
		public async Task GenerateBatchIdAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_BATCH_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var batches = new Batches(client);

			// Act
			var batchId = await batches.GenerateBatchIdAsync(TestContext.Current.CancellationToken);

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
				""batch_id"": ""ABC123""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, batchId)).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var batches = new Batches(client);

			// Act
			var result = await batches.ValidateBatchIdAsync(batchId, TestContext.Current.CancellationToken);

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
				""errors"": [
					{
						""field"": null,
						""message"": ""invalid batch id""
					}
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, batchId)).Respond(HttpStatusCode.BadRequest, "application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var batches = new Batches(client);

			// Act
			var result = await batches.ValidateBatchIdAsync(batchId, TestContext.Current.CancellationToken);

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
				""errors"": [
					{
						""field"": null,
						""message"": ""an error has occurred""
					}
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, batchId)).Respond(HttpStatusCode.BadRequest, "application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var batches = new Batches(client);

			// Act
			var result = await Should.ThrowAsync<Exception>(batches.ValidateBatchIdAsync(batchId, TestContext.Current.CancellationToken));

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();

			result.Message.ShouldBe("an error has occurred");
		}

		[Fact]
		public async Task CancelAsync()
		{
			// Arrange
			var batchId = "YOUR_BATCH_ID";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri("user/scheduled_sends")).Respond("application/json", SINGLE_BATCH_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var batches = new Batches(client);

			// Act
			await batches.Cancel(batchId, TestContext.Current.CancellationToken);

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

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var batches = new Batches(client);

			// Act
			await batches.Pause(batchId, TestContext.Current.CancellationToken);

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

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var batches = new Batches(client);

			// Act
			var result = await batches.GetAllAsync(TestContext.Current.CancellationToken);

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

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var batches = new Batches(client);

			// Act
			var result = await batches.GetAsync(batchId, TestContext.Current.CancellationToken);

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

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var batches = new Batches(client);

			// Act
			var result = await batches.GetAsync(batchId, TestContext.Current.CancellationToken);

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

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var batches = new Batches(client);

			// Act
			await batches.Resume(batchId, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}
	}
}
