using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Json;
using StrongGrid.Models.Legacy;
using StrongGrid.Resources.Legacy;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class CampaignsTests
	{
		internal const string ENDPOINT = "campaigns";

		internal const string SINGLE_CAMPAIGN_JSON = @"{
			""id"": 986724,
			""title"": ""March Newsletter"",
			""subject"": ""New Products for Spring!"",
			""sender_id"": 124451,
			""list_ids"": [
				110,
				124
			],
			""segment_ids"": [
				110
			],
			""categories"": [
				""spring line""
			],
			""suppression_group_id"": 42,
			""custom_unsubscribe_url"": """",
			""ip_pool"": ""marketing"",
			""html_content"": ""<html><head><title></title></head><body><p>Check out our spring line!</p></body></html>"",
			""plain_content"": ""Check out our spring line!"",
			""status"": ""Draft""
		}";
		internal const string MULTIPLE_CAMPAIGNS_JSON = @"{
			""result"": [
				{
					""id"": 986724,
					""title"": ""March Newsletter"",
					""subject"": ""New Products for Spring!"",
					""sender_id"": 124451,
					""list_ids"": [
						110,
						124
					],
					""segment_ids"": [
						110
					],
					""categories"": [
						""spring line""
					],
					""suppression_group_id"": 42,
					""custom_unsubscribe_url"": """",
					""ip_pool"": ""marketing"",
					""html_content"": ""<html><head><title></title></head><body><p>Check out our spring line!</p></body></html>"",
					""plain_content"": ""Check out our spring line!"",
					""status"": ""Draft""
				},
				{
					""id"": 986723,
					""title"": ""February Newsletter"",
					""subject"": ""Final Winter Product Sale!"",
					""sender_id"": 124451,
					""list_ids"": [
						110,
						124
					],
					""segment_ids"": [
						110
					],
					""categories"": [
						""winter line""
					],
					""suppression_group_id"": 42,
					""custom_unsubscribe_url"": """",
					""ip_pool"": ""marketing"",
					""html_content"": ""<html><head><title></title></head><body><p>Last call for winter clothes!</p></body></html>"",
					""plain_content"": ""Last call for winter clothes!"",
					""status"": ""Sent""
				}
			]
		}";

		private readonly ITestOutputHelper _outputHelper;

		public CampaignsTests(ITestOutputHelper outputHelper)
		{
			_outputHelper = outputHelper;
		}

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<Campaign>(SINGLE_CAMPAIGN_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.Categories.ShouldBe(new[] { "spring line" });
			result.CustomUnsubscribeUrl.ShouldBe("");
			result.HtmlContent.ShouldBe("<html><head><title></title></head><body><p>Check out our spring line!</p></body></html>");
			result.Id.ShouldBe(986724);
			result.IpPool.ShouldBe("marketing");
			result.Lists.ShouldBe(new[] { 110L, 124L });
			result.Segments.ShouldBe(new[] { 110L });
			result.SenderId.ShouldBe(124451);
			result.Status.ShouldBe(CampaignStatus.Draft);
			result.Subject.ShouldBe("New Products for Spring!");
			result.SuppressionGroupId.ShouldBe(42);
			result.TextContent.ShouldBe("Check out our spring line!");
			result.Title.ShouldBe("March Newsletter");
		}

		[Fact]
		public async Task CreateAsync()
		{
			// Arrange
			var title = "March Newsletter";
			var suppressionGroupId = 42;
			var senderId = 124451;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_CAMPAIGN_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var campaigns = new Campaigns(client);

			// Act
			var result = await campaigns.CreateAsync(title, senderId, null, null, null, null, null, null, suppressionGroupId, null, null, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetAllAsync()
		{
			// Arrange
			var limit = 2;
			var offset = 0;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT) + $"?limit={limit}&offset={offset}").Respond("application/json", MULTIPLE_CAMPAIGNS_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var campaigns = new Campaigns(client);

			// Act
			var result = await campaigns.GetAllAsync(limit, offset, TestContext.Current.CancellationToken);

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
			var campaignId = 986724;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, campaignId)).Respond("application/json", SINGLE_CAMPAIGN_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var campaigns = new Campaigns(client);

			// Act
			var result = await campaigns.GetAsync(campaignId, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task DeleteAsync()
		{
			// Arrange
			var campaignId = 123;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, campaignId)).Respond(HttpStatusCode.OK);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var campaigns = new Campaigns(client);

			// Act
			await campaigns.DeleteAsync(campaignId, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task UpdateAsync()
		{
			// Arrange
			var campaignId = 986724;
			var title = "March Newsletter";
			var suppressionGroupId = 42;
			var senderId = 124451;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT, campaignId)).Respond("application/json", SINGLE_CAMPAIGN_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var campaigns = new Campaigns(client);

			// Act
			var result = await campaigns.UpdateAsync(campaignId, title, suppressionGroupId, null, null, null, null, null, null, senderId, null, null, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Id.ShouldBe(986724);
			result.Title.ShouldBe(title);
		}

		[Fact]
		public async Task SendNowAsync()
		{
			// Arrange
			var campaignId = 986724;

			var apiResponse = @"{
				""id"": 986724,
				""status"": ""Scheduled""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, campaignId, "schedules/now")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var campaigns = new Campaigns(client);

			// Act
			await campaigns.SendNowAsync(campaignId, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task ScheduleAsync()
		{
			// Arrange
			var campaignId = 986724;
			var sendOn = DateTime.UtcNow.AddHours(5);

			var apiResponse = @"{
				""id"": 986724,
				""send_at"": 1489771528,
				""status"": ""Scheduled""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, campaignId, "schedules")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var campaigns = new Campaigns(client);

			// Act
			await campaigns.ScheduleAsync(campaignId, sendOn, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task RescheduleAsync()
		{
			// Arrange
			var campaignId = 986724;
			var sendOn = DateTime.UtcNow.AddHours(15);

			var apiResponse = @"{
				""id"": 986724,
				""send_at"": 1489451436,
				""status"": ""Scheduled""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT, campaignId, "schedules")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var campaigns = new Campaigns(client);

			// Act
			await campaigns.RescheduleAsync(campaignId, sendOn, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task GetScheduledDateAsync()
		{
			// Arrange
			var campaignId = 986724;
			var apiResponse = @"{
				""send_at"": 1489771528
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, campaignId, "schedules")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var campaigns = new Campaigns(client);

			// Act
			var result = await campaigns.GetScheduledDateAsync(campaignId, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.HasValue.ShouldBeTrue();
		}

		[Fact]
		public async Task UnscheduleAsync()
		{
			// Arrange
			var campaignId = 986724;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, campaignId, "schedules")).Respond(HttpStatusCode.NoContent);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var campaigns = new Campaigns(client);

			// Act
			await campaigns.UnscheduleAsync(campaignId, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task SendTestAsync_single()
		{
			// Arrange
			var campaignId = 986724;
			var emailAddresses = new[] { "test1@example.com" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, campaignId, "schedules/test")).Respond(HttpStatusCode.NoContent);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var campaigns = new Campaigns(client);

			// Act
			await campaigns.SendTestAsync(campaignId, emailAddresses, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task SendTestAsync_multiple()
		{
			// Arrange
			var campaignId = 986724;
			var emailAddresses = new[] { "test1@example.com", "test2@example.com" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, campaignId, "schedules/test")).Respond(HttpStatusCode.NoContent);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var campaigns = new Campaigns(client);

			// Act
			await campaigns.SendTestAsync(campaignId, emailAddresses, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}
	}
}
