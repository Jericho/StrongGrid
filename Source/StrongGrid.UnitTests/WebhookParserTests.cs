using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using Shouldly;
using StrongGrid.Model.Webhooks;
using StrongGrid.Utilities;
using System.IO;
using Xunit;

namespace StrongGrid.UnitTests
{
	public class WebhookParserTests
	{
		private const string PROCESSED_JSON = @"
		{
			'sg_event_id':'sendgrid_internal_event_id',
			'sg_message_id':'sendgrid_internal_message_id',
			'email':'email@example.com',
			'timestamp':1249948800,
			'smtp-id':'<original-smtp-id@domain.com>',
			'unique_arg_key':'unique_arg_value',
			'category':['category1', 'category2'],
			'event':'processed',
			'newsletter': {
				'newsletter_user_list_id': '10557865',
				'newsletter_id': '1943530',
				'newsletter_send_id': '2308608'
			},
			'asm_group_id': 1,
			'send_at':1249949000
		}";

		private const string BOUNCED_JSON = @"
		{
			'status':'5.0.0',
			'sg_event_id':'sendgrid_internal_event_id',
			'sg_message_id':'sendgrid_internal_message_id',
			'event':'bounce',
			'email':'email@example.com',
			'timestamp':1249948800,
			'smtp-id':'<original-smtp-id@domain.com>',
			'unique_arg_key':'unique_arg_value',
			'category':['category1', 'category2'],
			'newsletter': {
				'newsletter_user_list_id': '10557865',
				'newsletter_id': '1943530',
				'newsletter_send_id': '2308608'
			},
			'asm_group_id': 1,
			'reason':'500 No Such User',
			'type':'bounce',
			'ip' : '127.0.0.1',
			'tls' : '1',
			'cert_err' : '0'
		}";

		private const string DEFERRED_JSON = @"
		{
			'response':'400 Try again',
			'sg_event_id':'sendgrid_internal_event_id',
			'sg_message_id':'sendgrid_internal_message_id',
			'event':'deferred',
			'email':'email@example.com',
			'timestamp':1249948800,
			'smtp-id':'<original-smtp-id@domain.com>',
			'unique_arg_key':'unique_arg_value',
			'category':['category1', 'category2'],
			'attempt':'10',
			'newsletter': {
				'newsletter_user_list_id': '10557865',
				'newsletter_id': '1943530',
				'newsletter_send_id': '2308608'
			},
			'asm_group_id': 1,
			'ip' : '127.0.0.1',
			'tls' : '0',
			'cert_err' : '0'
		}";

		private const string DROPPED_JSON = @"
		{
			'sg_event_id':'sendgrid_internal_event_id',
			'sg_message_id':'sendgrid_internal_message_id',
			'email':'email@example.com',
			'timestamp':1249948800,
			'smtp-id':'<original-smtp-id@domain.com>',
			'unique_arg_key':'unique_arg_value',
			'category':['category1', 'category2'],
			'reason':'Bounced Address',
			'event':'dropped'
		}";

		private const string DELIVERED_JSON = @"
		{
			'response':'250 OK',
			'sg_event_id':'sendgrid_internal_event_id',
			'sg_message_id':'sendgrid_internal_message_id',
			'event':'delivered',
			'email':'email@example.com',
			'timestamp':1249948800,
			'smtp-id':'<original-smtp-id@domain.com>',
			'unique_arg_key':'unique_arg_value',
			'category':['category1', 'category2'],
			'newsletter': {
				'newsletter_user_list_id': '10557865',
				'newsletter_id': '1943530',
				'newsletter_send_id': '2308608'
			},
			'asm_group_id': 1,
			'ip' : '127.0.0.1',
			'tls' : '1',
			'cert_err' : '1'
		}";

