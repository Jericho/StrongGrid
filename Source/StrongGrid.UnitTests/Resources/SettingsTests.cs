using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
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
		public void GetEnforcedTlsSettings()
		{
			// Arrange
			var apiResponse = @"{
				'require_tls': true,
				'require_valid_cert': false
			}";

			_mockClient
				.Setup(c => c.GetAsync("/user/settings/enforced_tls", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.GetEnforcedTlsSettingsAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.RequireTls);
			Assert.AreEqual(false, result.RequireValidCertificate);
		}

		[TestMethod]
		public void UpdateEnforcedTlsSettings()
		{
			// Arrange
			var requireTls = true;
			var requireValidCert = true;

			var apiResponse = @"{
				'require_tls': true,
				'require_valid_cert': true
			}";

			_mockClient
				.Setup(c => c.PatchAsync("/user/settings/enforced_tls", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.UpdateEnforcedTlsSettingsAsync(requireTls, requireValidCert, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.RequireTls);
			Assert.AreEqual(true, result.RequireValidCertificate);
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
		public void GetNewRelicSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
				'license_key': 'key'
			}";

			_mockClient
				.Setup(c => c.GetAsync("/partner_settings/new_relic", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.GetNewRelicSettingsAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Enabled);
			Assert.AreEqual("key", result.LicenseKey);
		}

		[TestMethod]
		public void UpdateNewRelicSettings()
		{
			// Arrange
			var enabled = true;
			var licenseKey = "abc123";

			var apiResponse = @"{
				'enabled': true,
				'license_key': 'abc123'
			}";

			_mockClient
				.Setup(c => c.PatchAsync("/partner_settings/new_relic", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.UpdateNewRelicSettingsAsync(enabled, licenseKey, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Enabled);
			Assert.AreEqual("abc123", result.LicenseKey);
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

		[TestMethod]
		public void GetClickTrackingSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
			}";

			_mockClient
				.Setup(c => c.GetAsync("/tracking_settings/click", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.GetClickTrackingSettingsAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void UpdateClickTrackingSettings()
		{
			// Arrange
			var enabled = true;

			var apiResponse = @"{
				'enabled': true,
			}";

			_mockClient
				.Setup(c => c.PatchAsync("/tracking_settings/click", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.UpdateClickTrackingSettingsAsync(enabled, CancellationToken.None).Result;

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void GetGoogleAnalyticsGlobalSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
				'utm_source': 'sendgrid.com',
				'utm_medium': 'email',
				'utm_term': '',
				'utm_content': '',
				'utm_campaign': 'website'
			}";

			_mockClient
				.Setup(c => c.GetAsync("/tracking_settings/google_analytics", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.GetGoogleAnalyticsGlobalSettingsAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Enabled);
			Assert.AreEqual("website", result.UtmCampaign);
			Assert.AreEqual("", result.UtmContent);
			Assert.AreEqual("email", result.UtmMedium);
			Assert.AreEqual("sendgrid.com", result.UtmSource);
			Assert.AreEqual("", result.UtmTerm);
		}

		[TestMethod]
		public void UpdateGoogleAnalyticsGlobal()
		{
			// Arrange
			var enabled = true;
			var utmSource = "sendgrid.com";
			var utmMedium = "email";
			var utmTerm = "";
			var utmContent = "";
			var utmCampaign = "website";

			var apiResponse = @"{
				'enabled': true,
				'utm_source': 'sendgrid.com',
				'utm_medium': 'email',
				'utm_term': '',
				'utm_content': '',
				'utm_campaign': 'website'
			}";

			_mockClient
				.Setup(c => c.PatchAsync("/tracking_settings/google_analytics", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.UpdateGoogleAnalyticsGlobalSettingsAsync(enabled, utmSource, utmMedium, utmTerm, utmContent, utmCampaign, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetOpenTrackingSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
			}";

			_mockClient
				.Setup(c => c.GetAsync("/tracking_settings/open", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.GetOpenTrackingSettingsAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void UpdateOpenTrackingSettings()
		{
			// Arrange
			var enabled = true;

			var apiResponse = @"{
				'enabled': true,
			}";

			_mockClient
				.Setup(c => c.PatchAsync("/tracking_settings/open", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.UpdateOpenTrackingSettingsAsync(enabled, CancellationToken.None).Result;

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void GetSubscriptionTrackingSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
				'landing': 'landing page html',
				'url': 'url',
				'replace': 'replacement tag',
				'html_content': 'html content',
				'plain_content': 'text content'
			}";

			_mockClient
				.Setup(c => c.GetAsync("/tracking_settings/subscription", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.GetSubscriptionTrackingSettingsAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Enabled);
			Assert.AreEqual("html content", result.HtmlContent);
			Assert.AreEqual("landing page html", result.LandingPageHtml);
			Assert.AreEqual("replacement tag", result.ReplacementTag);
			Assert.AreEqual("text content", result.TextContent);
			Assert.AreEqual("url", result.Url);
		}

		[TestMethod]
		public void UpdateSubscriptionTrackingSettings()
		{
			// Arrange
			var enabled = true;
			var landingPageHtml = "landing page html";
			var url = "url";
			var replacementTag = "replacement tag";
			var htmlContent = "html content";
			var textContent = "text content";

			var apiResponse = @"{
				'enabled': true,
				'landing': 'landing page html',
				'url': 'url',
				'replace': 'replacement tag',
				'html_content': 'html content',
				'plain_content': 'text content'
			}";

			_mockClient
				.Setup(c => c.PatchAsync("/tracking_settings/subscription", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.UpdateSubscriptionTrackingSettingsAsync(enabled, landingPageHtml, url, replacementTag, htmlContent, textContent, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetBccMailSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
				'email': 'email@example.com'
			}";

			_mockClient
				.Setup(c => c.GetAsync("/mail_settings/bcc", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.GetBccMailSettingsAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Enabled);
			Assert.AreEqual("email@example.com", result.EmailAddress);
		}

		[TestMethod]
		public void UpdateBccMailSettings()
		{
			// Arrange
			var enabled = true;
			var email = "email@example.com";

			var apiResponse = @"{
				'enabled': true,
				'email': 'email@example.com'
			}";

			_mockClient
				.Setup(c => c.PatchAsync("/mail_settings/bcc", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.UpdateBccMailSettingsAsync(enabled, email, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetAddressWhitelistMailSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
				'list': [
					'email1@example.com',
					'example.com'
				]
			}";

			_mockClient
				.Setup(c => c.GetAsync("/mail_settings/address_whitelist", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.GetAddressWhitelistMailSettingsAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Enabled);
			Assert.IsNotNull(result.EmailAddresses);
			Assert.AreEqual(2, result.EmailAddresses.Length);
			Assert.AreEqual("email1@example.com", result.EmailAddresses[0]);
			Assert.AreEqual("example.com", result.EmailAddresses[1]);
		}

		[TestMethod]
		public void UpdateAddressWhitelistMailSettings()
		{
			// Arrange
			var enabled = true;
			var emailAddresses = new[] { "email@example.com", "example.com" };

			var apiResponse = @"{
				'enabled': true,
				'list': [
					'email1@example.com',
					'example.com'
				]
			}";

			_mockClient
				.Setup(c => c.PatchAsync("/mail_settings/address_whitelist", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.UpdateAddressWhitelistMailSettingsAsync(enabled, emailAddresses, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetFooterMailSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
				'html_content': '... 123 ...',
				'plain_content': '... abc ...'
			}";

			_mockClient
				.Setup(c => c.GetAsync("/mail_settings/footer", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.GetFooterMailSettingsAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Enabled);
			Assert.AreEqual("... 123 ...", result.HtmlContent);
			Assert.AreEqual("... abc ...", result.TextContent);
		}

		[TestMethod]
		public void UpdateFooterMailSettings()
		{
			// Arrange
			var enabled = true;
			var htmlContent = "html content";
			var textContent = "text content";

			var apiResponse = @"{
				'enabled': true,
				'html_content': 'html content',
				'plain_content': 'text content'
			}";

			_mockClient
				.Setup(c => c.PatchAsync("/mail_settings/footer", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.UpdateFooterMailSettingsAsync(enabled, htmlContent, textContent, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetForwardSpamMailSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
				'email': 'email address'
			}";

			_mockClient
				.Setup(c => c.GetAsync("/mail_settings/forward_spam", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.GetForwardSpamMailSettingsAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Enabled);
			Assert.AreEqual("email address", result.EmailAddress);
		}

		[TestMethod]
		public void UpdateForwardSpamMailSettings()
		{
			// Arrange
			var enabled = true;
			var email = "email address";

			var apiResponse = @"{
				'enabled': true,
				'email': 'email address'
			}";

			_mockClient
				.Setup(c => c.PatchAsync("/mail_settings/forward_spam", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.UpdateForwardSpamMailSettingsAsync(enabled, email, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetPlainContentMailSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
			}";

			_mockClient
				.Setup(c => c.GetAsync("/mail_settings/plain_content", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.GetPlainContentMailSettingsAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void UpdatPlainContentMailSettings()
		{
			// Arrange
			var enabled = true;

			var apiResponse = @"{
				'enabled': true,
			}";

			_mockClient
				.Setup(c => c.PatchAsync("/mail_settings/plain_content", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.UpdatPlainContentMailSettingsAsync(enabled, CancellationToken.None).Result;

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void GetSpamCheckMailSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
				'url': 'url',
				'max_score': 5
			}";

			_mockClient
				.Setup(c => c.GetAsync("/mail_settings/spam_check", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.GetSpamCheckMailSettingsAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Enabled);
			Assert.AreEqual("url", result.Url);
			Assert.AreEqual(5, result.Threshold);
		}

		[TestMethod]
		public void UpdateSpamCheckMailSettings()
		{
			// Arrange
			var enabled = true;
			var postToUrl = "url";
			var threshold = 5;

			var apiResponse = @"{
				'enabled': true,
				'url': 'url',
				'max_score': 5
			}";

			_mockClient
				.Setup(c => c.PatchAsync("/mail_settings/spam_check", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.UpdateSpamCheckMailSettingsAsync(enabled, postToUrl, threshold, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetTemplateMailSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
				'html_content': ' <% body %> '
			}";

			_mockClient
				.Setup(c => c.GetAsync("/mail_settings/template", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.GetTemplateMailSettingsAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Enabled);
			Assert.AreEqual(" <% body %> ", result.HtmlContent);
		}

		[TestMethod]
		public void UpdateTemplateMailSettings()
		{
			// Arrange
			var enabled = true;
			var htmlContent = "' <% body %> ";

			var apiResponse = @"{
				'enabled': true,
				'html_content': ' <% body %> '
			}";

			_mockClient
				.Setup(c => c.PatchAsync("/mail_settings/template", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.UpdateTemplateMailSettingsAsync(enabled, htmlContent, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetBouncePurgeMailSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
				'hard_bounces': 5,
				'soft_bounces': 5
			}";

			_mockClient
				.Setup(c => c.GetAsync("/mail_settings/bounce_purge", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.GetBouncePurgeMailSettingsAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Enabled);
			Assert.AreEqual(5, result.HardBounces);
			Assert.AreEqual(5, result.SoftBounces);
		}

		[TestMethod]
		public void UpdatBouncePurgeMailSettings()
		{
			// Arrange
			var enabled = true;
			var hardBounces = 5;
			var softBounces = 5;

			var apiResponse = @"{
				'enabled': true,
				'hard_bounces': 5,
				'soft_bounces': 5
			}";

			_mockClient
				.Setup(c => c.PatchAsync("/mail_settings/bounce_purge", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.UpdatBouncePurgeMailSettingsAsync(enabled, hardBounces, softBounces, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetForwardBounceMailSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
				'email': 'email address'
			}";

			_mockClient
				.Setup(c => c.GetAsync("/mail_settings/forward_bounce", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.GetForwardBounceMailSettingsAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Enabled);
			Assert.AreEqual("email address", result.EmailAddress);
		}

		[TestMethod]
		public void UpdatForwardBounceMailSettings()
		{
			// Arrange
			var enabled = true;
			var email = "email address";

			var apiResponse = @"{
				'enabled': true,
				'email': 'email address'
			}";

			_mockClient
				.Setup(c => c.PatchAsync("/mail_settings/forward_bounce", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = CreateSettings();

			// Act
			var result = settings.UpdateForwardBounceMailSettingsAsync(enabled, email, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}
	}
}
