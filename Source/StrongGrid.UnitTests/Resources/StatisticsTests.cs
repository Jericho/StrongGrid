using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrongGrid.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class StatisticsTests
	{
		[TestMethod]
		public void GetGlobalStats()
		{
			// Arrange
			var startDate = new DateTime(2015, 1, 1);
			var endDate = new DateTime(2015, 1, 2);
			var apiResponse = @"[
				{
					'date': '2015 - 01 - 01',
					'stats': [
						{
						'metrics': {
							'blocks': 1,
							'bounce_drops': 0,
							'bounces': 0,
							'clicks': 0,
							'deferred': 1,
							'delivered': 1,
							'invalid_emails': 1,
							'opens': 1,
							'processed': 2,
							'requests': 3,
							'spam_report_drops': 0,
							'spam_reports': 0,
							'unique_clicks': 0,
							'unique_opens': 1,
							'unsubscribe_drops': 0,
							'unsubscribes': 0
							}
						}
					]
				},
				{
					'date': '2015-01-02',
					'stats': [
						{
						'metrics': {
							'blocks': 0,
							'bounce_drops': 0,
							'bounces': 0,
							'clicks': 0,
							'deferred': 0,
							'delivered': 0,
							'invalid_emails': 0,
							'opens': 0,
							'processed': 0,
							'requests': 0,
							'spam_report_drops': 0,
							'spam_reports': 0,
							'unique_clicks': 0,
							'unique_opens': 0,
							'unsubscribe_drops': 0,
							'unsubscribes': 0
							}
						}
					]
				}
			]";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(string.Format("/stats?start_date={0}&end_date={1}", startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd")), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var statistics = new Statistics(mockClient.Object);

			// Act
			var result = statistics.GetGlobalStatisticsAsync(startDate, endDate, AggregateBy.None, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual(1, result[0].Stats.Length);
			Assert.AreEqual(3, result[0].Stats[0].Metrics.Requests);
		}

		[TestMethod]
		public void GetCategoryStats()
		{
			// Arrange
			var categories = new[] { "cat1", "cat2" };
			var startDate = new DateTime(2015, 1, 1);
			var endDate = new DateTime(2015, 1, 2);
			var apiResponse = @"[
				{
					'date': '2015 - 01 - 01',
					'stats': [
						{
							'metrics': {
								'blocks': 0,
								'bounce_drops': 0,
								'bounces': 0,
								'clicks': 0,
								'deferred': 0,
								'delivered': 0,
								'invalid_emails': 0,
								'opens': 0,
								'processed': 0,
								'requests': 0,
								'spam_report_drops': 0,
								'spam_reports': 0,
								'unique_clicks': 0,
								'unique_opens': 0,
								'unsubscribe_drops': 0,
								'unsubscribes': 0
							},
							'name': 'cat1',
							'type': 'category'
						},
						{
							'metrics': {
								'blocks': 0,
								'bounce_drops': 0,
								'bounces': 0,
								'clicks': 0,
								'deferred': 0,
								'delivered': 0,
								'invalid_emails': 0,
								'opens': 0,
								'processed': 0,
								'requests': 0,
								'spam_report_drops': 0,
								'spam_reports': 0,
								'unique_clicks': 0,
								'unique_opens': 0,
								'unsubscribe_drops': 0,
								'unsubscribes': 0
							},
							'name': 'cat2',
							'type': 'category'
						}
					]
				},
				{
					'date': '2015-01-02',
					'stats': [
						{
							'metrics': {
								'blocks': 10,
								'bounce_drops': 0,
								'bounces': 0,
								'clicks': 0,
								'deferred': 0,
								'delivered': 0,
								'invalid_emails': 0,
								'opens': 0,
								'processed': 0,
								'requests': 10,
								'spam_report_drops': 0,
								'spam_reports': 0,
								'unique_clicks': 0,
								'unique_opens': 0,
								'unsubscribe_drops': 0,
								'unsubscribes': 0
							},
							'name': 'cat1',
							'type': 'category'
						},
						{
							'metrics': {
								'blocks': 0,
								'bounce_drops': 0,
								'bounces': 0,
								'clicks': 6,
								'deferred': 0,
								'delivered': 5,
								'invalid_emails': 0,
								'opens': 6,
								'processed': 0,
								'requests': 5,
								'spam_report_drops': 0,
								'spam_reports': 0,
								'unique_clicks': 5,
								'unique_opens': 5,
								'unsubscribe_drops': 0,
								'unsubscribes': 6
							},
							'name': 'cat2',
							'type': 'category'
						}
					]
				}
			]";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(string.Format("/categories/stats?start_date={0}&end_date={1}&categories={2}&categories={3}", startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), categories[0], categories[1]), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var statistics = new Statistics(mockClient.Object);

			// Act
			var result = statistics.GetCategoriesStatisticsAsync(categories, startDate, endDate, AggregateBy.None, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual(2, result[0].Stats.Length);
			Assert.AreEqual("cat1", result[0].Stats[0].Name);
			Assert.AreEqual("category", result[0].Stats[0].Type);
		}

		[TestMethod]
		public void GetSubusersStats()
		{
			// Arrange
			var subusers = new[] { "user1", "user2" };
			var startDate = new DateTime(2015, 1, 1);
			var endDate = new DateTime(2015, 1, 2);
			var apiResponse = @"[
				{
					'date': '2015-01-01',
					'stats': [
						{
							'metrics': {
								'blocks': 0,
								'bounce_drops': 0,
								'bounces': 0,
								'clicks': 0,
								'deferred': 0,
								'delivered': 0,
								'invalid_emails': 0,
								'opens': 0,
								'processed': 0,
								'requests': 0,
								'spam_report_drops': 0,
								'spam_reports': 0,
								'unique_clicks': 0,
								'unique_opens': 0,
								'unsubscribe_drops': 0,
								'unsubscribes': 0
							},
							'name': 'user1',
							'type': 'subuser'
						},
						{
							'metrics': {
								'blocks': 0,
								'bounce_drops': 0,
								'bounces': 0,
								'clicks': 0,
								'deferred': 0,
								'delivered': 0,
								'invalid_emails': 0,
								'opens': 0,
								'processed': 0,
								'requests': 0,
								'spam_report_drops': 0,
								'spam_reports': 0,
								'unique_clicks': 0,
								'unique_opens': 0,
								'unsubscribe_drops': 0,
								'unsubscribes': 0
							},
							'name': 'user2',
							'type': 'subuser'
						}
					]
				},
				{
					'date': '2015-01-02',
					'stats': [
						{
							'metrics': {
								'blocks': 10,
								'bounce_drops': 0,
								'bounces': 0,
								'clicks': 0,
								'deferred': 0,
								'delivered': 0,
								'invalid_emails': 0,
								'opens': 0,
								'processed': 0,
								'requests': 10,
								'spam_report_drops': 0,
								'spam_reports': 0,
								'unique_clicks': 0,
								'unique_opens': 0,
								'unsubscribe_drops': 0,
								'unsubscribes': 0
							},
							'name': 'user1',
							'type': 'subuser'
						},
						{
							'metrics': {
								'blocks': 0,
								'bounce_drops': 0,
								'bounces': 0,
								'clicks': 6,
								'deferred': 0,
								'delivered': 5,
								'invalid_emails': 0,
								'opens': 6,
								'processed': 0,
								'requests': 5,
								'spam_report_drops': 0,
								'spam_reports': 0,
								'unique_clicks': 5,
								'unique_opens': 5,
								'unsubscribe_drops': 0,
								'unsubscribes': 6
							},
							'name': 'user2',
							'type': 'subuser'
						}
					]
				}
			]";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(string.Format("/subusers/stats?start_date={0}&end_date={1}&subusers={2}&subusers={3}", startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), subusers[0], subusers[1]), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var statistics = new Statistics(mockClient.Object);

			// Act
			var result = statistics.GetSubusersStatisticsAsync(subusers, startDate, endDate, AggregateBy.None, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual(2, result[0].Stats.Length);
			Assert.AreEqual("user1", result[0].Stats[0].Name);
			Assert.AreEqual("subuser", result[0].Stats[0].Type);
		}

		[TestMethod]
		public void GetCountryStats()
		{
			// Arrange
			var country = "US";
			var startDate = new DateTime(2014, 10, 1);
			var endDate = new DateTime(2014, 10, 2);
			var apiResponse = @"[
				{
					'date': '2014-10-01',
					'stats': [
						{
							'metrics': {
								'clicks': 0,
								'opens': 1,
								'unique_clicks': 0,
								'unique_opens': 1
							},
							'name': 'us',
							'type': 'country'
						}
					]
				},
				{
					'date': '2014-10-02',
					'stats': [
						{
							'metrics': {
								'clicks': 0,
								'opens': 0,
								'unique_clicks': 0,
								'unique_opens': 0
							},
							'name': 'us',
							'type': 'country'
						}
					]
				}
			]";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(string.Format("/geo/stats?start_date={0}&end_date={1}&country={2}", startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), country), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var statistics = new Statistics(mockClient.Object);

			// Act
			var result = statistics.GetCountryStatisticsAsync(country, startDate, endDate, AggregateBy.None, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual(1, result[0].Stats.Length);
			Assert.AreEqual("us", result[0].Stats[0].Name);
			Assert.AreEqual("country", result[0].Stats[0].Type);
		}

		[TestMethod]
		public void GetDeviceTypesStats()
		{
			// Arrange
			var startDate = new DateTime(2014, 10, 1);
			var endDate = new DateTime(2014, 10, 2);
			var apiResponse = @"[
				{
					'date': '2014-10-01',
					'stats': [
						{
							'metrics': {
								'opens': 1,
								'unique_opens': 1
							},
							'name': 'Webmail',
							'type': 'device'
						}
					]
				},
				{
					'date': '2014-10-02',
					'stats': [
						{
							'metrics': {
								'opens': 0,
								'unique_opens': 0
							},
							'name': 'Webmail',
							'type': 'device'
						}
					]
				}
			]";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(string.Format("/devices/stats?start_date={0}&end_date={1}", startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd")), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var statistics = new Statistics(mockClient.Object);

			// Act
			var result = statistics.GetDeviceTypesStatisticsAsync(startDate, endDate, AggregateBy.None, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual(1, result[0].Stats.Length);
			Assert.AreEqual(1, result[0].Stats[0].Metrics.Opens);
			Assert.AreEqual("Webmail", result[0].Stats[0].Name);
			Assert.AreEqual("device", result[0].Stats[0].Type);
		}

		[TestMethod]
		public void GetClientTypesStats()
		{
			// Arrange
			var startDate = new DateTime(2014, 10, 1);
			var endDate = new DateTime(2014, 10, 2);
			var apiResponse = @"[
				{
					'date': '2014-10-01',
					'stats': [
						{
							'metrics': {
								'opens': 1,
								'unique_opens': 1
							},
							'name': 'Gmail',
							'type': 'device'
						}
					]
				},
				{
					'date': '2014-10-02',
					'stats': [
						{
							'metrics': {
								'opens': 0,
								'unique_opens': 0
							},
							'name': 'Gmail',
							'type': 'device'
						}
					]
				}
			]";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(string.Format("/clients/stats?start_date={0}&end_date={1}", startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd")), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var statistics = new Statistics(mockClient.Object);

			// Act
			var result = statistics.GetClientTypesStatisticsAsync(startDate, endDate, AggregateBy.None, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual(1, result[0].Stats.Length);
			Assert.AreEqual(1, result[0].Stats[0].Metrics.Opens);
			Assert.AreEqual("Gmail", result[0].Stats[0].Name);
			Assert.AreEqual("client", result[0].Stats[0].Type);
		}

		[TestMethod]
		public void GetInboxProvidersStats()
		{
			// Arrange
			var providers = new[] { "Gmail", "Hotmail" };
			var startDate = new DateTime(2014, 10, 1);
			var endDate = new DateTime(2014, 10, 2);
			var apiResponse = @"[
				{
					'date': '2014-10-01',
					'stats': [
						{
							'metrics': {
								'blocks': 1,
								'bounces': 0,
								'clicks': 0,
								'deferred': 1,
								'delivered': 1,
								'drops': 0,
								'opens': 1,
								'spam_reports': 0,
								'unique_clicks': 0,
								'unique_opens': 1
							},
							'name': 'Gmail',
							'type': 'esp'
						}
					]
				},
				{
					'date': '2014-10-02',
					'stats': [
						{
							'metrics': {
								'blocks': 0,
								'bounces': 0,
								'clicks': 0,
								'deferred': 0,
								'delivered': 0,
								'drops': 0,
								'opens': 0,
								'spam_reports': 0,
								'unique_clicks': 0,
								'unique_opens': 0
							},
							'name': 'Gmail',
							'type': 'esp'
						}
					]
				}
			]";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(string.Format("/mailbox_providers/stats?start_date={0}&end_date={1}&mailbox_providers={2}&mailbox_providers={3}", startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), providers[0], providers[1]), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var statistics = new Statistics(mockClient.Object);

			// Act
			var result = statistics.GetInboxProvidersStatisticsAsync(providers, startDate, endDate, AggregateBy.None, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual(1, result[0].Stats.Length);
			Assert.AreEqual("Gmail", result[0].Stats[0].Name);
			Assert.AreEqual("esp", result[0].Stats[0].Type);
		}

		[TestMethod]
		public void GetBrowsersStats()
		{
			// Arrange
			var browsers = new[] { "Chrome", "Firefox" };
			var startDate = new DateTime(2014, 10, 1);
			var endDate = new DateTime(2014, 10, 2);
			var apiResponse = @"[
				{
					'date': '2014-10-01',
					'stats': [
						{
							'metrics': {
								'clicks': 0,
								'unique_clicks': 0
							},
							'name': 'Chrome',
							'type': 'browser'
						},
						{
							'metrics': {
								'clicks': 1,
								'unique_clicks': 1
							},
							'name': 'Firefox',
							'type': 'browser'
						}
					]
				},
				{
					'date': '2014-10-02',
					'stats': [
						{
							'metrics': {
								'clicks': 0,
								'unique_clicks': 0
							},
							'name': 'Chrome',
							'type': 'browser'
						},
						{
							'metrics': {
								'clicks': 1,
								'unique_clicks': 1
							},
							'name': 'Firefox',
							'type': 'browser'
						}
					]
				}
			]";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(string.Format("/browsers/stats?start_date={0}&end_date={1}&browsers={2}&browsers={3}", startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), browsers[0], browsers[1]), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var statistics = new Statistics(mockClient.Object);

			// Act
			var result = statistics.GetBrowsersStatisticsAsync(browsers, startDate, endDate, AggregateBy.None, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual(2, result[0].Stats.Length);
			Assert.AreEqual("Chrome", result[0].Stats[0].Name);
			Assert.AreEqual("browser", result[0].Stats[0].Type);
		}
	}
}
