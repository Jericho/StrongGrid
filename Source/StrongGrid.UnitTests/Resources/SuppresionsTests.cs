using Moq;
using Newtonsoft.Json.Linq;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.UnitTests;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class SuppresionsTests
	{
		#region FIELDS

		private const string ENDPOINT = "/asm/groups";

		#endregion

		[Fact]
		public void GetUnsubscribedAddresses()
		{
			// Arrange
			var groupId = 123;

			var apiResponse = @"[
				'test1@example.com',
				'test2@example.com'
			]";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, $"{ENDPOINT}/{groupId}/suppressions").Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var suppresions = new Suppressions(client, ENDPOINT);

			// Act
			var result = suppresions.GetUnsubscribedAddressesAsync(groupId, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
			result[0].ShouldBe("test1@example.com");
			result[1].ShouldBe("test2@example.com");
		}

		[Fact]
		public void AddAddressToUnsubscribeGroup_single_email()
		{
			// Arrange
			var groupId = 103;
			var email = "test1@example.com";

			var apiResponse = @"{
				'recipient_emails': [
					'test1@example.com',
					'test2@example.com'
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, $"{ENDPOINT}/{groupId}/suppressions").Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var suppressions = new Suppressions(client, ENDPOINT);

			// Act
			suppressions.AddAddressToUnsubscribeGroupAsync(groupId, email, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void AddAddressToUnsubscribeGroup_multiple_emails()
		{
			// Arrange
			var groupId = 103;
			var emails = new[] { "test1@example.com", "test2@example.com" };

			var apiResponse = @"{
				'recipient_emails': [
					'test1@example.com',
					'test2@example.com'
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, $"{ENDPOINT}/{groupId}/suppressions").Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var suppressions = new Suppressions(client, ENDPOINT);

			// Act
			suppressions.AddAddressToUnsubscribeGroupAsync(groupId, emails, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void RemoveAddressFromSuppressionGroup()
		{
			// Arrange
			var groupId = 103;
			var email = "test1@example.com";


			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, $"{ENDPOINT}/{groupId}/suppressions/{email}").Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var suppressions = new Suppressions(client, ENDPOINT);

			// Act
			suppressions.RemoveAddressFromSuppressionGroupAsync(groupId, email, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}
	}
}
