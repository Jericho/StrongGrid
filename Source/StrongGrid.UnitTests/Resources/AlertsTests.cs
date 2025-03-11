using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Resources;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class AlertsTests
	{
		#region FIELDS

		internal const string ENDPOINT = "alerts";

		internal const string SINGLE_ALERT_JSON = @"{
			""created_at"": 1451520930,
			""email_to"": ""test@example.com"",
			""frequency"": ""daily"",
			""id"": 48,
			""type"": ""stats_notification"",
			""updated_at"": 1451520930
		}";
		internal const string MULTIPLE_ALERTS_JSON = @"[
			{
				""created_at"": 1451498784,
				""email_to"": ""test@example.com"",
				""id"": 46,
				""percentage"": 90,
				""type"": ""usage_limit"",
				""updated_at"": 1451498784
			},
			{
				""created_at"": 1451498812,
				""email_to"": ""test@example.com"",
				""frequency"": ""monthly"",
				""id"": 47,
				""type"": ""stats_notification"",
				""updated_at"": 1451498812
			},
			{
				""created_at"": 1451520930,
				""email_to"": ""test@example.com"",
				""frequency"": ""daily"",
				""id"": 48,
				""type"": ""stats_notification"",
				""updated_at"": 1451520930
			}
		]";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<Alert>(SINGLE_ALERT_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.CreatedOn.ShouldBe(new DateTime(2015, 12, 31, 0, 15, 30, DateTimeKind.Utc));
			result.EmailTo.ShouldBe("test@example.com");
			result.Frequency.ShouldBe(Frequency.Daily);
			result.Id.ShouldBe(48);
			result.ModifiedOn.ShouldBe(new DateTime(2015, 12, 31, 0, 15, 30, DateTimeKind.Utc));
			result.Type.ShouldBe(AlertType.StatsNotification);
		}

		[Fact]
		public async Task Create()
		{
			// Arrange
			var type = AlertType.StatsNotification;
			var emailTo = "test@example.cpm";
			var frequency = Frequency.Weekly;
			var percentage = 75;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_ALERT_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var alerts = new Alerts(client);

			// Act
			var result = await alerts.CreateAsync(type, emailTo, frequency, percentage, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task Get()
		{
			// Arrange
			var alertId = 48;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, alertId)).Respond("application/json", SINGLE_ALERT_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var alerts = new Alerts(client);

			// Act
			var result = await alerts.GetAsync(alertId, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetAll()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", MULTIPLE_ALERTS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var alerts = new Alerts(client);

			// Act
			var result = await alerts.GetAllAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(3);
		}

		[Fact]
		public async Task Delete()
		{
			// Arrange
			var alertId = 48;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, alertId)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var alerts = new Alerts(client);

			// Act
			await alerts.DeleteAsync(alertId, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task Update()
		{
			// Arrange
			var alertId = 48;
			var emailTo = "test@example.com";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT, alertId)).Respond("application/json", SINGLE_ALERT_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var alerts = new Alerts(client);

			// Act
			var result = await alerts.UpdateAsync(alertId, null, emailTo, null, null, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}
	}
}
