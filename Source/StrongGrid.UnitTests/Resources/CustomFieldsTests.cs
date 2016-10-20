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
	public class CustomFieldsTests
	{
		private const string ENDPOINT = "/contactdb/custom_fields";

		[TestMethod]
		public void Create()
		{
			// Arrange
			var name = "customfield1";
			var type = FieldType.Text;

			var apiResponse = @"{
				'id': 1,
				'name': 'customfield1',
				'type': 'text'
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var customFields = new CustomFields(mockClient.Object);

			// Act
			var result = customFields.CreateAsync(name, type, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Id);
			Assert.AreEqual(name, result.Name);
			Assert.AreEqual(type, result.Type);
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var fieldId = 1;

			var apiResponse = @"{
				'id': 1,
				'name': 'customfield1',
				'type': 'text'
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(ENDPOINT + "/" + fieldId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var customFields = new CustomFields(mockClient.Object);

			// Act
			var result = customFields.GetAsync(fieldId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Id);
			Assert.AreEqual("customfield1", result.Name);
			Assert.AreEqual(FieldType.Text, result.Type);
		}

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			var apiResponse = @"{
				'custom_fields': [
					{
						'id': 1,
						'name': 'birthday',
						'type': 'date'
					},
					{
						'id': 2,
						'name': 'middle_name',
						'type': 'text'
					},
					{
						'id': 3,
						'name': 'favorite_number',
						'type': 'number'
					}
				]
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var customFields = new CustomFields(mockClient.Object);

			// Act
			var result = customFields.GetAllAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Length);
			Assert.AreEqual("birthday", result[0].Name);
			Assert.AreEqual("middle_name", result[1].Name);
			Assert.AreEqual("favorite_number", result[2].Name);
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			var fieldId = 1;

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync(ENDPOINT + "/" + fieldId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Accepted));

			var customFields = new CustomFields(mockClient.Object);

			// Act
			customFields.DeleteAsync(fieldId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void GetReservedFields()
		{
			// Arrange
			var apiResponse = @"{
				'reserved_fields': [
					{
						'name': 'first_name',
						'type': 'text'
					},
					{
						'name': 'last_name',
						'type': 'text'
					},
					{
						'name': 'email',
						'type': 'text'
					},
					{
						'name': 'created_at',
						'type': 'date'
					},
					{
						'name': 'updated_at',
						'type': 'date'
					},
					{
						'name': 'last_emailed',
						'type': 'date'
					},
					{
						'name': 'last_clicked',
						'type': 'date'
					},
					{
						'name': 'last_opened',
						'type': 'date'
					},
					{
						'name': 'my_custom_field',
						'type': 'text'
					}
				]
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync("/contactdb/reserved_fields", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var customFields = new CustomFields(mockClient.Object);

			// Act
			var result = customFields.GetReservedFieldsAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(9, result.Length);
			Assert.AreEqual("first_name", result[0].Name);
			Assert.AreEqual("last_name", result[1].Name);
			Assert.AreEqual("email", result[2].Name);
		}
	}
}
