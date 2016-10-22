using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class ListsTests
	{
		private const string ENDPOINT = "/contactdb/lists";

		[TestMethod]
		public void Create()
		{
			// Arrange
			var name = "listname";

			var apiResponse = @"{
				'id': 1,
				'name': 'listname',
				'recipient_count': 0
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var lists = new Lists(mockClient.Object);

			// Act
			var result = lists.CreateAsync(name, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Id);
			Assert.AreEqual("listname", result.Name);
			Assert.AreEqual(0, result.RecipientCount);
		}

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			var apiResponse = @"{
				'lists': [
					{
						'id': 1,
						'name': 'the jones',
						'recipient_count': 1
					}
				]
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var lists = new Lists(mockClient.Object);

			// Act
			var result = lists.GetAllAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual(1, result[0].Id);
			Assert.AreEqual("the jones", result[0].Name);
			Assert.AreEqual(1, result[0].RecipientCount);
		}

		[TestMethod]
		public void Delete_single()
		{
			// Arrange
			var listId = 1;

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync(ENDPOINT + "/" + listId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var lists = new Lists(mockClient.Object);

			// Act
			lists.DeleteAsync(listId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Delete_multiple()
		{
			// Arrange
			var listIds = new long[] { 1, 2, 3 };

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync(ENDPOINT, It.Is<JArray>(o => o.Count == 3), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var lists = new Lists(mockClient.Object);

			// Act
			lists.DeleteAsync(listIds, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var listId = 1;

			var apiResponse = @"{
				'id': 1,
				'name': 'listname',
				'recipient_count': 0
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(ENDPOINT + "/" + listId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var lists = new Lists(mockClient.Object);

			// Act
			var result = lists.GetAsync(listId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Id);
			Assert.AreEqual("listname", result.Name);
			Assert.AreEqual(0, result.RecipientCount);
		}

		[TestMethod]
		public void Update()
		{
			// Arrange
			var listId = 5;
			var name = "newlistname";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PatchAsync(ENDPOINT + "/" + listId, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var lists = new Lists(mockClient.Object);

			// Act
			lists.UpdateAsync(listId, name, CancellationToken.None).Wait();

			// Assert
		}

		[TestMethod]
		public void GetRecipients()
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
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(ENDPOINT + "/" + listId + "/recipients?page_size=" + recordsPerPage + "&page=" + page, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var lists = new Lists(mockClient.Object);

			// Act
			var result = lists.GetRecipientsAsync(listId, recordsPerPage, page, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("e@example.com", result[0].Email);
		}

		[TestMethod]
		public void AddRecipient()
		{
			// Arrange
			var listId = 1;
			var contactId = "abc123";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync(ENDPOINT + "/" + listId + "/recipients/" + contactId, (JObject)null, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));

			var lists = new Lists(mockClient.Object);

			// Act
			lists.AddRecipientAsync(listId, contactId, CancellationToken.None).Wait();

			// Assert
		}

		[TestMethod]
		public void RemoveRecipient()
		{
			// Arrange
			var listId = 1;
			var contactId = "abc123";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync(ENDPOINT + "/" + listId + "/recipients/" + contactId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var lists = new Lists(mockClient.Object);

			// Act
			lists.RemoveRecipientAsync(listId, contactId, CancellationToken.None).Wait();

			// Assert
		}

		[TestMethod]
		public void AddRecipients()
		{
			// Arrange
			var listId = 1;
			var contactIds = new[] { "abc123", "def456" };

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync(ENDPOINT + "/" + listId + "/recipients", It.Is<JArray>(o => o.Count == 2), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));

			var lists = new Lists(mockClient.Object);

			// Act
			lists.AddRecipientsAsync(listId, contactIds, CancellationToken.None).Wait();

			// Assert
		}
	}
}
