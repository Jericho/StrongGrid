using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class UserTests
	{
		private const string ENDPOINT = "/user/profile";

		[TestMethod]
		public void GetProfile()
		{
			// Arrange
			var apiResponse = @"{
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

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var user = new User(mockClient.Object);

			// Act
			var result = user.GetProfileAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual("814 West Chapman Avenue", result.Address);
			Assert.AreEqual("Orange", result.City);
		}

		[TestMethod]
		public void GetAccount()
		{
			// Arrange
			var apiResponse = @"{
				'type': 'free',
				'reputation': 99.7
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync("/user/account", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var user = new User(mockClient.Object);

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

			var apiResponse = @"{
				'address': '814 West Chapman Avenue',
				'city': 'New York',
				'company': 'SendGrid',
				'country': 'US',
				'first_name': 'Test',
				'last_name': 'User',
				'phone': '555-555-5555',
				'state': 'CA',
				'website': 'http://www.sendgrid.com',
				'zip': '92868'
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PatchAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var user = new User(mockClient.Object);

			// Act
			var result = user.UpdateProfileAsync(null, city, null, null, null, null, null, null, null, null, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(city, result.City);
		}

		[TestMethod]
		public void GetEmail()
		{
			// Arrange
			var apiResponse = @"{
				'email': 'test@example.com'
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync("/user/email", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var user = new User(mockClient.Object);

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
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PutAsync("/user/email", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var user = new User(mockClient.Object);

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

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync("/user/username", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var user = new User(mockClient.Object);

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
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PutAsync("/user/username", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var user = new User(mockClient.Object);

			// Act
			var result = user.UpdateUsernameAsync(username, CancellationToken.None).Result;

			// Assert
			Assert.AreEqual(username, result);
		}
	}
}
