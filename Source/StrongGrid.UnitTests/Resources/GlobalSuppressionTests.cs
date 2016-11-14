using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class GlobalSuppressionTests
	{
		#region FIELDS

		private const string ENDPOINT = "/asm/suppressions/global";
		private MockRepository _mockRepository;
		private Mock<IClient> _mockClient;

		#endregion

		private GlobalSuppressions CreateGlobalSuppressions()
		{
			return new GlobalSuppressions(_mockClient.Object, ENDPOINT);
		}

		[TestInitialize]
		public void TestInitialize()
		{
			_mockRepository = new MockRepository(MockBehavior.Strict);
			_mockClient = _mockRepository.Create<IClient>();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			_mockRepository.VerifyAll();
		}

		[TestMethod]
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

			_mockClient
				.Setup(c => c.PostAsync(ENDPOINT, It.Is<JObject>(o => o["recipient_emails"].ToObject<JArray>().Count == emails.Length), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var globalSuppressions = CreateGlobalSuppressions();

			// Act
			globalSuppressions.AddAsync(emails, CancellationToken.None).Wait();

			// Assert
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			var email = "test1@example.com";


			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{email}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var globalSuppressions = CreateGlobalSuppressions();

			// Act
			globalSuppressions.RemoveAsync(email, CancellationToken.None).Wait();

			// Assert
		}

		[TestMethod]
		public void IsUnsubscribed_true()
		{
			// Arrange
			var email = "test1@example.com";

			var apiResponse = @"{
				'recipient_email': 'test1@example.com'
			}";

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{email}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var globalSuppressions = CreateGlobalSuppressions();

			// Act
			var result = globalSuppressions.IsUnsubscribedAsync(email, CancellationToken.None).Result;

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void IsUnsubscribed_false()
		{
			// Arrange
			var email = "test1@example.com";

			var apiResponse = @"{
			}";

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{email}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var globalSuppressions = CreateGlobalSuppressions();

			// Act
			var result = globalSuppressions.IsUnsubscribedAsync(email, CancellationToken.None).Result;

			// Assert
			Assert.IsFalse(result);
		}
	}
}
