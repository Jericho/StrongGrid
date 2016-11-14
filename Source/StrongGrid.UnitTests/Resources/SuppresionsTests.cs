using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class SuppresionsTests
	{
		#region FIELDS

		private const string ENDPOINT = "/asm/groups";
		private MockRepository _mockRepository;
		private Mock<IClient> _mockClient;

		#endregion

		private Suppressions CreateSuppressions()
		{
			return new Suppressions(_mockClient.Object, ENDPOINT);

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
		public void GetUnsubscribedAddresses()
		{
			// Arrange
			var groupId = 123;

			var apiResponse = @"[
				'test1@example.com',
				'test2@example.com'
			]";

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{groupId}/suppressions", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var suppresions = CreateSuppressions();

			// Act
			var result = suppresions.GetUnsubscribedAddressesAsync(groupId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual("test1@example.com", result[0]);
			Assert.AreEqual("test2@example.com", result[1]);
		}

		[TestMethod]
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

			_mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/{groupId}/suppressions", It.Is<JObject>(o => o["recipient_emails"].ToObject<JArray>().Count == 1), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var suppressions = CreateSuppressions();

			// Act
			suppressions.AddAddressToUnsubscribeGroupAsync(groupId, email, CancellationToken.None).Wait();

			// Assert
		}

		[TestMethod]
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

			_mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/{groupId}/suppressions", It.Is<JObject>(o => o["recipient_emails"].ToObject<JArray>().Count == emails.Length), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var suppressions = CreateSuppressions();

			// Act
			suppressions.AddAddressToUnsubscribeGroupAsync(groupId, emails, CancellationToken.None).Wait();

			// Assert
		}

		[TestMethod]
		public void RemoveAddressFromSuppressionGroup()
		{
			// Arrange
			var groupId = 103;
			var email = "test1@example.com";


			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{groupId}/suppressions/{email}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var suppressions = CreateSuppressions();

			// Act
			suppressions.RemoveAddressFromSuppressionGroupAsync(groupId, email, CancellationToken.None).Wait();

			// Assert
		}
	}
}
