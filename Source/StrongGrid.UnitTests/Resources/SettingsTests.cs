using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using StrongGrid.Model;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class SettingsTests
	{
		#region FIELDS

		private const string ENDPOINT = "/alerts";

		private const string SINGLE_GLOBAL_SETTING_JSON = @"{
			'name': 'bcc',
			'title': 'BCC',
			'description': 'lorem ipsum... .',
			'enabled': true
		}";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<GlobalSetting>(SINGLE_GLOBAL_SETTING_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.Description.ShouldBe("lorem ipsum... .");
			result.Enabled.ShouldBe(true);
			result.Name.ShouldBe("bcc");
			result.Title.ShouldBe("BCC");
		}

		[Fact]
		public void GetEnforcedTlsSettings()
		{
			// Arrange
			var apiResponse = @"{
				'require_tls': true,
				'require_valid_cert': false
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync("/user/settings/enforced_tls", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.GetEnforcedTlsSettingsAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.RequireTls.ShouldBe(true);
			result.RequireValidCertificate.ShouldBe(false);
		}

		[Fact]
		public void UpdateEnforcedTlsSettings()
		{
			// Arrange
			var requireTls = true;
			var requireValidCert = true;

			var apiResponse = @"{
				'require_tls': true,
				'require_valid_cert': true
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync("/user/settings/enforced_tls", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.UpdateEnforcedTlsSettingsAsync(requireTls, requireValidCert, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.RequireTls.ShouldBe(true);
			result.RequireValidCertificate.ShouldBe(true);
		}

		[Fact]
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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync("/mail_settings?limit=" + limit + "&offset=" + offset, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.GetAllMailSettingsAsync(limit, offset, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].Title.ShouldBe("BCC");
		}

		[Fact]
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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync("/partner_settings?limit=" + limit + "&offset=" + offset, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.GetAllPartnerSettingsAsync(limit, offset, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].Title.ShouldBe("New Relic");
		}

		[Fact]
		public void GetNewRelicSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
				'license_key': 'key'
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync("/partner_settings/new_relic", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.GetNewRelicSettingsAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.LicenseKey.ShouldBe("key");
		}

		[Fact]
		public void UpdateNewRelicSettings()
		{
			// Arrange
			var enabled = true;
			var licenseKey = "abc123";

			var apiResponse = @"{
				'enabled': true,
				'license_key': 'abc123'
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync("/partner_settings/new_relic", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.UpdateNewRelicSettingsAsync(enabled, licenseKey, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.LicenseKey.ShouldBe("abc123");
		}

		[Fact]
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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync("/tracking_settings?limit=" + limit + "&offset=" + offset, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.GetAllTrackingSettingsAsync(limit, offset, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].Title.ShouldBe("Open Tracking");
		}

		[Fact]
		public void GetClickTrackingSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync("/tracking_settings/click", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.GetClickTrackingSettingsAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldBeTrue();
		}

		[Fact]
		public void UpdateClickTrackingSettings()
		{
			// Arrange
			var enabled = true;

			var apiResponse = @"{
				'enabled': true,
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync("/tracking_settings/click", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.UpdateClickTrackingSettingsAsync(enabled, CancellationToken.None).Result;

			// Assert
			result.ShouldBeTrue();
		}

		[Fact]
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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync("/tracking_settings/google_analytics", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.GetGoogleAnalyticsGlobalSettingsAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.UtmCampaign.ShouldBe("website");
			result.UtmContent.ShouldBe("");
			result.UtmMedium.ShouldBe("email");
			result.UtmSource.ShouldBe("sendgrid.com");
			result.UtmTerm.ShouldBe("");
		}

		[Fact]
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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync("/tracking_settings/google_analytics", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.UpdateGoogleAnalyticsGlobalSettingsAsync(enabled, utmSource, utmMedium, utmTerm, utmContent, utmCampaign, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetOpenTrackingSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync("/tracking_settings/open", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.GetOpenTrackingSettingsAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldBeTrue();
		}

		[Fact]
		public void UpdateOpenTrackingSettings()
		{
			// Arrange
			var enabled = true;

			var apiResponse = @"{
				'enabled': true,
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync("/tracking_settings/open", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.UpdateOpenTrackingSettingsAsync(enabled, CancellationToken.None).Result;

			// Assert
			result.ShouldBeTrue();
		}

		[Fact]
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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync("/tracking_settings/subscription", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.GetSubscriptionTrackingSettingsAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.HtmlContent.ShouldBe("html content");
			result.LandingPageHtml.ShouldBe("landing page html");
			result.ReplacementTag.ShouldBe("replacement tag");
			result.TextContent.ShouldBe("text content");
			result.Url.ShouldBe("url");
		}

		[Fact]
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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync("/tracking_settings/subscription", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.UpdateSubscriptionTrackingSettingsAsync(enabled, landingPageHtml, url, replacementTag, htmlContent, textContent, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetBccMailSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
				'email': 'email@example.com'
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync("/mail_settings/bcc", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.GetBccMailSettingsAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.EmailAddress.ShouldBe("email@example.com");
		}

		[Fact]
		public void UpdateBccMailSettings()
		{
			// Arrange
			var enabled = true;
			var email = "email@example.com";

			var apiResponse = @"{
				'enabled': true,
				'email': 'email@example.com'
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync("/mail_settings/bcc", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.UpdateBccMailSettingsAsync(enabled, email, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync("/mail_settings/address_whitelist", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.GetAddressWhitelistMailSettingsAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.EmailAddresses.ShouldNotBeNull();
			result.EmailAddresses.Length.ShouldBe(2);
			result.EmailAddresses[0].ShouldBe("email1@example.com");
			result.EmailAddresses[1].ShouldBe("example.com");
		}

		[Fact]
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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync("/mail_settings/address_whitelist", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.UpdateAddressWhitelistMailSettingsAsync(enabled, emailAddresses, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetFooterMailSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
				'html_content': '... 123 ...',
				'plain_content': '... abc ...'
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync("/mail_settings/footer", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.GetFooterMailSettingsAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.HtmlContent.ShouldBe("... 123 ...");
			result.TextContent.ShouldBe("... abc ...");
		}

		[Fact]
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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync("/mail_settings/footer", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.UpdateFooterMailSettingsAsync(enabled, htmlContent, textContent, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetForwardSpamMailSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
				'email': 'email address'
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync("/mail_settings/forward_spam", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.GetForwardSpamMailSettingsAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.EmailAddress.ShouldBe("email address");
		}

		[Fact]
		public void UpdateForwardSpamMailSettings()
		{
			// Arrange
			var enabled = true;
			var email = "email address";

			var apiResponse = @"{
				'enabled': true,
				'email': 'email address'
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync("/mail_settings/forward_spam", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.UpdateForwardSpamMailSettingsAsync(enabled, email, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetPlainContentMailSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync("/mail_settings/plain_content", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.GetPlainContentMailSettingsAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldBeTrue();
		}

		[Fact]
		public void UpdatPlainContentMailSettings()
		{
			// Arrange
			var enabled = true;

			var apiResponse = @"{
				'enabled': true,
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync("/mail_settings/plain_content", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.UpdatPlainContentMailSettingsAsync(enabled, CancellationToken.None).Result;

			// Assert
			result.ShouldBeTrue();
		}

		[Fact]
		public void GetSpamCheckMailSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
				'url': 'url',
				'max_score': 5
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync("/mail_settings/spam_check", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.GetSpamCheckMailSettingsAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.Url.ShouldBe("url");
			result.Threshold.ShouldBe(5);
		}

		[Fact]
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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync("/mail_settings/spam_check", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.UpdateSpamCheckMailSettingsAsync(enabled, postToUrl, threshold, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetTemplateMailSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
				'html_content': ' <% body %> '
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync("/mail_settings/template", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.GetTemplateMailSettingsAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.HtmlContent.ShouldBe(" <% body %> ");
		}

		[Fact]
		public void UpdateTemplateMailSettings()
		{
			// Arrange
			var enabled = true;
			var htmlContent = "' <% body %> ";

			var apiResponse = @"{
				'enabled': true,
				'html_content': ' <% body %> '
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync("/mail_settings/template", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.UpdateTemplateMailSettingsAsync(enabled, htmlContent, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetBouncePurgeMailSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
				'hard_bounces': 5,
				'soft_bounces': 5
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync("/mail_settings/bounce_purge", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.GetBouncePurgeMailSettingsAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.HardBounces.ShouldBe(5);
			result.SoftBounces.ShouldBe(5);
		}

		[Fact]
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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync("/mail_settings/bounce_purge", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.UpdatBouncePurgeMailSettingsAsync(enabled, hardBounces, softBounces, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetForwardBounceMailSettings()
		{
			// Arrange
			var apiResponse = @"{
				'enabled': true,
				'email': 'email address'
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync("/mail_settings/forward_bounce", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.GetForwardBounceMailSettingsAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.EmailAddress.ShouldBe("email address");
		}

		[Fact]
		public void UpdatForwardBounceMailSettings()
		{
			// Arrange
			var enabled = true;
			var email = "email address";

			var apiResponse = @"{
				'enabled': true,
				'email': 'email address'
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync("/mail_settings/forward_bounce", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var settings = new Settings(mockClient.Object, ENDPOINT);

			// Act
			var result = settings.UpdateForwardBounceMailSettingsAsync(enabled, email, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}
	}
}
