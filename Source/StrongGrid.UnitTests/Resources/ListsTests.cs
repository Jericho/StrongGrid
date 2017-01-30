using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Model;
using StrongGrid.UnitTests;
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, ENDPOINT).Respond("application/json", SINGLE_LIST_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client, ENDPOINT);

			// Act
			var result = lists.CreateAsync(name, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetAll()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, ENDPOINT).Respond("application/json", MULTIPLE_LISTS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client, ENDPOINT);

			// Act
			var result = lists.GetAllAsync(CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
		}

		[Fact]
		public void Delete_single()
		{
			// Arrange
			var listId = 1;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, $"{ENDPOINT}/{listId}").Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client, ENDPOINT);

			// Act
			lists.DeleteAsync(listId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void Delete_multiple()
		{
			// Arrange
			var listIds = new long[] { 1, 2, 3 };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, ENDPOINT).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client, ENDPOINT);

			// Act
			lists.DeleteAsync(listIds, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void Get()
		{
			// Arrange
			var listId = 1;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, $"{ENDPOINT}/{listId}").Respond("application/json", SINGLE_LIST_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client, ENDPOINT);

			// Act
			var result = lists.GetAsync(listId, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void Update()
		{
			// Arrange
			var listId = 5;
			var name = "newlistname";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), $"{ENDPOINT}/{listId}").Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client, ENDPOINT);

			// Act
			lists.UpdateAsync(listId, name, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, $"{ENDPOINT}/{listId}/recipients?page_size={recordsPerPage}&page={page}").Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client, ENDPOINT);

			// Act
			var result = lists.GetRecipientsAsync(listId, recordsPerPage, page, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, $"{ENDPOINT}/{listId}/recipients/{contactId}").Respond(HttpStatusCode.Created);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client, ENDPOINT);

			// Act
			lists.AddRecipientAsync(listId, contactId, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void RemoveRecipient()
		{
			// Arrange
			var listId = 1;
			var contactId = "abc123";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, $"{ENDPOINT}/{listId}/recipients/{contactId}").Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client, ENDPOINT);

			// Act
			lists.RemoveRecipientAsync(listId, contactId, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void AddRecipients()
		{
			// Arrange
			var listId = 1;
			var contactIds = new[] { "abc123", "def456" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, $"{ENDPOINT}/{listId}/recipients").Respond(HttpStatusCode.Created);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client, ENDPOINT);

			// Act
			lists.AddRecipientsAsync(listId, contactIds, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}
	}
}
