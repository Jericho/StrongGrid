using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.Resources;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class StatisticsTests
	{
		private readonly ITestOutputHelper _outputHelper;

		public StatisticsTests(ITestOutputHelper outputHelper)
		{
			_outputHelper = outputHelper;
		}

		[Fact]
		public async Task GetGlobalStatsAsync()
		{
			// Arrange
			var startDate = new DateTime(2015, 1, 1);
			var endDate = new DateTime(2015, 1, 2);

			var apiResponse = @"[
				{
					""date"": ""2015-01-01"",
					""stats"": [
						{
							""metrics"": {
								""blocks"": 1,
								""bounce_drops"": 0,
								""bounces"": 0,
								""clicks"": 0,
								""deferred"": 1,
								""delivered"": 1,
								""invalid_emails"": 1,
								""opens"": 1,
								""processed"": 2,
								""requests"": 3,
								""spam_report_drops"": 0,
								""spam_reports"": 0,
								""unique_clicks"": 0,
								""unique_opens"": 1,
								""unsubscribe_drops"": 0,
								""unsubscribes"": 0
							}
						}
					]
				},
				{
					""date"": ""2015-01-02"",
					""stats"": [
						{
							""metrics"": {
								""blocks"": 0,
								""bounce_drops"": 0,
								""bounces"": 0,
								""clicks"": 0,
								""deferred"": 0,
								""delivered"": 0,
								""invalid_emails"": 0,
								""opens"": 0,
								""processed"": 0,
								""requests"": 0,
								""spam_report_drops"": 0,
								""spam_reports"": 0,
								""unique_clicks"": 0,
								""unique_opens"": 0,
								""unsubscribe_drops"": 0,
								""unsubscribes"": 0
							}
						}
					]
				}
			]";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri($"stats?start_date={startDate.ToString("yyyy-MM-dd")}&end_date={endDate.ToString("yyyy-MM-dd")}")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var statistics = new Statistics(client);

			// Act
			var result = await statistics.GetGlobalStatisticsAsync(startDate, endDate, AggregateBy.None, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
			result[0].Stats.Length.ShouldBe(1);
			result[0].Stats[0].Metrics.Single(m => m.Key == "requests").Value.ShouldBe(3);
		}

		[Fact]
		public async Task GetCategoryStatsAsync()
		{
			// Arrange
			var categories = new[] { "cat1", "cat2" };
			var startDate = new DateTime(2015, 1, 1);
			var endDate = new DateTime(2015, 1, 2);

			var apiResponse = @"[
				{
					""date"": ""2015-01-01"",
					""stats"": [
						{
							""metrics"": {
								""blocks"": 0,
								""bounce_drops"": 0,
								""bounces"": 0,
								""clicks"": 0,
								""deferred"": 0,
								""delivered"": 0,
								""invalid_emails"": 0,
								""opens"": 0,
								""processed"": 0,
								""requests"": 0,
								""spam_report_drops"": 0,
								""spam_reports"": 0,
								""unique_clicks"": 0,
								""unique_opens"": 0,
								""unsubscribe_drops"": 0,
								""unsubscribes"": 0
							},
							""name"": ""cat1"",
							""type"": ""category""
						},
						{
							""metrics"": {
								""blocks"": 0,
								""bounce_drops"": 0,
								""bounces"": 0,
								""clicks"": 0,
								""deferred"": 0,
								""delivered"": 0,
								""invalid_emails"": 0,
								""opens"": 0,
								""processed"": 0,
								""requests"": 0,
								""spam_report_drops"": 0,
								""spam_reports"": 0,
								""unique_clicks"": 0,
								""unique_opens"": 0,
								""unsubscribe_drops"": 0,
								""unsubscribes"": 0
							},
							""name"": ""cat2"",
							""type"": ""category""
						}
					]
				},
				{
					""date"": ""2015-01-02"",
					""stats"": [
						{
							""metrics"": {
								""blocks"": 10,
								""bounce_drops"": 0,
								""bounces"": 0,
								""clicks"": 0,
								""deferred"": 0,
								""delivered"": 0,
								""invalid_emails"": 0,
								""opens"": 0,
								""processed"": 0,
								""requests"": 10,
								""spam_report_drops"": 0,
								""spam_reports"": 0,
								""unique_clicks"": 0,
								""unique_opens"": 0,
								""unsubscribe_drops"": 0,
								""unsubscribes"": 0
							},
							""name"": ""cat1"",
							""type"": ""category""
						},
						{
							""metrics"": {
								""blocks"": 0,
								""bounce_drops"": 0,
								""bounces"": 0,
								""clicks"": 6,
								""deferred"": 0,
								""delivered"": 5,
								""invalid_emails"": 0,
								""opens"": 6,
								""processed"": 0,
								""requests"": 5,
								""spam_report_drops"": 0,
								""spam_reports"": 0,
								""unique_clicks"": 5,
								""unique_opens"": 5,
								""unsubscribe_drops"": 0,
								""unsubscribes"": 6
							},
							""name"": ""cat2"",
							""type"": ""category""
						}
					]
				}
			]";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri($"categories/stats?start_date={startDate.ToString("yyyy-MM-dd")}&end_date={endDate.ToString("yyyy-MM-dd")}&categories={categories[0]}&categories={categories[1]}")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var statistics = new Statistics(client);

			// Act
			var result = await statistics.GetCategoriesStatisticsAsync(categories, startDate, endDate, AggregateBy.None, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
			result[0].Stats.Length.ShouldBe(2);
			result[0].Stats[0].Name.ShouldBe("cat1");
			result[0].Stats[0].Type.ShouldBe("category");
		}

		[Fact]
		public async Task GetSubusersStatsAsync()
		{
			// Arrange
			var subusers = new[] { "user1", "user2" };
			var startDate = new DateTime(2015, 1, 1);
			var endDate = new DateTime(2015, 1, 2);

			var apiResponse = @"[
				{
					""date"": ""2015-01-01"",
					""stats"": [
						{
							""metrics"": {
								""blocks"": 0,
								""bounce_drops"": 0,
								""bounces"": 0,
								""clicks"": 0,
								""deferred"": 0,
								""delivered"": 0,
								""invalid_emails"": 0,
								""opens"": 0,
								""processed"": 0,
								""requests"": 0,
								""spam_report_drops"": 0,
								""spam_reports"": 0,
								""unique_clicks"": 0,
								""unique_opens"": 0,
								""unsubscribe_drops"": 0,
								""unsubscribes"": 0
							},
							""name"": ""user1"",
							""type"": ""subuser""
						},
						{
							""metrics"": {
								""blocks"": 0,
								""bounce_drops"": 0,
								""bounces"": 0,
								""clicks"": 0,
								""deferred"": 0,
								""delivered"": 0,
								""invalid_emails"": 0,
								""opens"": 0,
								""processed"": 0,
								""requests"": 0,
								""spam_report_drops"": 0,
								""spam_reports"": 0,
								""unique_clicks"": 0,
								""unique_opens"": 0,
								""unsubscribe_drops"": 0,
								""unsubscribes"": 0
							},
							""name"": ""user2"",
							""type"": ""subuser""
						}
					]
				},
				{
					""date"": ""2015-01-02"",
					""stats"": [
						{
							""metrics"": {
								""blocks"": 10,
								""bounce_drops"": 0,
								""bounces"": 0,
								""clicks"": 0,
								""deferred"": 0,
								""delivered"": 0,
								""invalid_emails"": 0,
								""opens"": 0,
								""processed"": 0,
								""requests"": 10,
								""spam_report_drops"": 0,
								""spam_reports"": 0,
								""unique_clicks"": 0,
								""unique_opens"": 0,
								""unsubscribe_drops"": 0,
								""unsubscribes"": 0
							},
							""name"": ""user1"",
							""type"": ""subuser""
						},
						{
							""metrics"": {
								""blocks"": 0,
								""bounce_drops"": 0,
								""bounces"": 0,
								""clicks"": 6,
								""deferred"": 0,
								""delivered"": 5,
								""invalid_emails"": 0,
								""opens"": 6,
								""processed"": 0,
								""requests"": 5,
								""spam_report_drops"": 0,
								""spam_reports"": 0,
								""unique_clicks"": 5,
								""unique_opens"": 5,
								""unsubscribe_drops"": 0,
								""unsubscribes"": 6
							},
							""name"": ""user2"",
							""type"": ""subuser""
						}
					]
				}
			]";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri($"subusers/stats?start_date={startDate.ToString("yyyy-MM-dd")}&end_date={endDate.ToString("yyyy-MM-dd")}&subusers={subusers[0]}&subusers={subusers[1]}")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var statistics = new Statistics(client);

			// Act
			var result = await statistics.GetSubusersStatisticsAsync(subusers, startDate, endDate, AggregateBy.None, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
			result[0].Stats.Length.ShouldBe(2);
			result[0].Stats[0].Name.ShouldBe("user1");
			result[0].Stats[0].Type.ShouldBe("subuser");
		}

		[Fact]
		public async Task GetCountryStatsAsync()
		{
			// Arrange
			var country = "US";
			var startDate = new DateTime(2014, 10, 1);
			var endDate = new DateTime(2014, 10, 2);

			var apiResponse = @"[
				{
					""date"": ""2014-10-01"",
					""stats"": [
						{
							""metrics"": {
								""clicks"": 0,
								""opens"": 1,
								""unique_clicks"": 0,
								""unique_opens"": 1
							},
							""name"": ""us"",
							""type"": ""country""
						}
					]
				},
				{
					""date"": ""2014-10-02"",
					""stats"": [
						{
							""metrics"": {
								""clicks"": 0,
								""opens"": 0,
								""unique_clicks"": 0,
								""unique_opens"": 0
							},
							""name"": ""us"",
							""type"": ""country""
						}
					]
				}
			]";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri($"geo/stats?start_date={startDate.ToString("yyyy-MM-dd")}&end_date={endDate.ToString("yyyy-MM-dd")}&country={country}")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var statistics = new Statistics(client);

			// Act
			var result = await statistics.GetCountryStatisticsAsync(country, startDate, endDate, AggregateBy.None, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
			result[0].Stats.Length.ShouldBe(1);
			result[0].Stats[0].Name.ShouldBe("us");
			result[0].Stats[0].Type.ShouldBe("country");
		}

		[Fact]
		public async Task GetDeviceTypesStatsAsync()
		{
			// Arrange
			var startDate = new DateTime(2014, 10, 1);
			var endDate = new DateTime(2014, 10, 2);

			var apiResponse = @"[
				{
					""date"": ""2014-10-01"",
					""stats"": [
						{
							""metrics"": {
								""opens"": 1,
								""unique_opens"": 1
							},
							""name"": ""Webmail"",
							""type"": ""device""
						}
					]
				},
				{
					""date"": ""2014-10-02"",
					""stats"": [
						{
							""metrics"": {
								""opens"": 0,
								""unique_opens"": 0
							},
							""name"": ""Webmail"",
							""type"": ""device""
						}
					]
				}
			]";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri($"devices/stats?start_date={startDate.ToString("yyyy-MM-dd")}&end_date={endDate.ToString("yyyy-MM-dd")}")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var statistics = new Statistics(client);

			// Act
			var result = await statistics.GetDeviceTypesStatisticsAsync(startDate, endDate, AggregateBy.None, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
			result[0].Stats.Length.ShouldBe(1);
			result[0].Stats[0].Metrics.Single(m => m.Key == "opens").Value.ShouldBe(1);
			result[0].Stats[0].Name.ShouldBe("Webmail");
			result[0].Stats[0].Type.ShouldBe("device");
		}

		[Fact]
		public async Task GetClientTypesStatsAsync()
		{
			// Arrange
			var startDate = new DateTime(2014, 10, 1);
			var endDate = new DateTime(2014, 10, 2);

			var apiResponse = @"[
				{
					""date"": ""2014-10-01"",
					""stats"": [
						{
							""metrics"": {
								""opens"": 1,
								""unique_opens"": 1
							},
							""name"": ""Gmail"",
							""type"": ""client""
						}
					]
				},
				{
					""date"": ""2014-10-02"",
					""stats"": [
						{
							""metrics"": {
								""opens"": 0,
								""unique_opens"": 0
							},
							""name"": ""Gmail"",
							""type"": ""client""
						}
					]
				}
			]";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri($"clients/stats?start_date={startDate.ToString("yyyy-MM-dd")}&end_date={endDate.ToString("yyyy-MM-dd")}")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var statistics = new Statistics(client);

			// Act
			var result = await statistics.GetClientTypesStatisticsAsync(startDate, endDate, AggregateBy.None, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
			result[0].Stats.Length.ShouldBe(1);
			result[0].Stats[0].Metrics.Single(m => m.Key == "opens").Value.ShouldBe(1);
			result[0].Stats[0].Name.ShouldBe("Gmail");
			result[0].Stats[0].Type.ShouldBe("client");
		}

		[Fact]
		public async Task GetInboxProvidersStatsAsync()
		{
			// Arrange
			var providers = new[] { "Gmail", "Hotmail" };
			var startDate = new DateTime(2014, 10, 1);
			var endDate = new DateTime(2014, 10, 2);

			var apiResponse = @"[
				{
					""date"": ""2014-10-01"",
					""stats"": [
						{
							""metrics"": {
								""blocks"": 1,
								""bounces"": 0,
								""clicks"": 0,
								""deferred"": 1,
								""delivered"": 1,
								""drops"": 0,
								""opens"": 1,
								""spam_reports"": 0,
								""unique_clicks"": 0,
								""unique_opens"": 1
							},
							""name"": ""Gmail"",
							""type"": ""esp""
						}
					]
				},
				{
					""date"": ""2014-10-02"",
					""stats"": [
						{
							""metrics"": {
								""blocks"": 0,
								""bounces"": 0,
								""clicks"": 0,
								""deferred"": 0,
								""delivered"": 0,
								""drops"": 0,
								""opens"": 0,
								""spam_reports"": 0,
								""unique_clicks"": 0,
								""unique_opens"": 0
							},
							""name"": ""Gmail"",
							""type"": ""esp""
						}
					]
				}
			]";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri($"mailbox_providers/stats?start_date={startDate.ToString("yyyy-MM-dd")}&end_date={endDate.ToString("yyyy-MM-dd")}&mailbox_providers={providers[0]}&mailbox_providers={providers[1]}")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var statistics = new Statistics(client);

			// Act
			var result = await statistics.GetInboxProvidersStatisticsAsync(providers, startDate, endDate, AggregateBy.None, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
			result[0].Stats.Length.ShouldBe(1);
			result[0].Stats[0].Name.ShouldBe("Gmail");
			result[0].Stats[0].Type.ShouldBe("esp");
		}

		[Fact]
		public async Task GetBrowsersStatsAsync()
		{
			// Arrange
			var browsers = new[] { "Chrome", "Firefox" };
			var startDate = new DateTime(2014, 10, 1);
			var endDate = new DateTime(2014, 10, 2);

			var apiResponse = @"[
				{
					""date"": ""2014-10-01"",
					""stats"": [
						{
							""metrics"": {
								""clicks"": 0,
								""unique_clicks"": 0
							},
							""name"": ""Chrome"",
							""type"": ""browser""
						},
						{
							""metrics"": {
								""clicks"": 1,
								""unique_clicks"": 1
							},
							""name"": ""Firefox"",
							""type"": ""browser""
						}
					]
				},
				{
					""date"": ""2014-10-02"",
					""stats"": [
						{
							""metrics"": {
								""clicks"": 0,
								""unique_clicks"": 0
							},
							""name"": ""Chrome"",
							""type"": ""browser""
						},
						{
							""metrics"": {
								""clicks"": 1,
								""unique_clicks"": 1
							},
							""name"": ""Firefox"",
							""type"": ""browser""
						}
					]
				}
			]";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri($"browsers/stats?start_date={startDate.ToString("yyyy-MM-dd")}&end_date={endDate.ToString("yyyy-MM-dd")}&browsers={browsers[0]}&browsers={browsers[1]}")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var statistics = new Statistics(client);

			// Act
			var result = await statistics.GetBrowsersStatisticsAsync(browsers, startDate, endDate, AggregateBy.None, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
			result[0].Stats.Length.ShouldBe(2);
			result[0].Stats[0].Name.ShouldBe("Chrome");
			result[0].Stats[0].Type.ShouldBe("browser");
		}
	}
}
