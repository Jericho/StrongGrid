using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class UnsubscribeGroupsTests
	{
		#region FIELDS

		private const string ENDPOINT = "/asm/groups";
		private MockRepository _mockRepository;
		private Mock<IClient> _mockClient;

		private const string SINGLE_SUPPRESSION_GROUP_JSON = @"{
			'id': 1,
			'name': 'Product Suggestions',
			'description': 'Suggestions for products our users might like.',
			'is_default': true
		}";
		private const string MULTIPLE_SUPPRESSION_GROUPS_JSON = @"[
			{
				'id': 100,
				'name': 'Newsletters',
				'description': 'Our monthly newsletter.',
				'last_email_sent_at': null,
				'is_default': true,
				'unsubscribes': 400
			},
			{
				'id': 101,
				'name': 'Alerts',
				'description 2': 'Emails triggered by user-defined rules.',
				'last_email_sent_at': null,
				'is_default': false,
				'unsubscribes': 1
			}
		]";

		#endregion

		private UnsubscribeGroups CreateUnsubscribeGroups()
		{
			return new UnsubscribeGroups(_mockClient.Object, ENDPOINT);
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
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<SuppressionGroup>(SINGLE_SUPPRESSION_GROUP_JSON);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual("Suggestions for products our users might like.", result.Description);
			Assert.AreEqual(1, result.Id);
			Assert.AreEqual(true, result.IsDefault);
			Assert.AreEqual("Product Suggestions", result.Name);
		}

		[TestMethod]
		public void Create()
		{
			// Arrange
			var name = "Product Suggestions";
			var description = "Suggestions for products our users might like.";
			var isDefault = true;

			var apiResponse = @"{
				'name': 'Product Suggestions',
				'description': 'Suggestions for products our users might like.',
				'is_default': true
			}";

			_mockClient
				.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var goups = CreateUnsubscribeGroups();

			// Act
			var result = goups.CreateAsync(name, description, isDefault, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(name, result.Name);
			Assert.AreEqual(description, result.Description);
			Assert.AreEqual(isDefault, result.IsDefault);
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var groupId = 100;

			_mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{groupId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_SUPPRESSION_GROUP_JSON) })
				.Verifiable();

			var groups = CreateUnsubscribeGroups();

			// Act
			var result = groups.GetAsync(groupId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			_mockClient
				.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_SUPPRESSION_GROUPS_JSON) })
				.Verifiable();

			var groups = CreateUnsubscribeGroups();

			// Act
			var result = groups.GetAllAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual(100, result[0].Id);
			Assert.AreEqual(101, result[1].Id);
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			var groupId = 100;

			_mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{groupId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
				.Verifiable();

			var groups = CreateUnsubscribeGroups();

			// Act
			groups.DeleteAsync(groupId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Update()
		{
			// Arrange
			var groupId = 103;
			var name = "Item Suggestions";
			var description = "Suggestions for items our users might like.";

			_mockClient
				.Setup(c => c.PatchAsync($"{ENDPOINT}/{groupId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_SUPPRESSION_GROUP_JSON) })
				.Verifiable();

			var groups = CreateUnsubscribeGroups();

			// Act
			var result = groups.UpdateAsync(groupId, name, description, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}
	}
}
