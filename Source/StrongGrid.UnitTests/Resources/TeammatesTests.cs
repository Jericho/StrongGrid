using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.Resources;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class TeammatesTests
	{
		#region FIELDS

		private const string ENDPOINT = "teammates";

		private const string MULTIPLE_ACCESS_REQUESTS_JSON = @"[
			{
				'id': 1,
				'scope_group_name': 'Mail Settings',
				'username': 'teammate1',
				'email': 'teammate1@example.com',
				'first_name': 'Teammate',
				'last_name': 'One'
			},
			{
				'id': 2,
				'scope_group_name': 'Stats',
				'username': 'teammate2',
				'email': 'teammate2@example.com',
				'first_name': 'Teammate',
				'last_name': 'Two'
			}
		]";
		private const string SINGLE_INVITATION_JSON = @"{
			'email': 'teammate1 @example.com',
			'scopes': [
				'user.profile.read',
				'user.profile.update'
			],
			'is_admin': false
		}";
		private const string MULTIPLE_INVITATIONS_JSON = @"{
			'result': [
				{
					'email': 'user1@example.com',
					'scopes': [
						'user.profile.read',
						'user.profile.edit'
					],
					'is_admin': false,
					'pending_id': 'abcd123abc',
					'expiration_date': 1456424263
				},
				{
					'email': 'user2@example.com',
					'scopes': [],
					'is_admin': true,
					'pending_id': 'bcde234bcd',
					'expiration_date': 1456424263
				}
			]
		}";
		private const string SINGLE_TEAMMATE_JSON = @"{
			'username': 'teammate1',
			'email': 'teammate1@example.com',
			'first_name': 'Jane',
			'last_name': 'Doe',
			'user_type': 'owner',
			'is_admin': true,
			'phone': '123-345-3453',
			'website': 'www.example.com',
			'company': 'ACME Inc.',
			'address': '123 Acme St',
			'address2': '',
			'city': 'City',
			'state': 'CA',
			'country': 'USA',
			'zip': '12345'
		}";
		private const string MULTIPLE_TEAMMATES_JSON = @"{
			'result': [
				{
					'username': 'teammate2',
					'email': 'teammate2@example.com',
					'first_name': 'John',
					'last_name': 'Doe',
					'user_type': 'teammate',
					'is_admin': false,
					'phone': '123-345-3453',
					'website': 'www.example.com',
					'company': 'ACME Inc.',
					'address': '123 Acme St',
					'address2': '',
					'city': 'City',
					'state': 'CA',
					'country': 'USA',
					'zip': '12345'
				},
				{
					'username': 'teammate3',
					'email': 'teammate3@example.com',
					'first_name': 'Steve',
					'last_name': 'Doe',
					'user_type': 'admin',
					'is_admin': true,
					'phone': '123-345-3453',
					'website': 'www.example.com',
					'company': 'ACME Inc.',
					'address': '123 Acme St',
					'address2': '',
					'city': 'City',
					'state': 'CA',
					'country': 'USA',
					'zip': '12345'
				}
			]
		}";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<Teammate>(SINGLE_TEAMMATE_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.AddressLine1.ShouldBe("123 Acme St");
			result.AddressLine2.ShouldBeEmpty();
			result.City.ShouldBe("City");
			result.Company.ShouldBe("ACME Inc.");
			result.Country.ShouldBe("USA");
			result.Email.ShouldBe("teammate1@example.com");
			result.FirstName.ShouldBe("Jane");
			result.IsAdmin.ShouldBe(true);
			result.LastName.ShouldBe("Doe");
			result.Phone.ShouldBe("123-345-3453");
			result.Scopes.ShouldBeNull();
			result.State.ShouldBe("CA");
			result.Username.ShouldBe("teammate1");
			result.UserType.ShouldBe("owner");
			result.Website.ShouldBe("www.example.com");
			result.ZipCode.ShouldBe("12345");
		}

		[Fact]
		public async Task GetAccessRequestsAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("scopes/requests")).Respond("application/json", MULTIPLE_ACCESS_REQUESTS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			var result = await teammates.GetAccessRequestsAsync(10, 0, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public async Task DenyAccessRequestsAsync()
		{
			// Arrange
			var requestId = "abc123";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri("scopes/requests", requestId)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			await teammates.DenyAccessRequestAsync(requestId, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task AproveAccessRequestAsync()
		{
			// Arrange
			var requestId = "abc123";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri("scopes/requests", requestId)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			await teammates.ApproveAccessRequestAsync(requestId, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task ResendInvitationAsync()
		{
			// Arrange
			var token = "abc123";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "pending", token, "resend")).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			await teammates.ResendInvitationAsync(token, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task GetAllPendingInvitationsAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "pending")).Respond("application/json", MULTIPLE_INVITATIONS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			var result = await teammates.GetAllPendingInvitationsAsync(CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public async Task DeleteInvitationAsync()
		{
			// Arrange
			var token = "xxxxxxxx";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, "pending", token)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			await teammates.DeleteInvitationAsync(token, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task InviteTeammateAsync()
		{
			// Arrange
			var email = "dummy@example.com";
			var scopes = new[] { "mail.send", "alerts.create", "alerts.read" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_INVITATION_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			var result = await teammates.InviteTeammateAsync(email, scopes, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task InviteTeammateAsAdminAsync()
		{
			// Arrange
			var email = "dummy@example.com";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_INVITATION_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			var result = await teammates.InviteTeammateAsAdminAsync(email, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetAllTeammatesAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", MULTIPLE_TEAMMATES_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			var result = await teammates.GetAllTeammatesAsync(10, 0, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public async Task GetTeammateAsync()
		{
			// Arrange
			var username = "xxxxxxxx";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, username)).Respond("application/json", SINGLE_TEAMMATE_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			var result = await teammates.GetTeammateAsync(username, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task UpdateTeammatePermissionsAsync()
		{
			// Arrange
			var username = "xxxxxxxx";
			var scopes = new[] { "mail.send", "alerts.create", "alerts.read" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT, username)).Respond("application/json", SINGLE_TEAMMATE_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			var result = await teammates.UpdateTeammatePermissionsAsync(username, scopes, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task DeleteTeammateAsync()
		{
			// Arrange
			var username = "xxxxxxxx";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, username)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			await teammates.DeleteTeammateAsync(username, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}
	}
}
