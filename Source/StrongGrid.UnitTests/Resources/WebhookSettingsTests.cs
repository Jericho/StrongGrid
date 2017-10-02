using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.Resources;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class WebhookSettingsTests
	{
		#region FIELDS

		private const string EVENT_ENDPOINT = "user/webhooks/event";
		private const string INBOUNDPARSE_ENDPOINT = "user/webhooks/parse";

		private const string SINGLE_EVENT_WEBHOOK_SETTING_JSON = @"{
			'enabled': true,
			'url': 'url',
			'group_resubscribe': true,
			'delivered': true,
			'group_unsubscribe': true,
			'spam_report': true,
			'bounce': true,
			'deferred': true,
			'unsubscribe': true,
			'processed': true,
			'open': true,
			'click': true,
			'dropped': true
		}";

		private const string SINGLE_INBOUNDPARSE_WEBHOOK_SETTING_JSON = @"{
			'hostname': 'myhostname.com',
			'url': 'http://email.myhosthame.com',
			'spam_check': true,
			'send_raw': false
		}";

		#endregion

		[Fact]
		public void Parse_event_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<EventWebhookSettings>(SINGLE_EVENT_WEBHOOK_SETTING_JSON);

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
			var result = JsonConvert.DeserializeObject<InboundParseWebhookSettings>(SINGLE_INBOUNDPARSE_WEBHOOK_SETTING_JSON);

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
				'enabled': true,
				'url': 'url',
				'group_resubscribe': true,
				'delivered': true,
				'group_unsubscribe': true,
				'spam_report': true,
				'bounce': true,
				'deferred': true,
				'unsubscribe': true,
				'processed': true,
				'open': true,
				'click': true,
				'dropped': true
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(EVENT_ENDPOINT, "settings")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var webhooks = new WebhookSettings(client);

			// Act
			var result = await webhooks.GetEventWebhookSettingsAsync(CancellationToken.None).ConfigureAwait(false);

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

			var apiResponse = @"{
				'enabled': true,
				'url': 'url',
				'group_resubscribe': true,
				'delivered': true,
				'group_unsubscribe': true,
				'spam_report': true,
				'bounce': true,
				'deferred': true,
				'unsubscribe': true,
				'processed': true,
				'open': true,
				'click': true,
				'dropped': true
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(EVENT_ENDPOINT, "settings")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var webhooks = new WebhookSettings(client);

			// Act
			var result = await webhooks.UpdateEventWebhookSettingsAsync(enabled, url, bounce, click, deferred, delivered, dropped, groupResubscribe, groupUnsubscribe, open, processed, spamReport, unsubscribe, CancellationToken.None).ConfigureAwait(false);

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

			var client = Utils.GetFluentClient(mockHttp);
			var webhooks = new WebhookSettings(client);

			// Act
			await webhooks.SendEventTestAsync(url, CancellationToken.None).ConfigureAwait(false);

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

			var client = Utils.GetFluentClient(mockHttp);
			var webhooks = new WebhookSettings(client);

			// Act
			var result = await webhooks.GetInboundParseWebhookSettings(hostname, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetAllInboundParseWebhookSettings()
		{
			// Arrange
			var apiResponse = "{'result':[" +
				SINGLE_INBOUNDPARSE_WEBHOOK_SETTING_JSON + "," +
				SINGLE_INBOUNDPARSE_WEBHOOK_SETTING_JSON +
			"]}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(INBOUNDPARSE_ENDPOINT, "settings")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var webhooks = new WebhookSettings(client);

			// Act
			var result = await webhooks.GetAllInboundParseWebhookSettings(CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}
	}
}
