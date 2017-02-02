using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Model;
using StrongGrid.UnitTests;
using StrongGrid.Utilities;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class InvalidEmailsTests
	{
		#region FIELDS

		private const string ENDPOINT = "suppression/invalid_emails";

		private const string SINGLE_INVALID_EMAIL_JSON = @"{
			'created': 1454433146,
			'email': 'test1@example.com',
			'reason': 'Mail domain mentioned in email address is unknown'
		}";
		private const string MULTIPLE_INVALID_EMAILS_JSON = @"[
			{
				'created': 1449953655,
				'email': 'user1@example.com',
				'reason': 'Mail domain mentioned in email address is unknown'
			},
			{
				'created': 1449939373,
				'email': 'user1@example.com',
				'reason': 'Mail domain mentioned in email address is unknown'
			}
		]";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<InvalidEmail>(SINGLE_INVALID_EMAIL_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.CreatedOn.ShouldBe(new DateTime(2016, 2, 2, 17, 12, 26, DateTimeKind.Utc));
			result.Email.ShouldBe("test1@example.com");
			result.Reason.ShouldBe("Mail domain mentioned in email address is unknown");
		}

		[Fact]
		public void GetAll()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT) + $"?start_time=&end_time=&limit=25&offset=0").Respond("application/json", MULTIPLE_INVALID_EMAILS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var invalidEmails = new InvalidEmails(client);

			// Act
			var result = invalidEmails.GetAllAsync().Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public void DeleteAll()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var invalidEmails = new InvalidEmails(client);

			// Act
			invalidEmails.DeleteAllAsync().Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void DeleteMultiple()
		{
			// Arrange
			var emailAddresses = new[] { "email1@test.com", "email2@test.com" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var invalidEmails = new InvalidEmails(client);

			// Act
			invalidEmails.DeleteMultipleAsync(emailAddresses).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void Delete()
		{
			// Arrange
			var emailAddress = "spam1@test.com";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, emailAddress)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var invalidEmails = new InvalidEmails(client);

			// Act
			invalidEmails.DeleteAsync(emailAddress).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void Get()
		{
			// Arrange
			var emailAddress = "test1@example.com";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, emailAddress)).Respond("application/json", MULTIPLE_INVALID_EMAILS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var invalidEmails = new InvalidEmails(client);

			// Act
			var result = invalidEmails.GetAsync(emailAddress, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}
	}
}
