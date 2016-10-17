using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class UnsubscribeGroupsTests
	{
		private const string ENDPOINT = "/asm/groups";

		[TestMethod]
		public void Create()
		{
			// Arrange
			var name = "Product Suggestions";
			var description = "Suggestions for products our users might like.";
			var isDefault = true;

			var apiResponse = @"{
				'name': 'Product Suggestions',
				'description': 'Suggestions for products our users might like.',
				'is_default': true
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var goups = new UnsubscribeGroups(mockClient.Object);

			// Act
			var result = goups.CreateAsync(name, description, isDefault, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(name, result.Name);
			Assert.AreEqual(description, result.Description);
			Assert.AreEqual(isDefault, result.IsDefault);
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var groupId = 100;
			var apiResponse = @"{
				'id': 100,
				'name': 'Newsletters',
				'description': 'Our monthly newsletter.',
				'last_email_sent_at': null,
				'is_default': true,
				'unsubscribes': 400
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(ENDPOINT + "/" + groupId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var groups = new UnsubscribeGroups(mockClient.Object);

			// Act
			var result = groups.GetAsync(groupId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(groupId, result.Id);
			Assert.AreEqual("Newsletters", result.Name);
		}

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			var apiResponse = @"[
				{
					'id': 100,
					'name': 'Newsletters',
					'description': 'Our monthly newsletter.',
					'last_email_sent_at': null,
					'is_default': true,
					'unsubscribes': 400
				},
				{
					'id': 101,
					'name': 'Alerts',
					'description 2': 'Emails triggered by user-defined rules.',
					'last_email_sent_at': null,
					'is_default': false,
					'unsubscribes': 1
				}
			]";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var groups = new UnsubscribeGroups(mockClient.Object);

			// Act
			var result = groups.GetAllAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual(100, result[0].Id);
			Assert.AreEqual(101, result[1].Id);
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			var groupId = 100;

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync(ENDPOINT + "/" + groupId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var groups = new UnsubscribeGroups(mockClient.Object);

			// Act
			groups.DeleteAsync(groupId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Update()
		{
			// Arrange
			var groupId = 103;
			var name = "Item Suggestions";
			var description = "Suggestions for items our users might like.";

			var apiResponse = @"{
				'id':103,
				'name': 'Item Suggestions',
				'description': 'Suggestions for items our users might like.'
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PatchAsync(ENDPOINT + "/" + groupId, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var groups = new UnsubscribeGroups(mockClient.Object);

			// Act
			var result = groups.UpdateAsync(groupId, name, description, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(groupId, result.Id);
			Assert.AreEqual(name, result.Name);
			Assert.AreEqual(description, result.Description);
		}
	}
}
