using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class BouncesTests
	{
		#region FIELDS

		private const string ENDPOINT = "/suppression/bounces";

		private const string SINGLE_BOUNCE_JSON = @"{
			'created': 1443651125,
			'email': 'testemail1@test.com',
			'reason': '550 5.1.1 The email account that you tried to reach does not exist. Please try double-checking the recipient\'s email address for typos or unnecessary spaces. Learn more at  https://support.google.com/mail/answer/6596 o186si2389584ioe.63 - gsmtp ',
			'status': '5.1.1'
		}";
		private const string MULTIPLE_BOUNCES_JSON = @"[
			{
				'created': 1443651125,
				'email': 'testemail1@test.com',
				'reason': '550 5.1.1 The email account that you tried to reach does not exist. Please try double-checking the recipient\'s email address for typos or unnecessary spaces. Learn more at  https://support.google.com/mail/answer/6596 o186si2389584ioe.63 - gsmtp ',
				'status': '5.1.1'
			},
			{
				'created': 1433800303,
				'email': 'testemail2@testing.com',
				'reason': '550 5.1.1 <testemail2@testing.com>: Recipient address rejected: User unknown in virtual alias table ',
				'status': '5.1.1'
			}
		]";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<Bounce>(SINGLE_BOUNCE_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.CreatedOn.ShouldBe(new System.DateTime(2015, 9, 30, 22, 12, 5, 0, System.DateTimeKind.Utc));
			result.EmailAddress.ShouldBe("testemail1@test.com");
			result.Reason.ShouldBe("550 5.1.1 The email account that you tried to reach does not exist. Please try double-checking the recipient\'s email address for typos or unnecessary spaces. Learn more at  https://support.google.com/mail/answer/6596 o186si2389584ioe.63 - gsmtp ");
			result.Status.ShouldBe("5.1.1");
		}

		[Fact]
		public void Get_between_startdate_and_enddate()
		{
			// Arrange
			var start = new DateTime(2015, 6, 8, 0, 0, 0, DateTimeKind.Utc);
			var end = new DateTime(2015, 9, 30, 23, 59, 59, DateTimeKind.Utc);

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}?start_time={start.ToUnixTime()}&end_time={end.ToUnixTime()}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_BOUNCES_JSON) })
				.Verifiable();

			var bounces = new Bounces(mockClient.Object, ENDPOINT);

			// Act
			var result = bounces.GetAsync(start, end, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public void Get_by_email()
		{
			// Arrange
			var email = "bounce1@test.com";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{email}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_BOUNCES_JSON) }).
				Verifiable();

			var bounces = new Bounces(mockClient.Object, ENDPOINT);

			// Act
			var result = bounces.GetAsync(email, CancellationToken.None).Result;

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
				.Setup(c => c.DeleteAsync(ENDPOINT, It.Is<JObject>(o => o["delete_all"].ToObject<bool>()), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var bounces = new Bounces(mockClient.Object, ENDPOINT);

			// Act
			bounces.DeleteAllAsync(CancellationToken.None).Wait();

			// Assert
		}

		[Fact]
		public void Delete_by_single_email()
		{
			// Arrange
			var email = "email1@test.com";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{email}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var bounces = new Bounces(mockClient.Object, ENDPOINT);

			// Act
			bounces.DeleteAsync(email, CancellationToken.None).Wait();

			// Assert
		}

		[Fact]
		public void Delete_by_multiple_emails()
		{
			// Arrange
			var emails = new[] { "email1@test.com", "email2@test.com" };

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync(ENDPOINT, It.Is<JObject>(o => o["emails"].ToObject<JArray>().Count == emails.Length), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var bounces = new Bounces(mockClient.Object, ENDPOINT);

			// Act
			bounces.DeleteAsync(emails, CancellationToken.None).Wait();

			// Assert
		}
	}
}
