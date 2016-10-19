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
	public class GlobalStatsTests
	{
		private const string ENDPOINT = "/stats";

		[TestMethod]
		public void Get()
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
			mockClient.Setup(c => c.GetAsync(ENDPOINT + "?start_date=" + startDate.ToString("yyyy-MM-dd") + "&end_date=" + endDate.ToString("yyyy-MM-dd"), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var globalStats = new GlobalStats(mockClient.Object);

			// Act
			var result = globalStats.GetAsync(startDate, endDate, AggregateBy.None, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual(1, result[0].Stats.Length);
			Assert.AreEqual(3, result[0].Stats[0].Metrics.Requests);
		}
	}
}
