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
