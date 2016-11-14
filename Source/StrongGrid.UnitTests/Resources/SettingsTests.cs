using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using StrongGrid.Model;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class SettingsTests
	{
		#region FIELDS

		private const string ENDPOINT = "/alerts";
		private MockRepository _mockRepository;
		private Mock<IClient> _mockClient;

		private const string SINGLE_GLOBAL_SETTING_JSON = @"{
			'name': 'bcc',
			'title': 'BCC',
			'description': 'lorem ipsum... .',
			'enabled': true
		}";

		#endregion

		private Settings CreateSettings()
		{
			return new Settings(_mockClient.Object, ENDPOINT);
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
			var result = JsonConvert.DeserializeObject<GlobalSetting>(SINGLE_GLOBAL_SETTING_JSON);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual("lorem ipsum... .", result.Description);
			Assert.AreEqual(true, result.Enabled);
			Assert.AreEqual("bcc", result.Name);
			Assert.AreEqual("BCC", result.Title);
		}

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

			_mockClient
				.Setup(c => c.GetAsync("/mail_settings?limit=" + limit + "&offset=" + offset, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

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

			_mockClient
				.Setup(c => c.GetAsync("/partner_settings?limit=" + limit + "&offset=" + offset, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

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

			_mockClient
				.Setup(c => c.GetAsync("/tracking_settings?limit=" + limit + "&offset=" + offset, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.GetAllTrackingSettingsAsync(limit, offset, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("Open Tracking", result[0].Title);
		}
	}
}
