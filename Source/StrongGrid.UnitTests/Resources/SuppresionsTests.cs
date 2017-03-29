using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.UnitTests;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
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
		public void GetAll()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "suppressions")).Respond("application/json", ALL_SUPPRESSIONS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var suppresions = new Suppressions(client);

			// Act
			var result = suppresions.GetAllAsync(CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(3);
		}

		[Fact]
		public void GetUnsubscribedGroups()
		{
			// Arrange
			var email = "test@exmaple.com";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "suppressions", email)).Respond("application/json", ALL_GROUPS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var suppresions = new Suppressions(client);

			// Act
			var result = suppresions.GetUnsubscribedGroupsAsync(email, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public void GetUnsubscribedAddresses()
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
			var result = suppresions.GetUnsubscribedAddressesAsync(groupId, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "groups", groupId, "suppressions")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var suppressions = new Suppressions(client);

			// Act
			suppressions.AddAddressToUnsubscribeGroupAsync(groupId, email, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "groups", groupId, "suppressions")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var suppressions = new Suppressions(client);

			// Act
			suppressions.AddAddressToUnsubscribeGroupAsync(groupId, emails, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void RemoveAddressFromSuppressionGroup()
		{
			// Arrange
			var groupId = 103;
			var email = "test1@example.com";


			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, "groups", groupId, "suppressions", email)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var suppressions = new Suppressions(client);

			// Act
			suppressions.RemoveAddressFromSuppressionGroupAsync(groupId, email, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void IsSuppressed_true()
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
			var result = suppresions.IsSuppressedAsync(groupId, email, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBeTrue();
		}

		[Fact]
		public void IsSuppressed_()
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
			var result = suppresions.IsSuppressedAsync(groupId, email, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBeFalse();
		}
	}
}
