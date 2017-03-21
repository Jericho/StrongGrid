using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Model;
using StrongGrid.UnitTests;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class WebhookStatsTests
	{
		[Fact]
		public void GetGlobalStats()
		{
			// Arrange
			var startDate = new DateTime(2015, 1, 1);
			var endDate = new DateTime(2015, 1, 2);
			var apiResponse = @"[
				{
					'date': '2015-01-01',
					'stats': [
						{
							'metrics': {
								'received': 1
							}
						}
					]
				},
				{
					'date': '2015-01-02',
					'stats': [
						{
							'metrics': {
								'received': 3
							}
						}
					]
				}
			]";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri($"user/webhooks/parse/stats?start_date={startDate.ToString("yyyy-MM-dd")}&end_date={ endDate.ToString("yyyy-MM-dd")}")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var webhookStats = new WebhookStats(client);

			// Act
			var result = webhookStats.GetInboundParseUsageAsync(startDate, endDate, AggregateBy.None, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
			result[0].Stats.Length.ShouldBe(1);

			result[0].Stats[0].Metrics.Length.ShouldBe(1);
			result[0].Stats[0].Metrics.Single(m => m.Key == "received").Value.ShouldBe(1);

			result[1].Stats[0].Metrics.Length.ShouldBe(1);
			result[1].Stats[0].Metrics.Single(m => m.Key == "received").Value.ShouldBe(3);
		}
	}
}
