using Newtonsoft.Json;
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
	public class TeammatesTests
	{
		#region FIELDS

		private const string BASE_URI = "https://api.sendgrid.com";
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
		public void GetAccessRequestsAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("scopes/requests")).Respond("application/json", MULTIPLE_ACCESS_REQUESTS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			var result = teammates.GetAccessRequestsAsync(10, 0, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public void DenyAccessRequestsAsync()
		{
			// Arrange
			var requestId = "abc123";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri("scopes/requests", requestId)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			teammates.DenyAccessRequestAsync(requestId, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void AproveAccessRequestAsync()
		{
			// Arrange
			var requestId = "abc123";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri("scopes/requests", requestId)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			teammates.ApproveAccessRequestAsync(requestId, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void ResendInvitationAsync()
		{
			// Arrange
			var token = "abc123";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "pending", token, "resend")).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			teammates.ResendInvitationAsync(token, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void GetAllPendingInvitationsAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "pending")).Respond("application/json", MULTIPLE_INVITATIONS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			var result = teammates.GetAllPendingInvitationsAsync(CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public void DeleteInvitationAsync()
		{
			// Arrange
			var token = "xxxxxxxx";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, "pending", token)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			teammates.DeleteInvitationAsync(token, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void InviteTeammateAsync()
		{
			// Arrange
			var email = "dummy@example.com";
			var scopes = new[] { "mail.send", "alerts.create", "alerts.read" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_INVITATION_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			var result = teammates.InviteTeammateAsync(email, scopes, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void InviteTeammateAsAdminAsync()
		{
			// Arrange
			var email = "dummy@example.com";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_INVITATION_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			var result = teammates.InviteTeammateAsAdminAsync(email, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetAllTeammatesAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", MULTIPLE_TEAMMATES_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			var result = teammates.GetAllTeammatesAsync(10, 0, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public void GetTeammateAsync()
		{
			// Arrange
			var username = "xxxxxxxx";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, username)).Respond("application/json", SINGLE_TEAMMATE_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			var result = teammates.GetTeammateAsync(username, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void UpdateTeammatePermissionsAsync()
		{
			// Arrange
			var username = "xxxxxxxx";
			var scopes = new[] { "mail.send", "alerts.create", "alerts.read" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT, username)).Respond("application/json", SINGLE_TEAMMATE_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			var result = teammates.UpdateTeammatePermissionsAsync(username, scopes, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void DeleteTeammateAsync()
		{
			// Arrange
			var username = "xxxxxxxx";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, username)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var teammates = new Teammates(client);

			// Act
			teammates.DeleteTeammateAsync(username, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

	}
}
