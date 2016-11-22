using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using StrongGrid.Model;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class UnsubscribeGroupsTests
	{
		#region FIELDS

		private const string ENDPOINT = "/asm/groups";

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

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<SuppressionGroup>(SINGLE_SUPPRESSION_GROUP_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.Description.ShouldBe("Suggestions for products our users might like.");
			result.Id.ShouldBe(1);
			result.IsDefault.ShouldBe(true);
			result.Name.ShouldBe("Product Suggestions");
		}

		[Fact]
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

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var goups = new UnsubscribeGroups(mockClient.Object, ENDPOINT);

			// Act
			var result = goups.CreateAsync(name, description, isDefault, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Name.ShouldBe(name);
			result.Description.ShouldBe(description);
			result.IsDefault.ShouldBe(isDefault);
		}

		[Fact]
		public void Get()
		{
			// Arrange
			var groupId = 100;

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{groupId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_SUPPRESSION_GROUP_JSON) })
				.Verifiable();

			var groups = new UnsubscribeGroups(mockClient.Object, ENDPOINT);

			// Act
			var result = groups.GetAsync(groupId, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetAll()
		{
			// Arrange
			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_SUPPRESSION_GROUPS_JSON) })
				.Verifiable();

			var groups = new UnsubscribeGroups(mockClient.Object, ENDPOINT);

			// Act
			var result = groups.GetAllAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
			result[0].Id.ShouldBe(100);
			result[1].Id.ShouldBe(101);
		}

		[Fact]
		public void Delete()
		{
			// Arrange
			var groupId = 100;

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{groupId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
				.Verifiable();

			var groups = new UnsubscribeGroups(mockClient.Object, ENDPOINT);

			// Act
			groups.DeleteAsync(groupId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void Update()
		{
			// Arrange
			var groupId = 103;
			var name = "Item Suggestions";
			var description = "Suggestions for items our users might like.";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync($"{ENDPOINT}/{groupId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_SUPPRESSION_GROUP_JSON) })
				.Verifiable();

			var groups = new UnsubscribeGroups(mockClient.Object, ENDPOINT);

			// Act
			var result = groups.UpdateAsync(groupId, name, description, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}
	}
}
