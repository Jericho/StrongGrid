using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.UnitTests;
using StrongGrid.Utilities;
using System.Collections.Generic;
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
		private const string MONITOR_SETTINGS_JSON = @"{
		  'email': 'test@example.com',
		  'frequency': 500
		}";

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
			result.EmailAddress.ShouldBe("Test@example.com");
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
			result.EmailAddress.ShouldBe("Test@example.com");
			result.Password.ShouldBe("somepass");
			result.IsDisabled.ShouldBe(false);
			result.Ips.ShouldBe(new[] { "1.1.1.1", "2.2.2.2" });
		}

		[Fact]
		public void Parse_json_MonitorSettings()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<MonitorSettings>(MONITOR_SETTINGS_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.EmailAddress.ShouldBe("test@example.com");
			result.Frequency.ShouldBe(500);
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
		public void Get()
		{
			// Arrange
			var username = "TestUser";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, username)).Respond("application/json", SINGLE_SUBUSER_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var subusers = new Subusers(client);

			// Act
			var result = subusers.GetAsync(username, CancellationToken.None).Result;

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
		public void Update_disabled()
		{
			// Arrange
			var username = "someuser";
			var disabled = true;
			var ips = default(Parameter<IEnumerable<string>>);

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT, username)).Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var subusers = new Subusers(client);

			// Act
			subusers.UpdateAsync(username, disabled, ips, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void Update_ips()
		{
			// Arrange
			var username = "someuser";
			var disabled = default(Parameter<bool>);
			var ips = new[] { "1.1.1.1", "2.2.2.2" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod( "PUT" ), Utils.GetSendGridApiUri(ENDPOINT, username, "ips")).Respond("application/json", MULTIPLE_IPS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var subusers = new Subusers(client);

			// Act
			subusers.UpdateAsync(username, disabled, ips, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void GetMonitorSettings()
		{
			// Arrange
			var username = "someuser";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, username, "monitor")).Respond("application/json", MONITOR_SETTINGS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var subusers = new Subusers(client);

			// Act
			var result = subusers.GetMonitorSettingsAsync(username, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void CreateMonitorSettings()
		{
			// Arrange
			var username = "someuser";
			var email = "test@example.com";
			var frequency = 500;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, username, "monitor")).Respond("application/json", MONITOR_SETTINGS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var subusers = new Subusers(client);

			// Act
			var result = subusers.CreateMonitorSettingsAsync(username, email, frequency, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void UpdateMonitorSettings_email()
		{
			// Arrange
			var username = "someuser";
			var email = "test@example.com";
			var frequency = default(Parameter<int>);

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Put, Utils.GetSendGridApiUri(ENDPOINT, username, "monitor")).Respond("application/json", MONITOR_SETTINGS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var subusers = new Subusers(client);

			// Act
			var result = subusers.UpdateMonitorSettingsAsync(username, email, frequency, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void UpdateMonitorSettings_frequency()
		{
			// Arrange
			var username = "someuser";
			var email = default(Parameter<string>);
			var frequency = 500;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Put, Utils.GetSendGridApiUri(ENDPOINT, username, "monitor")).Respond("application/json", MONITOR_SETTINGS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var subusers = new Subusers(client);

			// Act
			var result = subusers.UpdateMonitorSettingsAsync(username, email, frequency, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void DeleteMonitorSettings()
		{
			// Arrange
			var username = "someuser";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, username, "monitor")).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var subusers = new Subusers(client);

			// Act
			subusers.DeleteMonitorSettingsAsync(username, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}
	}
}