		private const string CLICK_JSON = @"
		{
			'sg_event_id':'sendgrid_internal_event_id',
			'sg_message_id':'sendgrid_internal_message_id',
			'ip':'255.255.255.255',
			'useragent':'Mozilla/5.0 (iPhone; CPU iPhone OS 7_1_2 like Mac OS X) AppleWebKit/537.51.2 (KHTML, like Gecko) Version/7.0 Mobile/11D257 Safari/9537.53',
			'event':'click',
			'email':'email@example.com',
			'timestamp':1249948800,
			'url':'http://yourdomain.com/blog/news.html',
			'url_offset': {
				'index': 0,
				'type': 'html'
			},
			'unique_arg_key':'unique_arg_value',
			'category':['category1', 'category2'],
			'newsletter': {
				'newsletter_user_list_id': '10557865',
				'newsletter_id': '1943530',
				'newsletter_send_id': '2308608'
			},
			'asm_group_id': 1
		}";

		[Fact]
		public void Parse_processed_JSON()
		{
			// Arrange

			// Act
			var result = (ProcessedEvent)JsonConvert.DeserializeObject<Event>(PROCESSED_JSON, new WebHookEventConverter());

			// Assert
			result.AsmGroupId.ShouldBe(1);
			result.Categories.Length.ShouldBe(2);
			result.Categories[0].ShouldBe("category1");
			result.Categories[1].ShouldBe("category2");
			result.CertificateValidationError.ShouldBeFalse();
			result.Email.ShouldBe("email@example.com");
			result.EventType.ShouldBe(EventType.Processed);
			result.InternalEventId.ShouldBe("sendgrid_internal_event_id");
			result.InternalMessageId.ShouldBe("sendgrid_internal_message_id");
			result.IpAddress.ShouldBeNull();
			result.Newsletter.ShouldNotBeNull();
			result.Newsletter.Id.ShouldBe("1943530");
			result.Newsletter.SendId.ShouldBe("2308608");
			result.Newsletter.UserListId.ShouldBe("10557865");
			result.ProcessedOn.ToUnixTime().ShouldBe(1249949000);
			result.SmtpId.ShouldBe("<original-smtp-id@domain.com>");
			result.Timestamp.ToUnixTime().ShouldBe(1249948800);
			result.Tls.ShouldBeFalse();
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(1);
			result.UniqueArguments.Keys.ShouldContain("unique_arg_key");
			result.UniqueArguments["unique_arg_key"].ShouldBe("unique_arg_value");
		}

		[Fact]
		public void Parse_bounced_JSON()
		{
			// Arrange

			// Act
			var result = (BouncedEvent)JsonConvert.DeserializeObject<Event>(BOUNCED_JSON, new WebHookEventConverter());

			// Assert
			result.Categories.Length.ShouldBe(2);
			result.Categories[0].ShouldBe("category1");
			result.Categories[1].ShouldBe("category2");
			result.CertificateValidationError.ShouldBeFalse();
			result.Email.ShouldBe("email@example.com");
			result.EventType.ShouldBe(EventType.Bounce);
			result.InternalEventId.ShouldBe("sendgrid_internal_event_id");
			result.InternalMessageId.ShouldBe("sendgrid_internal_message_id");
			result.IpAddress.ShouldBe("127.0.0.1");
			result.Reason.ShouldBe("500 No Such User");
			result.SmtpId.ShouldBe("<original-smtp-id@domain.com>");
			result.Status.ShouldBe("5.0.0");
			result.Timestamp.ToUnixTime().ShouldBe(1249948800);
			result.Tls.ShouldBeTrue();
			result.Type.ShouldBe("bounce");
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(1);
			result.UniqueArguments.Keys.ShouldContain("unique_arg_key");
			result.UniqueArguments["unique_arg_key"].ShouldBe("unique_arg_value");
		}

