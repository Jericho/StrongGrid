using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using StrongGrid.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class CampaignsTests
	{
		#region FIELDS

		private const string ENDPOINT = "/campaigns";

		private const string SINGLE_CAMPAIGN_JSON = @"{
			'id': 986724,
			'title': 'March Newsletter',
			'subject': 'New Products for Spring!',
			'sender_id': 124451,
			'list_ids': [
				110,
				124
			],
			'segment_ids': [
				110
			],
			'categories': [
				'spring line'
			],
			'suppression_group_id': 42,
			'custom_unsubscribe_url': '',
			'ip_pool': 'marketing',
			'html_content': '<html><head><title></title></head><body><p>Check out our spring line!</p></body></html>',
			'plain_content': 'Check out our spring line!',
			'status': 'Draft'
		}";
		private const string MULTIPLE_CAMPAIGNS_JSON = @"{
			'result': [
				{
					'id': 986724,
					'title': 'March Newsletter',
					'subject': 'New Products for Spring!',
					'sender_id': 124451,
					'list_ids': [
						110,
						124
					],
					'segment_ids': [
						110
					],
					'categories': [
						'spring line'
					],
					'suppression_group_id': 42,
					'custom_unsubscribe_url': '',
					'ip_pool': 'marketing',
					'html_content': '<html><head><title></title></head><body><p>Check out our spring line!</p></body></html>',
					'plain_content': 'Check out our spring line!',
					'status': 'Draft'
				},
				{
					'id': 986723,
					'title': 'February Newsletter',
					'subject': 'Final Winter Product Sale!',
					'sender_id': 124451,
					'list_ids': [
						110,
						124
					],
					'segment_ids': [
						110
					],
					'categories': [
						'winter line'
					],
					'suppression_group_id': 42,
					'custom_unsubscribe_url': '',
					'ip_pool': 'marketing',
					'html_content': '<html><head><title></title></head><body><p>Last call for winter clothes!</p></body></html>',
					'plain_content': 'Last call for winter clothes!',
					'status': 'Sent'
				}
			]
		}";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<Campaign>(SINGLE_CAMPAIGN_JSON);

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
		public void Create()
		{
			// Arrange
			var title = "March Newsletter";
			var suppressionGroupId = 42;
			var senderId = 124451;

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_CAMPAIGN_JSON) })
				.Verifiable();

			var campaigns = new Campaigns(mockClient.Object, ENDPOINT);

			// Act
			var result = campaigns.CreateAsync(title, suppressionGroupId, senderId, null, null, null, null, null, null, null, null, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetAll()
		{
			// Arrange
			var limit = 2;
			var offset = 0;

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}?limit={limit}&offset={offset}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_CAMPAIGNS_JSON) })
				.Verifiable();

			var campaigns = new Campaigns(mockClient.Object, ENDPOINT);

			// Act
			var result = campaigns.GetAllAsync(limit, offset, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public void Get()
		{
			// Arrange
			var campaignId = 986724;

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{campaignId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_CAMPAIGN_JSON) })
				.Verifiable();

			var campaigns = new Campaigns(mockClient.Object, ENDPOINT);

			// Act
			var result = campaigns.GetAsync(campaignId, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void Delete()
		{
			// Arrange
			var campaignId = 123;

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{campaignId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
				.Verifiable();

			var campaigns = new Campaigns(mockClient.Object, ENDPOINT);

			// Act
			campaigns.DeleteAsync(campaignId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void Update()
		{
			// Arrange
			var campaignId = 986724;
			var title = "March Newsletter";
			var suppressionGroupId = 42;
			var senderId = 124451;

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync($"{ENDPOINT}/{campaignId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_CAMPAIGN_JSON) })
				.Verifiable();

			var campaigns = new Campaigns(mockClient.Object, ENDPOINT);

			// Act
			var result = campaigns.UpdateAsync(campaignId, title, suppressionGroupId, senderId, null, null, null, null, null, null, null, null, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Id.ShouldBe(986724);
			result.Title.ShouldBe(title);
		}

		[Fact]
		public void SendNow()
		{
			// Arrange
			var campaignId = 986724;

			var apiResponse = @"{
				'id': 986724,
				'status': 'Scheduled'
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/{campaignId}/schedules/now", (JObject)null, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var campaigns = new Campaigns(mockClient.Object, ENDPOINT);

			// Act
			campaigns.SendNowAsync(campaignId, CancellationToken.None).Wait();

			// Assert
		}

		[Fact]
		public void Schedule()
		{
			// Arrange
			var campaignId = 986724;
			var sendOn = DateTime.UtcNow.AddHours(5);

			var apiResponse = @"{
				'id': 986724,
				'send_at': 1489771528,
				'status': 'Scheduled'
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/{campaignId}/schedules", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var campaigns = new Campaigns(mockClient.Object, ENDPOINT);

			// Act
			campaigns.ScheduleAsync(campaignId, sendOn, CancellationToken.None).Wait();

			// Assert
		}

		[Fact]
		public void Reschedule()
		{
			// Arrange
			var campaignId = 986724;
			var sendOn = DateTime.UtcNow.AddHours(15);

			var apiResponse = @"{
				'id': 986724,
				'send_at': 1489451436,
				'status': 'Scheduled'
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync($"{ENDPOINT}/{campaignId}/schedules", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var campaigns = new Campaigns(mockClient.Object, ENDPOINT);

			// Act
			campaigns.RescheduleAsync(campaignId, sendOn, CancellationToken.None).Wait();

			// Assert
		}

		[Fact]
		public void GetScheduledDate()
		{
			// Arrange
			var campaignId = 986724;
			var apiResponse = @"{
				'send_at': 1489771528
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{campaignId}/schedules", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var campaigns = new Campaigns(mockClient.Object, ENDPOINT);

			// Act
			var result = campaigns.GetScheduledDateAsync(campaignId, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.HasValue.ShouldBeTrue();
		}

		[Fact]
		public void Unschedule()
		{
			// Arrange
			var campaignId = 986724;

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{campaignId}/schedules", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var campaigns = new Campaigns(mockClient.Object, ENDPOINT);

			// Act
			campaigns.UnscheduleAsync(campaignId, CancellationToken.None).Wait();

			// Assert
		}

		[Fact]
		public void SendTest_single()
		{
			// Arrange
			var campaignId = 986724;
			var emailAddresses = new[] { "test1@example.com" };

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/{campaignId}/schedules/test", It.Is<JObject>(o => o["to"].ToString() == emailAddresses[0]), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var campaigns = new Campaigns(mockClient.Object, ENDPOINT);

			// Act
			campaigns.SendTestAsync(campaignId, emailAddresses, CancellationToken.None).Wait();

			// Assert
		}

		[Fact]
		public void SendTest_multiple()
		{
			// Arrange
			var campaignId = 986724;
			var emailAddresses = new[] { "test1@example.com", "test2@exmaple.com" };

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/{campaignId}/schedules/test", It.Is<JObject>(o => o["to"].ToObject<JArray>().Count == emailAddresses.Length), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var campaigns = new Campaigns(mockClient.Object, ENDPOINT);

			// Act
			campaigns.SendTestAsync(campaignId, emailAddresses, CancellationToken.None).Wait();

			// Assert
		}
	}
}
