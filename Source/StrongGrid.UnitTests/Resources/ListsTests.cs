using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.UnitTests;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class ListsTests
	{
		#region FIELDS

		private const string ENDPOINT = "contactdb/lists";

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
		public async Task CreateAsync()
		{
			// Arrange
			var name = "listname";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_LIST_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client);

			// Act
			var result = await lists.CreateAsync(name, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetAllAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", MULTIPLE_LISTS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client);

			// Act
			var result = await lists.GetAllAsync(CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
		}

		[Fact]
		public async Task DeleteAsync_single()
		{
			// Arrange
			var listId = 1;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, listId)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client);

			// Act
			await lists.DeleteAsync(listId, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task DeleteAsync_multiple()
		{
			// Arrange
			var listIds = new long[] { 1, 2, 3 };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client);

			// Act
			await lists.DeleteAsync(listIds, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task GetAsync()
		{
			// Arrange
			var listId = 1;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, listId)).Respond("application/json", SINGLE_LIST_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client);

			// Act
			var result = await lists.GetAsync(listId, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task UpdateAsync()
		{
			// Arrange
			var listId = 5;
			var name = "newlistname";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT, listId)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client);

			// Act
			await lists.UpdateAsync(listId, name, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task GetRecipientsAsync()
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
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, listId, $"recipients?page_size={recordsPerPage}&page={page}")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client);

			// Act
			var result = await lists.GetRecipientsAsync(listId, recordsPerPage, page, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].Email.ShouldBe("e@example.com");
		}

		[Fact]
		public async Task AddRecipientAsync()
		{
			// Arrange
			var listId = 1;
			var contactId = "abc123";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, listId, "recipients", contactId)).Respond(HttpStatusCode.Created);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client);

			// Act
			await lists.AddRecipientAsync(listId, contactId, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task RemoveRecipientAsync()
		{
			// Arrange
			var listId = 1;
			var contactId = "abc123";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, listId, "recipients", contactId)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client);

			// Act
			await lists.RemoveRecipientAsync(listId, contactId, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task AddRecipientsAsync()
		{
			// Arrange
			var listId = 1;
			var contactIds = new[] { "abc123", "def456" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, listId, "recipients")).Respond(HttpStatusCode.Created);

			var client = Utils.GetFluentClient(mockHttp);
			var lists = new Lists(client);

			// Act
			await lists.AddRecipientsAsync(listId, contactIds, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}
	}
}
