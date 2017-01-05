using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using StrongGrid.Model;
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

		private const string ENDPOINT = "/suppression/invalid_emails";

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
			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}?start_time=&end_time=&limit=25&offset=0", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_INVALID_EMAILS_JSON) })
				.Verifiable();

			var invalidEmails = new InvalidEmails(mockClient.Object, ENDPOINT);

			// Act
			var result = invalidEmails.GetAllAsync().Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public void DeleteAll()
		{
			// Arrange
			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var invalidEmails = new InvalidEmails(mockClient.Object, ENDPOINT);

			// Act
			invalidEmails.DeleteAllAsync().Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void DeleteMultiple()
		{
			// Arrange
			var emailAddresses = new[] { "email1@test.com", "email2@test.com" };

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync(ENDPOINT, It.Is<JObject>(o => o["emails"].ToObject<string[]>().Length == emailAddresses.Length), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var invalidEmails = new InvalidEmails(mockClient.Object, ENDPOINT);

			// Act
			invalidEmails.DeleteMultipleAsync(emailAddresses).Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void Delete()
		{
			// Arrange
			var emailAddress = "spam1@test.com";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{emailAddress}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var invalidEmails = new InvalidEmails(mockClient.Object, ENDPOINT);

			// Act
			invalidEmails.DeleteAsync(emailAddress).Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void Get()
		{
			// Arrange
			var emailAddress = "test1@example.com";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{emailAddress}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_INVALID_EMAILS_JSON) })
				.Verifiable();

			var invalidEmails = new InvalidEmails(mockClient.Object, ENDPOINT);

			// Act
			var result = invalidEmails.GetAsync(emailAddress, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}
	}
}
