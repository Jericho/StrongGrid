using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Resources;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class SettingsTests
	{
		internal const string SINGLE_GLOBAL_SETTING_JSON = @"{
			""name"": ""bcc"",
			""title"": ""BCC"",
			""description"": ""lorem ipsum... ."",
			""enabled"": true
		}";

		private readonly ITestOutputHelper _outputHelper;

		public SettingsTests(ITestOutputHelper outputHelper)
		{
			_outputHelper = outputHelper;
		}

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<GlobalSetting>(SINGLE_GLOBAL_SETTING_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.Description.ShouldBe("lorem ipsum... .");
			result.Enabled.ShouldBe(true);
			result.Name.ShouldBe("bcc");
			result.Title.ShouldBe("BCC");
		}

		[Fact]
		public async Task GetEnforcedTlsSettingsAsync()
		{
			// Arrange
			var apiResponse = @"{
				""require_tls"": true,
				""require_valid_cert"": false
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("user/settings/enforced_tls")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.GetEnforcedTlsSettingsAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.RequireTls.ShouldBe(true);
			result.RequireValidCertificate.ShouldBe(false);
		}

		[Fact]
		public async Task UpdateEnforcedTlsSettingsAsync()
		{
			// Arrange
			var requireTls = true;
			var requireValidCert = true;

			var apiResponse = @"{
				""require_tls"": true,
				""require_valid_cert"": true
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri("user/settings/enforced_tls")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.UpdateEnforcedTlsSettingsAsync(requireTls, requireValidCert, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.RequireTls.ShouldBe(true);
			result.RequireValidCertificate.ShouldBe(true);
		}

		[Fact]
		public async Task GetAllMailSettingsAsync()
		{
			// Arrange
			var limit = 15;
			var offset = 3;

			var apiResponse = @"{
				""result"": [
					{
						""name"": ""bcc"",
						""title"": ""BCC"",
						""description"": ""lorem ipsum... ."",
						""enabled"": true
					}
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri($"mail_settings?limit={limit}&offset={offset}")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.GetAllMailSettingsAsync(limit, offset, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].Title.ShouldBe("BCC");
		}

		[Fact]
		public async Task GetAllPartnerSettingsAsync()
		{
			// Arrange
			var limit = 15;
			var offset = 3;

			var apiResponse = @"{
				""result"": [
					{
						""name"": ""foo_bar"",
						""title"": ""Foo Bar"",
						""description"": ""lorem ipsum... ."",
						""enabled"": true
					}
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri($"partner_settings?limit={limit}&offset={offset}")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.GetAllPartnerSettingsAsync(limit, offset, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].Title.ShouldBe("Foo Bar");
		}

		[Fact]
		public async Task GetNewRelicSettingsAsync()
		{
			// Arrange
			var apiResponse = @"{
				""enabled"": true,
				""license_key"": ""key""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("partner_settings/new_relic")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.GetNewRelicSettingsAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.LicenseKey.ShouldBe("key");
		}

		[Fact]
		public async Task UpdateNewRelicSettings()
		{
			// Arrange
			var enabled = true;
			var licenseKey = "abc123";

			var apiResponse = @"{
				""enabled"": true,
				""license_key"": ""abc123""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri("partner_settings/new_relic")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.UpdateNewRelicSettingsAsync(enabled, licenseKey, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.LicenseKey.ShouldBe("abc123");
		}

		[Fact]
		public async Task GetAllTrackingSettingsAsync()
		{
			// Arrange
			var limit = 15;
			var offset = 3;

			var apiResponse = @"{
				""result"": [
					{
						""name"": ""open"",
						""title"": ""Open Tracking"",
						""description"": ""lorem ipsum... ."",
						""enabled"": true
					}
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri($"tracking_settings?limit={limit}&offset={offset}")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.GetAllTrackingSettingsAsync(limit, offset, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].Title.ShouldBe("Open Tracking");
		}

		[Fact]
		public async Task GetClickTrackingSettingsAsync()
		{
			// Arrange
			var apiResponse = @"{
				""enable_text"": true,
				""enabled"": false
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("tracking_settings/click")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.GetClickTrackingSettingsAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.EnabledInTextContent.ShouldBeTrue();
			result.EnabledInHtmlContent.ShouldBeFalse();
		}

		[Fact]
		public async Task UpdateClickTrackingSettingsAsync()
		{
			// Arrange
			var enabledInText = false;
			var enabledInHtml = true;

			var apiResponse = @"{
				""enable_text"": false,
				""enabled"": true
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri("tracking_settings/click")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.UpdateClickTrackingSettingsAsync(enabledInText, enabledInHtml, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.EnabledInTextContent.ShouldBeFalse();
			result.EnabledInHtmlContent.ShouldBeTrue();
		}

		[Fact]
		public async Task GetGoogleAnalyticsGlobalSettingsAsync()
		{
			// Arrange
			var apiResponse = @"{
				""enabled"": true,
				""utm_source"": ""sendgrid.com"",
				""utm_medium"": ""email"",
				""utm_term"": """",
				""utm_content"": """",
				""utm_campaign"": ""website""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("tracking_settings/google_analytics")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.GetGoogleAnalyticsGlobalSettingsAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.UtmCampaign.ShouldBe("website");
			result.UtmContent.ShouldBe("");
			result.UtmMedium.ShouldBe("email");
			result.UtmSource.ShouldBe("sendgrid.com");
			result.UtmTerm.ShouldBe("");
		}

		[Fact]
		public async Task UpdateGoogleAnalyticsGlobalAsync()
		{
			// Arrange
			var enabled = true;
			var utmSource = "sendgrid.com";
			var utmMedium = "email";
			var utmTerm = "";
			var utmContent = "";
			var utmCampaign = "website";

			var apiResponse = @"{
				""enabled"": true,
				""utm_source"": ""sendgrid.com"",
				""utm_medium"": ""email"",
				""utm_term"": """",
				""utm_content"": """",
				""utm_campaign"": ""website""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri("tracking_settings/google_analytics")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.UpdateGoogleAnalyticsGlobalSettingsAsync(enabled, utmSource, utmMedium, utmTerm, utmContent, utmCampaign, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetOpenTrackingSettingsAsync()
		{
			// Arrange
			var apiResponse = @"{
				""enabled"": true
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("tracking_settings/open")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.GetOpenTrackingSettingsAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBeTrue();
		}

		[Fact]
		public async Task UpdateOpenTrackingSettingsAsync()
		{
			// Arrange
			var enabled = true;

			var apiResponse = @"{
				""enabled"": true
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri("tracking_settings/open")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.UpdateOpenTrackingSettingsAsync(enabled, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBeTrue();
		}

		[Fact]
		public async Task GetSubscriptionTrackingSettingsAsync()
		{
			// Arrange
			var apiResponse = @"{
				""enabled"": true,
				""landing"": ""landing page html"",
				""url"": ""url"",
				""replace"": ""replacement tag"",
				""html_content"": ""html content"",
				""plain_content"": ""text content""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("tracking_settings/subscription")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.GetSubscriptionTrackingSettingsAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.HtmlContent.ShouldBe("html content");
			result.LandingPageHtml.ShouldBe("landing page html");
			result.ReplacementTag.ShouldBe("replacement tag");
			result.TextContent.ShouldBe("text content");
			result.Url.ShouldBe("url");
		}

		[Fact]
		public async Task UpdateSubscriptionTrackingSettingsAsync()
		{
			// Arrange
			var enabled = true;
			var landingPageHtml = "landing page html";
			var url = "url";
			var replacementTag = "replacement tag";
			var htmlContent = "html content";
			var textContent = "text content";

			var apiResponse = @"{
				""enabled"": true,
				""landing"": ""landing page html"",
				""url"": ""url"",
				""replace"": ""replacement tag"",
				""html_content"": ""html content"",
				""plain_content"": ""text content""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri("tracking_settings/subscription")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.UpdateSubscriptionTrackingSettingsAsync(enabled, landingPageHtml, url, replacementTag, htmlContent, textContent, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetAddressWhitelistMailSettingsAsync()
		{
			// Arrange
			var apiResponse = @"{
				""enabled"": true,
				""list"": [
					""email1@example.com"",
					""example.com""
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("mail_settings/address_whitelist")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.GetAddressWhitelistMailSettingsAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.EmailAddresses.ShouldNotBeNull();
			result.EmailAddresses.Length.ShouldBe(2);
			result.EmailAddresses[0].ShouldBe("email1@example.com");
			result.EmailAddresses[1].ShouldBe("example.com");
		}

		[Fact]
		public async Task UpdateAddressWhitelistMailSettingsAsync()
		{
			// Arrange
			var enabled = true;
			var emailAddresses = new[] { "email@example.com", "example.com" };

			var apiResponse = @"{
				""enabled"": true,
				""list"": [
					""email1@example.com"",
					""example.com""
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri("mail_settings/address_whitelist")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.UpdateAddressWhitelistMailSettingsAsync(enabled, emailAddresses, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetFooterMailSettingsAsync()
		{
			// Arrange
			var apiResponse = @"{
				""enabled"": true,
				""html_content"": ""... 123 ..."",
				""plain_content"": ""... abc ...""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("mail_settings/footer")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.GetFooterMailSettingsAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.HtmlContent.ShouldBe("... 123 ...");
			result.TextContent.ShouldBe("... abc ...");
		}

		[Fact]
		public async Task UpdateFooterMailSettingsAsync()
		{
			// Arrange
			var enabled = true;
			var htmlContent = "html content";
			var textContent = "text content";

			var apiResponse = @"{
				""enabled"": true,
				""html_content"": ""html content"",
				""plain_content"": ""text content""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri("mail_settings/footer")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.UpdateFooterMailSettingsAsync(enabled, htmlContent, textContent, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetForwardSpamMailSettingsAsync()
		{
			// Arrange
			var apiResponse = @"{
				""enabled"": true,
				""email"": ""email address""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("mail_settings/forward_spam")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.GetForwardSpamMailSettingsAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.EmailAddress.ShouldBe("email address");
		}

		[Fact]
		public async Task UpdateForwardSpamMailSettingsAsync()
		{
			// Arrange
			var enabled = true;
			var email = "email address";

			var apiResponse = @"{
				""enabled"": true,
				""email"": ""email address""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri("mail_settings/forward_spam")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.UpdateForwardSpamMailSettingsAsync(enabled, email, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetPlainContentMailSettingsAsync()
		{
			// Arrange
			var apiResponse = @"{
				""enabled"": true
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("mail_settings/plain_content")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.GetPlainContentMailSettingsAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBeTrue();
		}

		[Fact]
		public async Task UpdatPlainContentMailSettingsAsync()
		{
			// Arrange
			var enabled = true;

			var apiResponse = @"{
				""enabled"": true
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri("mail_settings/plain_content")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.UpdatePlainContentMailSettingsAsync(enabled, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBeTrue();
		}

		[Fact]
		public async Task GetTemplateMailSettingsAsync()
		{
			// Arrange
			var apiResponse = @"{
				""enabled"": true,
				""html_content"": "" <% body %> ""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("mail_settings/template")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.GetTemplateMailSettingsAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.HtmlContent.ShouldBe(" <% body %> ");
		}

		[Fact]
		public async Task UpdateTemplateMailSettingsAsync()
		{
			// Arrange
			var enabled = true;
			var htmlContent = " <% body %> ";

			var apiResponse = @"{
				""enabled"": true,
				""html_content"": "" <% body %> ""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri("mail_settings/template")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.UpdateTemplateMailSettingsAsync(enabled, htmlContent, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetBouncePurgeMailSettingsAsync()
		{
			// Arrange
			var apiResponse = @"{
				""enabled"": true,
				""hard_bounces"": 5,
				""soft_bounces"": 5
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("mail_settings/bounce_purge")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.GetBouncePurgeMailSettingsAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.HardBounces.ShouldBe(5);
			result.SoftBounces.ShouldBe(5);
		}

		[Fact]
		public async Task UpdateBouncePurgeMailSettingsAsync()
		{
			// Arrange
			var enabled = true;
			var hardBounces = 5;
			var softBounces = 5;

			var apiResponse = @"{
				""enabled"": true,
				""hard_bounces"": 5,
				""soft_bounces"": 5
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri("mail_settings/bounce_purge")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.UpdateBouncePurgeMailSettingsAsync(enabled, hardBounces, softBounces, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetForwardBounceMailSettingsAsync()
		{
			// Arrange
			var apiResponse = @"{
				""enabled"": true,
				""email"": ""email address""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("mail_settings/forward_bounce")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.GetForwardBounceMailSettingsAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Enabled.ShouldBe(true);
			result.EmailAddress.ShouldBe("email address");
		}

		[Fact]
		public async Task UpdatForwardBounceMailSettingsAsync()
		{
			// Arrange
			var enabled = true;
			var email = "email address";

			var apiResponse = @"{
				""enabled"": true,
				""email"": ""email address""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri("mail_settings/forward_bounce")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var settings = new Settings(client);

			// Act
			var result = await settings.UpdateForwardBounceMailSettingsAsync(enabled, email, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}
	}
}
