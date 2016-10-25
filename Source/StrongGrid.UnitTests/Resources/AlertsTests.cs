using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class AlertsTests
	{
		private const string ENDPOINT = "/alerts";

		[TestMethod]
		public void Create()
		{
			// Arrange
			var type = AlertType.StatsNotification;
			var emailTo = "test@example.cpm";
			var frequency = Frequency.Weekly;
			var percentage = 75;

			var apiResponse = @"{
				'created_at': 1451520930,
				'email_to': 'test@example.com',
				'frequency': 'daily',
				'id': 48,
				'type': 'stats_notification',
				'updated_at': 1451520930
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var alerts = new Alerts(mockClient.Object);

			// Act
			var result = alerts.CreateAsync(type, emailTo, frequency, percentage, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(48, result.Id);
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var alertId = 48;
			var apiResponse = @"{
				'created_at': 1451520930,
				'email_to': 'test@example.com',
				'frequency': 'daily',
				'id': 48,
				'type': 'stats_notification',
				'updated_at': 1451520930
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync($"{ENDPOINT}/{alertId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var alerts = new Alerts(mockClient.Object);

			// Act
			var result = alerts.GetAsync(alertId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(alertId, result.Id);
			Assert.AreEqual(Frequency.Daily, result.Frequency);
			Assert.AreEqual(AlertType.StatsNotification, result.Type);
		}

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			var apiResponse = @"[
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

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var alerts = new Alerts(mockClient.Object);

			// Act
			var result = alerts.GetAllAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Length);
			Assert.AreEqual(46, result[0].Id);
			Assert.AreEqual(47, result[1].Id);
			Assert.AreEqual(48, result[2].Id);
			Assert.AreEqual(AlertType.UsageLimit, result[0].Type);
			Assert.AreEqual(AlertType.StatsNotification, result[1].Type);
			Assert.AreEqual(AlertType.StatsNotification, result[2].Type);
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			var alertId = 48;

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync($"{ENDPOINT}/{alertId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var alerts = new Alerts(mockClient.Object);

			// Act
			alerts.DeleteAsync(alertId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Update()
		{
			// Arrange
			var alertId = 48;
			var emailTo = "test@example.com";

			var apiResponse = @"{
				'created_at': 1451520930,
				'email_to': 'test@example.com',
				'frequency': 'daily',
				'id': 48,
				'type': 'stats_notification',
				'updated_at': 1451522691
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PatchAsync($"{ENDPOINT}/{alertId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var alerts = new Alerts(mockClient.Object);

			// Act
			var result = alerts.UpdateAsync(alertId, null, emailTo, null, null, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(Frequency.Daily, result.Frequency);
		}
	}
}
