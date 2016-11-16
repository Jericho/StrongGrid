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
	public class ListsTests
	{
		#region FIELDS

		private const string ENDPOINT = "/contactdb/lists";

		private const string SINGLE_LIST_JSON = @"{
			'id': 1,
			'name': 'listname',
			'recipient_count': 0
		}";
		private const string MULTIPLE_LISTS_JSON = @"{
			'lists': [
				{
					'id': 1,
					'name': 'the jones',
					'recipient_count': 1
				}
			]
		}";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<List>(SINGLE_LIST_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.Id.ShouldBe(1);
			result.Name.ShouldBe("listname");
			result.RecipientCount.ShouldBe(0);
		}

		[Fact]
		public void Create()
		{
			// Arrange
			var name = "listname";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_LIST_JSON) })
				.Verifiable();

			var lists = new Lists(mockClient.Object, ENDPOINT);

			// Act
			var result = lists.CreateAsync(name, CancellationToken.None).Result;

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
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_LISTS_JSON) })
				.Verifiable();

			var lists = new Lists(mockClient.Object, ENDPOINT);

			// Act
			var result = lists.GetAllAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
		}

		[Fact]
		public void Delete_single()
		{
			// Arrange
			var listId = 1;

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{listId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var lists = new Lists(mockClient.Object, ENDPOINT);

			// Act
			lists.DeleteAsync(listId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void Delete_multiple()
		{
			// Arrange
			var listIds = new long[] { 1, 2, 3 };

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync(ENDPOINT, It.Is<JArray>(o => o.Count == 3), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var lists = new Lists(mockClient.Object, ENDPOINT);

			// Act
			lists.DeleteAsync(listIds, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void Get()
		{
			// Arrange
			var listId = 1;

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{listId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_LIST_JSON) })
				.Verifiable();

			var lists = new Lists(mockClient.Object, ENDPOINT);

			// Act
			var result = lists.GetAsync(listId, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void Update()
		{
			// Arrange
			var listId = 5;
			var name = "newlistname";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync($"{ENDPOINT}/{listId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
				.Verifiable();

			var lists = new Lists(mockClient.Object, ENDPOINT);

			// Act
			lists.UpdateAsync(listId, name, CancellationToken.None).Wait();

			// Assert
		}

		[Fact]
		public void GetRecipients()
		{
			// Arrange
			var listId = 1;
			var recordsPerPage = 25;
			var page = 1;

			var apiResponse = @"{
				'recipients': [
					{
						'created_at': 1422395108,
						'email': 'e@example.com',
						'first_name': 'Ed',
						'id': 'YUBh',
						'last_clicked': null,
						'last_emailed': null,
						'last_name': null,
						'last_opened': null,
						'updated_at': 1422395108
					}
				]
			}";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{listId}/recipients?page_size={recordsPerPage}&page={page}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var lists = new Lists(mockClient.Object, ENDPOINT);

			// Act
			var result = lists.GetRecipientsAsync(listId, recordsPerPage, page, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].Email.ShouldBe("e@example.com");
		}

		[Fact]
		public void AddRecipient()
		{
			// Arrange
			var listId = 1;
			var contactId = "abc123";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/{listId}/recipients/{contactId}", (JObject)null, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created))
				.Verifiable();

			var lists = new Lists(mockClient.Object, ENDPOINT);

			// Act
			lists.AddRecipientAsync(listId, contactId, CancellationToken.None).Wait();

			// Assert
		}

		[Fact]
		public void RemoveRecipient()
		{
			// Arrange
			var listId = 1;
			var contactId = "abc123";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{listId}/recipients/{contactId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var lists = new Lists(mockClient.Object, ENDPOINT);

			// Act
			lists.RemoveRecipientAsync(listId, contactId, CancellationToken.None).Wait();

			// Assert
		}

		[Fact]
		public void AddRecipients()
		{
			// Arrange
			var listId = 1;
			var contactIds = new[] { "abc123", "def456" };

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/{listId}/recipients", It.Is<JArray>(o => o.Count == 2), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created))
				.Verifiable();

			var lists = new Lists(mockClient.Object, ENDPOINT);

			// Act
			lists.AddRecipientsAsync(listId, contactIds, CancellationToken.None).Wait();

			// Assert
		}
	}
}
