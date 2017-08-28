using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.UnitTests;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class WebhookSettingsTests
	{
		#region FIELDS

		private const string ENDPOINT = "user/webhooks/event/settings";

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

		#endregion

		[Fact]
		public void Parse_json()
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
		public async Task GetAsync()
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
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("user/webhooks/event/settings")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var webhooks = new WebhookSettings(client);

			// Act
			var result = await webhooks.GetAsync(CancellationToken.None).ConfigureAwait(false);

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
		public async Task UpdateAsync()
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
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri("user/webhooks/event/settings")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var webhooks = new WebhookSettings(client);

			// Act
			var result = await webhooks.UpdateAsync(enabled, url, bounce, click, deferred, delivered, dropped, groupResubscribe, groupUnsubscribe, open, processed, spamReport, unsubscribe, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task SendTestAsync()
		{
			// Arrange
			var url = "url";
			
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("POST"), Utils.GetSendGridApiUri("user/webhooks/event/test")).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var webhooks = new WebhookSettings(client);

			// Act
			await webhooks.SendTestAsync(url, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

	}
}
