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
	public class SettingsTests
	{
		[TestMethod]
		public void GetAllMailSettings()
		{
			// Arrange
			var limit = 15;
			var offset = 3;

			var apiResponse = @"{
				'result': [
					{
						'name': 'bcc',
						'title': 'BCC',
						'description': 'lorem ipsum... .',
						'enabled': true
					}
				]
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync("/mail_settings?limit=" + limit + "&offset=" + offset, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var settings = new Settings(mockClient.Object);

			// Act
			var result = settings.GetAllMailSettingsAsync(limit, offset, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("BCC", result[0].Title);
		}

		[TestMethod]
		public void GetAllPartnerSettings()
		{
			// Arrange
			var limit = 15;
			var offset = 3;

			var apiResponse = @"{
				'result': [
					{
						'name': 'new_relic',
						'title': 'New Relic',
						'description': 'lorem ipsum... .',
						'enabled': true
					}
				]
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync("/partner_settings?limit=" + limit + "&offset=" + offset, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var settings = new Settings(mockClient.Object);

			// Act
			var result = settings.GetAllPartnerSettingsAsync(limit, offset, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("New Relic", result[0].Title);
		}

		[TestMethod]
		public void GetAllTrackingSettings()
		{
			// Arrange
			var limit = 15;
			var offset = 3;

			var apiResponse = @"{
				'result': [
					{
						'name': 'open',
						'title': 'Open Tracking',
						'description': 'lorem ipsum... .',
						'enabled': true
					}
				]
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync("/tracking_settings?limit=" + limit + "&offset=" + offset, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var settings = new Settings(mockClient.Object);

			// Act
			var result = settings.GetAllTrackingSettingsAsync(limit, offset, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("Open Tracking", result[0].Title);
		}
	}
}
