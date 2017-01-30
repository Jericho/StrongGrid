using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Model;
using StrongGrid.UnitTests;
using StrongGrid.Utilities;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class AlertsTests
	{
		#region FIELDS

		private const string ENDPOINT = "/alerts";

		private const string SINGLE_ALERT_JSON = @"{
			'created_at': 1451520930,
			'email_to': 'test@example.com',
			'frequency': 'daily',
			'id': 48,
			'type': 'stats_notification',
			'updated_at': 1451520930
		}";
		private const string MULTIPLE_ALERTS_JSON = @"[
			{
				'created_at': 1451498784,
				'email_to': 'test@example.com',
				'id': 46,
				'percentage': 90,
				'type': 'usage_limit',
				'updated_at': 1451498784
			},
			{
				'created_at': 1451498812,
				'email_to': 'test@example.com',
				'frequency': 'monthly',
				'id': 47,
				'type': 'stats_notification',
				'updated_at': 1451498812
			},
			{
				'created_at': 1451520930,
				'email_to': 'test@example.com',
				'frequency': 'daily',
				'id': 48,
				'type': 'stats_notification',
				'updated_at': 1451520930
			}
		]";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<Alert>(SINGLE_ALERT_JSON);

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
		public void Create()
		{
			// Arrange
			var type = AlertType.StatsNotification;
			var emailTo = "test@example.cpm";
			var frequency = Frequency.Weekly;
			var percentage = 75;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, ENDPOINT).Respond("application/json", SINGLE_ALERT_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var alerts = new Alerts(client, ENDPOINT);

			// Act
			var result = alerts.CreateAsync(type, emailTo, frequency, percentage, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void Get()
		{
			// Arrange
			var alertId = 48;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, $"{ENDPOINT}/{alertId}").Respond("application/json", SINGLE_ALERT_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var alerts = new Alerts(client, ENDPOINT);

			// Act
			var result = alerts.GetAsync(alertId, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetAll()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, ENDPOINT).Respond("application/json", MULTIPLE_ALERTS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var alerts = new Alerts(client, ENDPOINT);

			// Act
			var result = alerts.GetAllAsync(CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(3);
		}

		[Fact]
		public void Delete()
		{
			// Arrange
			var alertId = 48;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, $"{ENDPOINT}/{alertId}").Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var alerts = new Alerts(client, ENDPOINT);

			// Act
			alerts.DeleteAsync(alertId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void Update()
		{
			// Arrange
			var alertId = 48;
			var emailTo = "test@example.com";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), $"{ENDPOINT}/{alertId}").Respond("application/json", SINGLE_ALERT_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var alerts = new Alerts(client, ENDPOINT);

			// Act
			var result = alerts.UpdateAsync(alertId, null, emailTo, null, null, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}
	}
}
