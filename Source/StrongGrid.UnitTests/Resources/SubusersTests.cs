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
	public class SubusersTests
	{
		#region FIELDS

		private const string BASE_URI = "https://api.sendgrid.com";
		private const string ENDPOINT = "subusers";

		private const string SINGLE_SUBUSER_JSON = @"{
			'id': 1,
			'username': 'TestUser',
			'email': 'Test@example.com',
			'disabled': true
		}";
		private const string SINGLE_SUBUSER_CREATED_JSON = @"{
			'id': 1,
			'username': 'TestUser',
			'password': 'somepass',
			'email': 'Test@example.com',
			'ips': [
				'1.1.1.1',
				'2.2.2.2'
			]
		}";
		private const string MULTIPLE_SUBUSER_JSON = @"[
			  {
				'id': 1,
				'username': 'TestUser',
				'email': 'Test@example.com',
				'disabled': false
			  },
			  {
				'id': 2,
				'username': 'John',
				'email': 'John@example.com',
				'disabled': true
			  }
		]";
		private const string MULTIPLE_IPS_JSON = @"[
			  '1.1.1.1',
			  '2.2.2.2'
		]";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<Subuser>(SINGLE_SUBUSER_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.Id.ShouldBe(1);
			result.Username.ShouldBe("TestUser");
			result.Email.ShouldBe("Test@example.com");
			result.IsDisabled.ShouldBe(true);
		}

		[Fact]
		public void Parse_json_created()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<Subuser>(SINGLE_SUBUSER_CREATED_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.Id.ShouldBe(1);
			result.Username.ShouldBe("TestUser");
			result.Email.ShouldBe("Test@example.com");
			result.Password.ShouldBe("somepass");
			result.IsDisabled.ShouldBe(false);
			result.Ips.ShouldBe(new[] { "1.1.1.1", "2.2.2.2" });
		}

		[Fact]
		public void Create()
		{
			// Arrange
			var username = "TestUser";
			var password = "somepass";
			var email = "Test@example.com";
			var ips = new[] { "1.1.1.1", "2.2.2.2" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_SUBUSER_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var subusers = new Subusers(client);

			// Act
			var result = subusers.CreateAsync(username, email, password, ips, CancellationToken.None).Result;

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
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", MULTIPLE_SUBUSER_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var subusers = new Subusers(client);

			// Act
			var result = subusers.GetAllAsync(null, 10, 0, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public void Delete()
		{
			// Arrange
			var keyId = "xxxxxxxx";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, keyId)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var subusers = new Subusers(client);

			// Act
			subusers.DeleteAsync(keyId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void Update()
		{
			// Arrange
			var username = "someuser";
			var disabled = true;
			var ips = new[] { "1.1.1.1", "2.2.2.2" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod( "PATCH" ), Utils.GetSendGridApiUri(ENDPOINT, username)).Respond(HttpStatusCode.NoContent);
			mockHttp.Expect(new HttpMethod( "PUT" ), Utils.GetSendGridApiUri(ENDPOINT, username, "ips")).Respond("application/json", MULTIPLE_IPS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var subusers = new Subusers(client);

			// Act
			subusers.UpdateAsync(username, disabled, ips, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}
	}
}
