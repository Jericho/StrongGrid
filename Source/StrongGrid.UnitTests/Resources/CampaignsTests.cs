using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class CampaignsTests
	{
		private const string ENDPOINT = "/campaigns";

		[TestMethod]
		public void Create()
		{
			// Arrange
			var title = "March Newsletter";
			var suppressionGroupId = 42;
			var senderId = 124451;

			var apiResponse = @"{
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
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var campaigns = new Campaigns(mockClient.Object);

			// Act
			var result = campaigns.CreateAsync(title, suppressionGroupId, senderId, null, null, null, null, null, null, null, null, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(986724, result.Id);
			Assert.AreEqual(title, result.Title);
		}

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			var limit = 2;
			var offset = 0;

			var apiResponse = @"{
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

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync($"{ENDPOINT}?limit={limit}&offset={offset}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var campaigns = new Campaigns(mockClient.Object);

			// Act
			var result = campaigns.GetAllAsync(limit, offset, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual("March Newsletter", result[0].Title);
			Assert.AreEqual("February Newsletter", result[1].Title);
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var campaignId = 986724;
			var apiResponse = @"{
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

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync($"{ENDPOINT}/{campaignId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var campaigns = new Campaigns(mockClient.Object);

			// Act
			var result = campaigns.GetAsync(campaignId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(campaignId, result.Id);
			Assert.AreEqual("New Products for Spring!", result.Subject);
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			var campaignId = 123;

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync($"{ENDPOINT}/{campaignId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var campaigns = new Campaigns(mockClient.Object);

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

			var apiResponse = @"{
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
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PatchAsync($"{ENDPOINT}/{campaignId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var campaigns = new Campaigns(mockClient.Object);

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
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync($"{ENDPOINT}/{campaignId}/schedules/now", (JObject)null, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var campaigns = new Campaigns(mockClient.Object);

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
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync($"{ENDPOINT}/{campaignId}/schedules", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var campaigns = new Campaigns(mockClient.Object);

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
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PatchAsync($"{ENDPOINT}/{campaignId}/schedules", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var campaigns = new Campaigns(mockClient.Object);

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

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync($"{ENDPOINT}/{campaignId}/schedules", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var campaigns = new Campaigns(mockClient.Object);

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

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync($"{ENDPOINT}/{campaignId}/schedules", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var campaigns = new Campaigns(mockClient.Object);

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

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync($"{ENDPOINT}/{campaignId}/schedules/test", It.Is<JObject>(o => o["to"].ToString() == emailAddresses[0]), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var campaigns = new Campaigns(mockClient.Object);

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

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync($"{ENDPOINT}/{campaignId}/schedules/test", It.Is<JObject>(o => o["to"].ToObject<JArray>().Count == emailAddresses.Length), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var campaigns = new Campaigns(mockClient.Object);

			// Act
			campaigns.SendTestAsync(campaignId, emailAddresses, CancellationToken.None).Wait();

			// Assert
		}
	}
}
