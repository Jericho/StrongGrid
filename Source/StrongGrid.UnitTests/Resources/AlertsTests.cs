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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_ALERT_JSON) })
				.Verifiable();

			var alerts = new Alerts(mockClient.Object, ENDPOINT);

			// Act
			var result = alerts.CreateAsync(type, emailTo, frequency, percentage, CancellationToken.None).Result;

			// Assert
			mockRepository.VerifyAll();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void Get()
		{
			// Arrange
			var alertId = 48;

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{alertId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_ALERT_JSON) })
				.Verifiable();

			var alerts = new Alerts(mockClient.Object, ENDPOINT);

			// Act
			var result = alerts.GetAsync(alertId, CancellationToken.None).Result;

			// Assert
			mockRepository.VerifyAll();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetAll()
		{
			// Arrange
			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_ALERTS_JSON) })
				.Verifiable();

			var alerts = new Alerts(mockClient.Object, ENDPOINT);

			// Act
			var result = alerts.GetAllAsync(CancellationToken.None).Result;

			// Assert
			mockRepository.VerifyAll();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(3);
		}

		[Fact]
		public void Delete()
		{
			// Arrange
			var alertId = 48;

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{alertId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
				.Verifiable();

			var alerts = new Alerts(mockClient.Object, ENDPOINT);

			// Act
			alerts.DeleteAsync(alertId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockRepository.VerifyAll();
		}

		[Fact]
		public void Update()
		{
			// Arrange
			var alertId = 48;
			var emailTo = "test@example.com";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync($"{ENDPOINT}/{alertId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_ALERT_JSON) })
				.Verifiable();

			var alerts = new Alerts(mockClient.Object, ENDPOINT);

			// Act
			var result = alerts.UpdateAsync(alertId, null, emailTo, null, null, CancellationToken.None).Result;

			// Assert
			mockRepository.VerifyAll();
			result.ShouldNotBeNull();
		}
	}
}