		[Fact]
		public void Parse_deferred_JSON()
		{
			// Arrange

			// Act
			var result = (DeferredEvent)JsonConvert.DeserializeObject<Event>(DEFERRED_JSON, new WebHookEventConverter());

			// Assert
			result.AsmGroupId.ShouldBe(1);
			result.Attempt.ShouldBe(10);
			result.Categories.Length.ShouldBe(2);
			result.Categories[0].ShouldBe("category1");
			result.Categories[1].ShouldBe("category2");
			result.CertificateValidationError.ShouldBeFalse();
			result.Email.ShouldBe("email@example.com");
			result.EventType.ShouldBe(EventType.Deferred);
			result.InternalEventId.ShouldBe("sendgrid_internal_event_id");
			result.InternalMessageId.ShouldBe("sendgrid_internal_message_id");
			result.IpAddress.ShouldBe("127.0.0.1");
			result.Newsletter.ShouldNotBeNull();
			result.Newsletter.Id.ShouldBe("1943530");
			result.Newsletter.SendId.ShouldBe("2308608");
			result.Newsletter.UserListId.ShouldBe("10557865");
			result.Response.ShouldBe("400 Try again");
			result.SmtpId.ShouldBe("<original-smtp-id@domain.com>");
			result.Timestamp.ToUnixTime().ShouldBe(1249948800);
			result.Tls.ShouldBeFalse();
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(1);
			result.UniqueArguments.Keys.ShouldContain("unique_arg_key");
			result.UniqueArguments["unique_arg_key"].ShouldBe("unique_arg_value");
		}

		[Fact]
		public void Parse_dropped_JSON()
		{
			// Arrange

			// Act
			var result = (DroppedEvent)JsonConvert.DeserializeObject<Event>(DROPPED_JSON, new WebHookEventConverter());

			// Assert
			result.Categories.Length.ShouldBe(2);
			result.Categories[0].ShouldBe("category1");
			result.Categories[1].ShouldBe("category2");
			result.CertificateValidationError.ShouldBeFalse();
			result.Email.ShouldBe("email@example.com");
			result.EventType.ShouldBe(EventType.Dropped);
			result.InternalEventId.ShouldBe("sendgrid_internal_event_id");
			result.InternalMessageId.ShouldBe("sendgrid_internal_message_id");
			result.IpAddress.ShouldBeNull();
			result.Reason.ShouldBe("Bounced Address");
			result.SmtpId.ShouldBe("<original-smtp-id@domain.com>");
			result.Timestamp.ToUnixTime().ShouldBe(1249948800);
			result.Tls.ShouldBeFalse();
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(1);
			result.UniqueArguments.Keys.ShouldContain("unique_arg_key");
			result.UniqueArguments["unique_arg_key"].ShouldBe("unique_arg_value");
		}

		[Fact]
		public void Parse_delivered_JSON()
		{
			// Arrange

			// Act
			var result = (DeliveredEvent)JsonConvert.DeserializeObject<Event>(DELIVERED_JSON, new WebHookEventConverter());

			// Assert
			result.AsmGroupId.ShouldBe(1);
			result.Categories.Length.ShouldBe(2);
			result.Categories[0].ShouldBe("category1");
			result.Categories[1].ShouldBe("category2");
			result.CertificateValidationError.ShouldBeTrue();
			result.Email.ShouldBe("email@example.com");
			result.EventType.ShouldBe(EventType.Delivered);
			result.InternalEventId.ShouldBe("sendgrid_internal_event_id");
			result.InternalMessageId.ShouldBe("sendgrid_internal_message_id");
			result.IpAddress.ShouldBe("127.0.0.1");
			result.Newsletter.ShouldNotBeNull();
			result.Newsletter.Id.ShouldBe("1943530");
			result.Newsletter.SendId.ShouldBe("2308608");
			result.Newsletter.UserListId.ShouldBe("10557865");
			result.Response.ShouldBe("250 OK");
			result.SmtpId.ShouldBe("<original-smtp-id@domain.com>");
			result.Timestamp.ToUnixTime().ShouldBe(1249948800);
			result.Tls.ShouldBeTrue();
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(1);
			result.UniqueArguments.Keys.ShouldContain("unique_arg_key");
			result.UniqueArguments["unique_arg_key"].ShouldBe("unique_arg_value");
		}

