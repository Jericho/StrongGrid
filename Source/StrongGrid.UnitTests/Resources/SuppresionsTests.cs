using Moq;
using Newtonsoft.Json.Linq;
using Shouldly;
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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{groupId}/suppressions", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var suppresions = new Suppressions(mockClient.Object, ENDPOINT);

			// Act
			var result = suppresions.GetUnsubscribedAddressesAsync(groupId, CancellationToken.None).Result;

			// Assert
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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/{groupId}/suppressions", It.Is<JObject>(o => o["recipient_emails"].ToObject<JArray>().Count == 1), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var suppressions = new Suppressions(mockClient.Object, ENDPOINT);

			// Act
			suppressions.AddAddressToUnsubscribeGroupAsync(groupId, email, CancellationToken.None).Wait();

			// Assert
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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/{groupId}/suppressions", It.Is<JObject>(o => o["recipient_emails"].ToObject<JArray>().Count == emails.Length), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var suppressions = new Suppressions(mockClient.Object, ENDPOINT);

			// Act
			suppressions.AddAddressToUnsubscribeGroupAsync(groupId, emails, CancellationToken.None).Wait();

			// Assert
		}

		[Fact]
		public void RemoveAddressFromSuppressionGroup()
		{
			// Arrange
			var groupId = 103;
			var email = "test1@example.com";


			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{groupId}/suppressions/{email}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var suppressions = new Suppressions(mockClient.Object, ENDPOINT);

			// Act
			suppressions.RemoveAddressFromSuppressionGroupAsync(groupId, email, CancellationToken.None).Wait();

			// Assert
		}
	}
}
