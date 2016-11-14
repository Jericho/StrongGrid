using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class UserTests
	{
		#region FIELDS

		private const string ENDPOINT = "/user/profile";
		private MockRepository _mockRepository;
		private Mock<IClient> _mockClient;

		private const string SINGLE_PROFILE_JSON = @"{
			'address': '814 West Chapman Avenue',
			'city': 'Orange',
			'company': 'SendGrid',
			'country': 'US',
			'first_name': 'Test',
			'last_name': 'User',
			'phone': '555-555-5555',
			'state': 'CA',
			'website': 'http://www.sendgrid.com',
			'zip': '92868'
		}";
		private const string MULTIPLE__JSON = @"[
		]";

		#endregion

		private User CreateUser()
		{
			return new User(_mockClient.Object, ENDPOINT);
		}

		[TestInitialize]
		public void TestInitialize()
		{
			_mockRepository = new MockRepository(MockBehavior.Strict);
			_mockClient = _mockRepository.Create<IClient>();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			_mockRepository.VerifyAll();
		}

		[TestMethod]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<UserProfile>(SINGLE_PROFILE_JSON);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual("814 West Chapman Avenue", result.Address);
			Assert.AreEqual("Orange", result.City);
			Assert.AreEqual("SendGrid", result.Company);
			Assert.AreEqual("US", result.Country);
			Assert.AreEqual("Test", result.FirstName);
			Assert.AreEqual("User", result.LastName);
			Assert.AreEqual("555-555-5555", result.Phone);
			Assert.AreEqual("CA", result.State);
			Assert.AreEqual("http://www.sendgrid.com", result.Website);
			Assert.AreEqual("92868", result.ZipCode);
		}

		[TestMethod]
		public void GetProfile()
		{
			// Arrange
			_mockClient
				.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_PROFILE_JSON) })
				.Verifiable();

			var user = CreateUser();

			// Act
			var result = user.GetProfileAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetAccount()
		{
			// Arrange
			var apiResponse = @"{
				'type': 'free',
				'reputation': 99.7
			}";

			_mockClient
				.Setup(c => c.GetAsync("/user/account", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var user = CreateUser();

			// Act
			var result = user.GetAccountAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual("free", result.Type);
			Assert.AreEqual(99.7F, result.Reputation);
		}

		[TestMethod]
		public void UpdateProfile()
		{
			// Arrange
			var city = "New York";

			_mockClient
				.Setup(c => c.PatchAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_PROFILE_JSON) })
				.Verifiable();

			var user = CreateUser();

			// Act
			var result = user.UpdateProfileAsync(null, city, null, null, null, null, null, null, null, null, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetEmail()
		{
			// Arrange
			var apiResponse = @"{
				'email': 'test@example.com'
			}";

			_mockClient
				.Setup(c => c.GetAsync("/user/email", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var user = CreateUser();

			// Act
			var result = user.GetEmailAsync(CancellationToken.None).Result;

			// Assert
			Assert.AreEqual("test@example.com", result);
		}

		[TestMethod]
		public void UpdateEmail()
		{
			// Arrange
			var email = "test@example.com";

			var apiResponse = @"{
				'email': 'test@example.com'
			}";

			_mockClient
				.Setup(c => c.PutAsync("/user/email", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var user = CreateUser();

			// Act
			var result = user.UpdateEmailAsync(email, CancellationToken.None).Result;

			// Assert
			Assert.AreEqual("test@example.com", result);
		}

		[TestMethod]
		public void GetUsername()
		{
			// Arrange
			var apiResponse = @"{
				'username': 'test_username'
			}";

			_mockClient
				.Setup(c => c.GetAsync("/user/username", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var user = CreateUser();

			// Act
			var result = user.GetUsernameAsync(CancellationToken.None).Result;

			// Assert
			Assert.AreEqual("test_username", result);
		}

		[TestMethod]
		public void UpdateUsername()
		{
			// Arrange
			var username = "test_username";

			var apiResponse = @"{
				'username': 'test_username'
			}";

			_mockClient
				.Setup(c => c.PutAsync("/user/username", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var user = CreateUser();

			// Act
			var result = user.UpdateUsernameAsync(username, CancellationToken.None).Result;

			// Assert
			Assert.AreEqual(username, result);
		}

		[TestMethod]
		public void GetCredits()
		{
			// Arrange
			var apiResponse = @"{
				'remain': 200,
				'total': 200,
				'overage': 0,
				'used': 0,
				'last_reset': '2013-01-01',
				'next_reset': '2013-02-01',
				'reset_frequency': 'monthly'
			}";

			_mockClient
				.Setup(c => c.GetAsync("/user/credits", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
				.Verifiable();

			var user = CreateUser();

			// Act
			var result = user.GetCreditsAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(200, result.Remaining);
			Assert.AreEqual(200, result.Total);
			Assert.AreEqual(0, result.Overage);
			Assert.AreEqual(0, result.Used);
			Assert.AreEqual("monthly", result.ResetFrequency);
			Assert.AreEqual(new DateTime(2013, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), result.LastReset);
			Assert.AreEqual(new DateTime(2013, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), result.NextReset);
		}

		[TestMethod]
		public void UpdatePassword()
		{
			// Arrange
			var oldPassword = "azerty";
			var newPassword = "qwerty";

			_mockClient
				.Setup(c => c.PutAsync("/user/password", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
				.Verifiable();

			var user = CreateUser();

			// Act
			user.UpdatePasswordAsync(oldPassword, newPassword, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}
	}
}
