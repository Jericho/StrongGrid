using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class AlertsTests
	{
		#region FIELDS

		private const string ENDPOINT = "/alerts";
		private MockRepository _mockRepository;
		private Mock<IClient> _mockClient;

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

		private Alerts CreateAlerts()
		{
			return new Alerts(_mockClient.Object, ENDPOINT);

		}

		[TestInitialize]
		public void TestInitialize()
		{
			_mockRepository = new MockRepository(MockBehavior.Strict);
			_mockClient = _mockRepository.Create<IClient>();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			_mockRepository.VerifyAll();
		}

		[TestMethod]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<Alert>(SINGLE_ALERT_JSON);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(new DateTime(2015, 12, 31, 0, 15, 30, DateTimeKind.Utc), result.CreatedOn);
			Assert.AreEqual("test@example.com", result.EmailTo);
			Assert.AreEqual(Frequency.Daily, result.Frequency);
			Assert.AreEqual(48, result.Id);
			Assert.AreEqual(new DateTime(2015, 12, 31, 0, 15, 30, DateTimeKind.Utc), result.ModifiedOn);
			Assert.AreEqual(AlertType.StatsNotification, result.Type);
		}

		[TestMethod]
		public void Create()
		{
			// Arrange
			var type = AlertType.StatsNotification;
			var emailTo = "test@example.cpm";
			var frequency = Frequency.Weekly;
			var percentage = 75;

			_mockClient
				.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_ALERT_JSON) })
				.Verifiable();

			var alerts = CreateAlerts();

			// Act
			var result = alerts.CreateAsync(type, emailTo, frequency, percentage, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var alertId = 48;

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{alertId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_ALERT_JSON) })
				.Verifiable();

			var alerts = CreateAlerts();

			// Act
			var result = alerts.GetAsync(alertId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			_mockClient
				.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_ALERTS_JSON) })
				.Verifiable();

			var alerts = CreateAlerts();

			// Act
			var result = alerts.GetAllAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Length);
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			var alertId = 48;

			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{alertId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
				.Verifiable();

			var alerts = CreateAlerts();

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

			_mockClient
				.Setup(c => c.PatchAsync($"{ENDPOINT}/{alertId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_ALERT_JSON) })
				.Verifiable();

			var alerts = CreateAlerts();

			// Act
			var result = alerts.UpdateAsync(alertId, null, emailTo, null, null, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}
	}
}
