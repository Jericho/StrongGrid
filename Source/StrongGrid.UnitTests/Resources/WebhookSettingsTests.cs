using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Resources;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class WebhookSettingsTests
	{
		internal const string EVENT_ENDPOINT = "user/webhooks/event";
		internal const string INBOUNDPARSE_ENDPOINT = "user/webhooks/parse";

		internal const string SINGLE_EVENT_WEBHOOK_SETTING_JSON = @"{
			""enabled"": true,
			""url"": ""url"",
			""group_resubscribe"": true,
			""delivered"": true,
			""group_unsubscribe"": true,
			""spam_report"": true,
			""bounce"": true,
			""deferred"": true,
			""unsubscribe"": true,
			""processed"": true,
			""open"": true,
			""click"": true,
			""dropped"": true
		}";

		internal const string SINGLE_INBOUNDPARSE_WEBHOOK_SETTING_JSON = @"{
			""hostname"": ""myhostname.com"",
			""url"": ""http://email.myhosthame.com"",
			""spam_check"": true,
			""send_raw"": false
		}";

		private readonly ITestOutputHelper _outputHelper;

		public WebhookSettingsTests(ITestOutputHelper outputHelper)
		{
			_outputHelper = outputHelper;
		}

		[Fact]
		public void Parse_event_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<EventWebhookSettings>(SINGLE_EVENT_WEBHOOK_SETTING_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.Url.ShouldBe("url");
			result.GroupResubscribe.ShouldBe(true);
			result.Delivered.ShouldBe(true);
			result.GroupUnsubscribe.ShouldBe(true);
			result.SpamReport.ShouldBe(true);
			result.Bounce.ShouldBe(true);
			result.Deferred.ShouldBe(true);
			result.Unsubscribe.ShouldBe(true);
			result.Processed.ShouldBe(true);
			result.Open.ShouldBe(true);
			result.Click.ShouldBe(true);
			result.Dropped.ShouldBe(true);
		}

		[Fact]
		public void Parse_inboundparse_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<InboundParseWebhookSettings>(SINGLE_INBOUNDPARSE_WEBHOOK_SETTING_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.HostName.ShouldBe("myhostname.com");
			result.Url.ShouldBe("http://email.myhosthame.com");
			result.SpamCheck.ShouldBeTrue();
			result.SendRaw.ShouldBeFalse();
		}

		[Fact]
		public async Task GetEventWebhookSettingsAsync()
		{
			// Arrange

			var apiResponse = @"{
				""enabled"": true,
				""url"": ""url"",
				""group_resubscribe"": true,
				""delivered"": true,
				""group_unsubscribe"": true,
				""spam_report"": true,
				""bounce"": true,
				""deferred"": true,
				""unsubscribe"": true,
				""processed"": true,
				""open"": true,
				""click"": true,
				""dropped"": true
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(EVENT_ENDPOINT, "settings")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var webhooks = new WebhookSettings(client);

			// Act
			var result = await webhooks.GetEventWebhookSettingsAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Url.ShouldBe("url");
			result.GroupResubscribe.ShouldBe(true);
			result.Delivered.ShouldBe(true);
			result.GroupUnsubscribe.ShouldBe(true);
			result.SpamReport.ShouldBe(true);
			result.Bounce.ShouldBe(true);
			result.Deferred.ShouldBe(true);
			result.Unsubscribe.ShouldBe(true);
			result.Processed.ShouldBe(true);
			result.Open.ShouldBe(true);
			result.Click.ShouldBe(true);
			result.Dropped.ShouldBe(true);
		}

		[Fact]
		public async Task UpdateEventWebhookSettingsAsync()
		{
			// Arrange
			var enabled = true;
			var url = "url";
			var bounce = true;
			var click = true;
			var deferred = true;
			var delivered = true;
			var dropped = true;
			var groupResubscribe = true;
			var groupUnsubscribe = true;
			var open = true;
			var processed = true;
			var spamReport = true;
			var unsubscribe = true;
			var friendlyName = "My friendly name";

			var apiResponse = @"{
				""enabled"": true,
				""url"": ""url"",
				""group_resubscribe"": true,
				""delivered"": true,
				""group_unsubscribe"": true,
				""spam_report"": true,
				""bounce"": true,
				""deferred"": true,
				""unsubscribe"": true,
				""processed"": true,
				""open"": true,
				""click"": true,
				""dropped"": true,
				""friendly_name"": ""My friendly name""
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(EVENT_ENDPOINT, "settings")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var webhooks = new WebhookSettings(client);

			// Act
			var result = await webhooks.UpdateEventWebhookSettingsAsync(enabled, url, bounce, click, deferred, delivered, dropped, groupResubscribe, groupUnsubscribe, open, processed, spamReport, unsubscribe, friendlyName, null, null, null, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task SendEventTestAsync()
		{
			// Arrange
			var url = "url";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("POST"), Utils.GetSendGridApiUri(EVENT_ENDPOINT, "test")).Respond(HttpStatusCode.NoContent);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var webhooks = new WebhookSettings(client);

			// Act
			await webhooks.SendEventTestAsync(url, null, null, null, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task GetInboundParseWebhookSettings()
		{
			// Arrange
			var hostname = "myhostname.com";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(INBOUNDPARSE_ENDPOINT, "settings", hostname)).Respond("application/json", SINGLE_INBOUNDPARSE_WEBHOOK_SETTING_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var webhooks = new WebhookSettings(client);

			// Act
			var result = await webhooks.GetInboundParseWebhookSettingsAsync(hostname, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetAllInboundParseWebhookSettings()
		{
			// Arrange
			var limit = 25;
			var endpoint = Utils.GetSendGridApiUri(INBOUNDPARSE_ENDPOINT, "settings");

			// This is what the endpoint URL should be but we don't support limit and offset yet.
			// See: https://github.com/Jericho/StrongGrid/issues/368
			// var endpoint = Utils.GetSendGridApiUri(INBOUNDPARSE_ENDPOINT, "settings") + $"?limit={limit}&offset=0";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, endpoint).Respond((HttpRequestMessage request) =>
			{
				var response = new HttpResponseMessage(HttpStatusCode.OK);
				response.Headers.Add("Link", $"<{endpoint}>; rel=\"next\"; title=\"1\", <{endpoint}>; rel=\"prev\"; title=\"1\", <{endpoint}>; rel=\"last\"; title=\"1\", <{endpoint}>; rel=\"first\"; title=\"1\"");
				response.Content = new StringContent(
					"{\"result\":[" +
					SINGLE_INBOUNDPARSE_WEBHOOK_SETTING_JSON + "," +
					SINGLE_INBOUNDPARSE_WEBHOOK_SETTING_JSON +
					"]}");
				return response;
			});

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var webhooks = new WebhookSettings(client);

			// Act
			var result = await webhooks.GetAllInboundParseWebhookSettingsAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Records.Length.ShouldBe(2);
		}
	}
}
