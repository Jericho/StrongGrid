using Moq;
using Newtonsoft.Json.Linq;
using Shouldly;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class GlobalSuppressionTests
	{
		#region FIELDS

		private const string ENDPOINT = "/asm/suppressions/global";

		#endregion

		[Fact]
		public void Add()
		{
			// Arrange
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
				.Setup(c => c.PostAsync(ENDPOINT, It.Is<JObject>(o => o["recipient_emails"].ToObject<JArray>().Count == emails.Length), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var globalSuppressions = new GlobalSuppressions(mockClient.Object, ENDPOINT);

			// Act
			globalSuppressions.AddAsync(emails, CancellationToken.None).Wait();

			// Assert
		}

		[Fact]
		public void Delete()
		{
			// Arrange
			var email = "test1@example.com";


			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{email}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var globalSuppressions = new GlobalSuppressions(mockClient.Object, ENDPOINT);

			// Act
			globalSuppressions.RemoveAsync(email, CancellationToken.None).Wait();

			// Assert
		}

		[Fact]
		public void IsUnsubscribed_true()
		{
			// Arrange
			var email = "test1@example.com";

			var apiResponse = @"{
				'recipient_email': 'test1@example.com'
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{email}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var globalSuppressions = new GlobalSuppressions(mockClient.Object, ENDPOINT);

			// Act
			var result = globalSuppressions.IsUnsubscribedAsync(email, CancellationToken.None).Result;

			// Assert
			result.ShouldBeTrue();
		}

		[Fact]
		public void IsUnsubscribed_false()
		{
			// Arrange
			var email = "test1@example.com";

			var apiResponse = @"{
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{email}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var globalSuppressions = new GlobalSuppressions(mockClient.Object, ENDPOINT);

			// Act
			var result = globalSuppressions.IsUnsubscribedAsync(email, CancellationToken.None).Result;

			// Assert
			result.ShouldBeFalse();
		}
	}
}
