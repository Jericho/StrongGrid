using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class CampaignsTests
	{
		#region FIELDS

		private const string ENDPOINT = "/campaigns";
		private MockRepository _mockRepository;
		private Mock<IClient> _mockClient;

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

		private Campaigns CreateCampaigns()
		{
			return new Campaigns(_mockClient.Object, ENDPOINT);

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
			var result = JsonConvert.DeserializeObject<Campaign>(SINGLE_CAMPAIGN_JSON);

			// Assert
			Assert.IsNotNull(result);
			CollectionAssert.AreEqual(new[] { "spring line" }, result.Categories);
			Assert.AreEqual("", result.CustomUnsubscribeUrl);
			Assert.AreEqual("<html><head><title></title></head><body><p>Check out our spring line!</p></body></html>", result.HtmlContent);
			Assert.AreEqual(986724, result.Id);
			Assert.AreEqual("marketing", result.IpPool);
			CollectionAssert.AreEqual(new[] { 110L, 124L }, result.Lists);
			CollectionAssert.AreEqual(new[] { 110L }, result.Segments);
			Assert.AreEqual(124451, result.SenderId);
			Assert.AreEqual(CampaignStatus.Draft, result.Status);
			Assert.AreEqual("New Products for Spring!", result.Subject);
			Assert.AreEqual(42, result.SuppressionGroupId);
			Assert.AreEqual("Check out our spring line!", result.TextContent);
			Assert.AreEqual("March Newsletter", result.Title);
		}

		[TestMethod]
		public void Create()
		{
			// Arrange
			var title = "March Newsletter";
			var suppressionGroupId = 42;
			var senderId = 124451;

			_mockClient
				.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_CAMPAIGN_JSON) })
				.Verifiable();

			var campaigns = CreateCampaigns();

			// Act
			var result = campaigns.CreateAsync(title, suppressionGroupId, senderId, null, null, null, null, null, null, null, null, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			var limit = 2;
			var offset = 0;

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}?limit={limit}&offset={offset}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_CAMPAIGNS_JSON) })
				.Verifiable();

			var campaigns = CreateCampaigns();

			// Act
			var result = campaigns.GetAllAsync(limit, offset, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var campaignId = 986724;

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{campaignId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_CAMPAIGN_JSON) })
				.Verifiable();

			var campaigns = CreateCampaigns();

			// Act
			var result = campaigns.GetAsync(campaignId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			var campaignId = 123;

			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{campaignId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
				.Verifiable();

			var campaigns = CreateCampaigns();

			// Act
			campaigns.DeleteAsync(campaignId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Update()
		{
			// Arrange
			var campaignId = 986724;
			var title = "March Newsletter";
			var suppressionGroupId = 42;
			var senderId = 124451;

			_mockClient
				.Setup(c => c.PatchAsync($"{ENDPOINT}/{campaignId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_CAMPAIGN_JSON) })
				.Verifiable();

			var campaigns = CreateCampaigns();

			// Act
			var result = campaigns.UpdateAsync(campaignId, title, suppressionGroupId, senderId, null, null, null, null, null, null, null, null, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(986724, result.Id);
			Assert.AreEqual(title, result.Title);
		}

		[TestMethod]
		public void SendNow()
		{
			// Arrange
			var campaignId = 986724;

			var apiResponse = @"{
				'id': 986724,
				'status': 'Scheduled'
			}";
			_mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/{campaignId}/schedules/now", (JObject)null, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var campaigns = CreateCampaigns();

			// Act
			campaigns.SendNowAsync(campaignId, CancellationToken.None).Wait();

			// Assert
		}

		[TestMethod]
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
			_mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/{campaignId}/schedules", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var campaigns = CreateCampaigns();

			// Act
			campaigns.ScheduleAsync(campaignId, sendOn, CancellationToken.None).Wait();

			// Assert
		}

		[TestMethod]
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
			_mockClient
				.Setup(c => c.PatchAsync($"{ENDPOINT}/{campaignId}/schedules", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var campaigns = CreateCampaigns();

			// Act
			campaigns.RescheduleAsync(campaignId, sendOn, CancellationToken.None).Wait();

			// Assert
		}

		[TestMethod]
		public void GetScheduledDate()
		{
			// Arrange
			var campaignId = 986724;
			var apiResponse = @"{
				'send_at': 1489771528
			}";

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{campaignId}/schedules", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var campaigns = CreateCampaigns();

			// Act
			var result = campaigns.GetScheduledDateAsync(campaignId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.IsTrue(result.HasValue);
		}

		[TestMethod]
		public void Unschedule()
		{
			// Arrange
			var campaignId = 986724;

			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{campaignId}/schedules", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var campaigns = CreateCampaigns();

			// Act
			campaigns.UnscheduleAsync(campaignId, CancellationToken.None).Wait();

			// Assert
		}

		[TestMethod]
		public void SendTest_single()
		{
			// Arrange
			var campaignId = 986724;
			var emailAddresses = new[] { "test1@example.com" };

			_mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/{campaignId}/schedules/test", It.Is<JObject>(o => o["to"].ToString() == emailAddresses[0]), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var campaigns = CreateCampaigns();

			// Act
			campaigns.SendTestAsync(campaignId, emailAddresses, CancellationToken.None).Wait();

			// Assert
		}

		[TestMethod]
		public void SendTest_multiple()
		{
			// Arrange
			var campaignId = 986724;
			var emailAddresses = new[] { "test1@example.com", "test2@exmaple.com" };

			_mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/{campaignId}/schedules/test", It.Is<JObject>(o => o["to"].ToObject<JArray>().Count == emailAddresses.Length), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var campaigns = CreateCampaigns();

			// Act
			campaigns.SendTestAsync(campaignId, emailAddresses, CancellationToken.None).Wait();

			// Assert
		}
	}
}
