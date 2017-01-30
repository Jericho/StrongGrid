using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Model;
using StrongGrid.UnitTests;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class StatisticsTests
	{
		#region FIELDS

		private const string ENDPOINT = "/stats";

		#endregion

		[Fact]
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, $"/stats?start_date={startDate.ToString("yyyy-MM-dd")}&end_date={ endDate.ToString("yyyy-MM-dd")}").Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var statistics = new Statistics(client, ENDPOINT);

			// Act
			var result = statistics.GetGlobalStatisticsAsync(startDate, endDate, AggregateBy.None, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
			result[0].Stats.Length.ShouldBe(1);
			result[0].Stats[0].Metrics.Requests.ShouldBe(3);
		}

		[Fact]
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, $"/categories/stats?start_date={startDate.ToString("yyyy-MM-dd")}&end_date={endDate.ToString("yyyy-MM-dd")}&categories={categories[0]}&categories={categories[1]}").Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var statistics = new Statistics(client, ENDPOINT);

			// Act
			var result = statistics.GetCategoriesStatisticsAsync(categories, startDate, endDate, AggregateBy.None, CancellationToken.None).Result;

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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, $"/subusers/stats?start_date={startDate.ToString("yyyy-MM-dd")}&end_date={endDate.ToString("yyyy-MM-dd")}&subusers={subusers[0]}&subusers={subusers[1]}").Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var statistics = new Statistics(client, ENDPOINT);

			// Act
			var result = statistics.GetSubusersStatisticsAsync(subusers, startDate, endDate, AggregateBy.None, CancellationToken.None).Result;

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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, $"/geo/stats?start_date={startDate.ToString("yyyy-MM-dd")}&end_date={endDate.ToString("yyyy-MM-dd")}&country={country}").Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var statistics = new Statistics(client, ENDPOINT);

			// Act
			var result = statistics.GetCountryStatisticsAsync(country, startDate, endDate, AggregateBy.None, CancellationToken.None).Result;

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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, $"/devices/stats?start_date={startDate.ToString("yyyy-MM-dd")}&end_date={endDate.ToString("yyyy-MM-dd")}").Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var statistics = new Statistics(client, ENDPOINT);

			// Act
			var result = statistics.GetDeviceTypesStatisticsAsync(startDate, endDate, AggregateBy.None, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
			result[0].Stats.Length.ShouldBe(1);
			result[0].Stats[0].Metrics.Opens.ShouldBe(1);
			result[0].Stats[0].Name.ShouldBe("Webmail");
			result[0].Stats[0].Type.ShouldBe("device");
		}

		[Fact]
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
							'type': 'client'
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
							'type': 'client'
						}
					]
				}
			]";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, $"/clients/stats?start_date={startDate.ToString("yyyy-MM-dd")}&end_date={endDate.ToString("yyyy-MM-dd")}").Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var statistics = new Statistics(client, ENDPOINT);

			// Act
			var result = statistics.GetClientTypesStatisticsAsync(startDate, endDate, AggregateBy.None, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
			result[0].Stats.Length.ShouldBe(1);
			result[0].Stats[0].Metrics.Opens.ShouldBe(1);
			result[0].Stats[0].Name.ShouldBe("Gmail");
			result[0].Stats[0].Type.ShouldBe("client");
		}

		[Fact]
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, $"/mailbox_providers/stats?start_date={startDate.ToString("yyyy-MM-dd")}&end_date={endDate.ToString("yyyy-MM-dd")}&mailbox_providers={providers[0]}&mailbox_providers={providers[1]}").Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var statistics = new Statistics(client, ENDPOINT);

			// Act
			var result = statistics.GetInboxProvidersStatisticsAsync(providers, startDate, endDate, AggregateBy.None, CancellationToken.None).Result;

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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, $"/browsers/stats?start_date={startDate.ToString("yyyy-MM-dd")}&end_date={endDate.ToString("yyyy-MM-dd")}&browsers={browsers[0]}&browsers={browsers[1]}").Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var statistics = new Statistics(client, ENDPOINT);

			// Act
			var result = statistics.GetBrowsersStatisticsAsync(browsers, startDate, endDate, AggregateBy.None, CancellationToken.None).Result;

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
