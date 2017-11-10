using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Resources;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class SuppresionsTests
	{
		#region FIELDS

		private const string ENDPOINT = "asm";
		private const string ALL_GROUPS_JSON = @"{
			'suppressions': [
				{
					'description': 'Optional description.',
					'id': 1,
					'is_default': true,
					'name': 'Weekly News',
					'suppressed': true
				},
				{
					'description': 'Some daily news.',
					'id': 2,
					'is_default': true,
					'name': 'Daily News',
					'suppressed': true
				},
				{
					'description': 'An old group.',
					'id': 2,
					'is_default': false,
					'name': 'Old News',
					'suppressed': false
				}
			]
		}";
		private const string ALL_SUPPRESSIONS_JSON = @"[
			{
				'email':'test @example.com',
				'group_id': 1,
				'group_name': 'Weekly News',
				'created_at': 1410986704
			},
			{
				'email':'test1@example.com',
				'group_id': 2,
				'group_name': 'Daily News',
				'created_at': 1411493671
			},
			{
				'email':'test2@example.com',
				'group_id': 2,
				'group_name': 'Daily News',
				'created_at': 1411493671
			}
		]";

		#endregion

		[Fact]
		public async Task GetAllAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "suppressions")).Respond("application/json", ALL_SUPPRESSIONS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var suppresions = new Suppressions(client);

			// Act
			var result = await suppresions.GetAllAsync(null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(3);
		}

		[Fact]
		public async Task GetUnsubscribedGroupsAsync()
		{
			// Arrange
			var email = "test@exmaple.com";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "suppressions", email)).Respond("application/json", ALL_GROUPS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var suppresions = new Suppressions(client);

			// Act
			var result = await suppresions.GetUnsubscribedGroupsAsync(email, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public async Task GetUnsubscribedAddressesAsync()
		{
			// Arrange
			var groupId = 123;

			var apiResponse = @"[
				'test1@example.com',
				'test2@example.com'
			]";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "groups", groupId, "suppressions")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var suppresions = new Suppressions(client);

			// Act
			var result = await suppresions.GetUnsubscribedAddressesAsync(groupId, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
			result[0].ShouldBe("test1@example.com");
			result[1].ShouldBe("test2@example.com");
		}

		[Fact]
		public async Task AddAddressToUnsubscribeGroupAsync_single_email()
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
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "groups", groupId, "suppressions")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var suppressions = new Suppressions(client);

			// Act
			await suppressions.AddAddressToUnsubscribeGroupAsync(groupId, email, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task AddAddressToUnsubscribeGroupAsync_multiple_emails()
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
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "groups", groupId, "suppressions")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var suppressions = new Suppressions(client);

			// Act
			await suppressions.AddAddressToUnsubscribeGroupAsync(groupId, emails, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task RemoveAddressFromSuppressionGrouAsyncp()
		{
			// Arrange
			var groupId = 103;
			var email = "test1@example.com";


			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, "groups", groupId, "suppressions", email)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var suppressions = new Suppressions(client);

			// Act
			await suppressions.RemoveAddressFromSuppressionGroupAsync(groupId, email, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task IsSuppressedAsync_true()
		{
			// Arrange
			var email = "test@example.com";
			var groupId = 123;

			var apiResponse = @"[
				'a@a.com',
				'b@b.com',
				'test@example.com'
			]";
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "groups", groupId, "suppressions/search")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var suppresions = new Suppressions(client);

			// Act
			var result = await suppresions.IsSuppressedAsync(groupId, email, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBeTrue();
		}

		[Fact]
		public async Task IsSuppressedAsync_false()
		{
			// Arrange
			var email = "test@example.com";
			var groupId = 123;

			var apiResponse = @"[
				'a@a.com',
				'b@b.com'
			]";
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "groups", groupId, "suppressions/search")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var suppresions = new Suppressions(client);

			// Act
			var result = await suppresions.IsSuppressedAsync(groupId, email, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBeFalse();
		}
	}
}