		[Fact]
		public void Parse_click_JSON()
		{
			// Arrange

			// Act
			var result = (ClickEvent)JsonConvert.DeserializeObject<Event>(CLICK_JSON, new WebHookEventConverter());

			// Assert
			result.AsmGroupId.ShouldBe(1);
			result.Categories.Length.ShouldBe(2);
			result.Categories[0].ShouldBe("category1");
			result.Categories[1].ShouldBe("category2");
			result.Email.ShouldBe("email@example.com");
			result.EventType.ShouldBe(EventType.Click);
			result.InternalEventId.ShouldBe("sendgrid_internal_event_id");
			result.InternalMessageId.ShouldBe("sendgrid_internal_message_id");
			result.IpAddress.ShouldBe("255.255.255.255");
			result.Newsletter.ShouldNotBeNull();
			result.Newsletter.Id.ShouldBe("1943530");
			result.Newsletter.SendId.ShouldBe("2308608");
			result.Newsletter.UserListId.ShouldBe("10557865");
			result.Timestamp.ToUnixTime().ShouldBe(1249948800);
			result.UniqueArguments.ShouldNotBeNull();
			result.UniqueArguments.Count.ShouldBe(1);
			result.UniqueArguments.Keys.ShouldContain("unique_arg_key");
			result.UniqueArguments["unique_arg_key"].ShouldBe("unique_arg_value");
			result.Url.ShouldBe("http://yourdomain.com/blog/news.html");
			result.UrlOffset.ShouldNotBeNull();
			result.UrlOffset.Index.ShouldBe(0);
			result.UrlOffset.Type.ShouldBe("html");
			result.UserAgent.ShouldBe("Mozilla/5.0 (iPhone; CPU iPhone OS 7_1_2 like Mac OS X) AppleWebKit/537.51.2 (KHTML, like Gecko) Version/7.0 Mobile/11D257 Safari/9537.53");
		}

		[Fact]
		public void Processed()
		{
			// Arrange
			var responseContent = $"[{PROCESSED_JSON}]";
			var parser = new WebhookParser();
			var mockResponse = GetMockRequest(responseContent);
				
			// Act
			var result = parser.ParseWebhookEventsAsync(mockResponse.Object).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].GetType().ShouldBe(typeof(ProcessedEvent));
		}

		[Fact]
		public void Bounced()
		{
			// Arrange
			var responseContent = $"[{BOUNCED_JSON}]";
			var parser = new WebhookParser();
			var mockResponse = GetMockRequest(responseContent);

			// Act
			var result = parser.ParseWebhookEventsAsync(mockResponse.Object).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].GetType().ShouldBe(typeof(BouncedEvent));
		}

		[Fact]
		public void Deferred()
		{
			// Arrange
			var responseContent = $"[{DEFERRED_JSON}]";
			var parser = new WebhookParser();
			var mockResponse = GetMockRequest(responseContent);

			// Act
			var result = parser.ParseWebhookEventsAsync(mockResponse.Object).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].GetType().ShouldBe(typeof(DeferredEvent));
		}

		[Fact]
		public void Dropped()
		{
			// Arrange
			var responseContent = $"[{DROPPED_JSON}]";
			var parser = new WebhookParser();
			var mockResponse = GetMockRequest(responseContent);

			// Act
			var result = parser.ParseWebhookEventsAsync(mockResponse.Object).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].GetType().ShouldBe(typeof(DroppedEvent));
		}

		[Fact]
		public void Click()
		{
			// Arrange
			var responseContent = $"[{CLICK_JSON}]";
			var parser = new WebhookParser();
			var mockResponse = GetMockRequest(responseContent);

			// Act
			var result = parser.ParseWebhookEventsAsync(mockResponse.Object).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].GetType().ShouldBe(typeof(ClickEvent));
		}

		private Mock<HttpRequest> GetMockRequest(string responseContent)
		{
			var mockRequest = new Mock<HttpRequest>(MockBehavior.Strict);
			mockRequest
				.SetupGet(r => r.Body)
				.Returns(() =>
				{
					var ms = new MemoryStream();
					var sw = new StreamWriter(ms);
					sw.Write(responseContent);
					sw.Flush();
					ms.Position = 0;
					return ms;
				});

			return mockRequest;
		}
	}
}
