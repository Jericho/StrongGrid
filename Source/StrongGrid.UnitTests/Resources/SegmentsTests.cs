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
	public class SegmentsTests
	{
		private const string ENDPOINT = "/contactdb/segments";

		[TestMethod]
		public void Create()
		{
			// Arrange
			var name = "Last Name Miller";
			var listId = 4;
			var conditions = new[]
			{
				new SearchCondition
				{
					Field = "last_name",
					Value= "Miller",
					Operator = ConditionOperator.Equal,
					LogicalOperator = LogicalOperator.None
				},
				new SearchCondition
				{
					Field = "last_clicked",
					Value = "01/02/2015",
					Operator = ConditionOperator.GreatherThan,
					LogicalOperator = LogicalOperator.And
				},
				new SearchCondition
				{
					Field = "clicks.campaign_identifier",
					Value = "513",
					Operator = ConditionOperator.Equal,
					LogicalOperator = LogicalOperator.Or
				}
			};

			var apiResponse = @"{
				'id': 1,
				'name': 'Last Name Miller',
				'list_id': 4,
				'conditions': [
					{
					'field': 'last_name',
					'value': 'Miller',
					'operator': 'eq',
					'and_or': ''

					},
					{
					'field': 'last_clicked',
					'value': '01/02/2015',
					'operator': 'gt',
					'and_or': 'and'
					},
					{
					'field': 'clicks.campaign_identifier',
					'value': '513',
					'operator': 'eq',
					'and_or': 'or'
					}
				]
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var segments = new Segments(mockClient.Object);

			// Act
			var result = segments.CreateAsync(name, listId, conditions, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Id);
		}

		[TestMethod]
		public void GetAll()
		{
			// Arrange
			var apiResponse = @"{
				'segments': [
					{
						'id': 1,
						'name': 'Last Name Miller',
						'list_id': 4,
						'conditions': [
							{
								'field': 'last_name',
								'value': 'Miller',
								'operator': 'eq',
								'and_or': ''
							}
						],
						'recipient_count': 1
					}
				]
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var segments = new Segments(mockClient.Object);

			// Act
			var result = segments.GetAllAsync(CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("Last Name Miller", result[0].Name);
		}

		[TestMethod]
		public void Get()
		{
			// Arrange
			var segmentId = 1;
			var apiResponse = @"{
				'id': 1,
				'name': 'Last Name Miller',
				'list_id': 4,
				'conditions': [
					{
						'field': 'last_name',
						'value': 'Miller',
						'operator': 'eq',
						'and_or': ''
					}
				],
				'recipient_count': 1
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(ENDPOINT + "/" + segmentId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var segments = new Segments(mockClient.Object);

			// Act
			var result = segments.GetAsync(segmentId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Id);
			Assert.AreEqual("Last Name Miller", result.Name);
		}

		[TestMethod]
		public void Update()
		{
			// Arrange
			var segmentId = 5;
			var name = "The Millers";
			var listId = 5;
			var conditions = new[]
			{
				new SearchCondition
				{
					Field = "last_name",
					Value= "Miller",
					Operator = ConditionOperator.Equal,
					LogicalOperator = LogicalOperator.None
				}
			};

			var apiResponse = @"{
				'id': 5,
				'name': 'The Millers',
				'list_id': 5,
				'conditions': [
					{
						'field': 'last_name',
						'value': 'Miller',
						'operator': 'eq',
						'and_or': ''
					}
				],
				'recipient_count': 1
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PatchAsync(ENDPOINT + "/" + segmentId, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var segments = new Segments(mockClient.Object);

			// Act
			var result = segments.UpdateAsync(segmentId, name, listId, conditions, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(5, result.Id);
			Assert.AreEqual("The Millers", result.Name);
		}

		[TestMethod]
		public void Delete_and_preserve_contacts()
		{
			// Arrange
			var segmentId = 1;
			var deleteContacts = false;

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync(ENDPOINT + "/" + segmentId + "?delete_contacts=false", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var segments = new Segments(mockClient.Object);

			// Act
			segments.DeleteAsync(segmentId, deleteContacts, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Delete_and_delete_contacts()
		{
			// Arrange
			var segmentId = 1;
			var deleteContacts = true;

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync(ENDPOINT + "/" + segmentId + "?delete_contacts=true", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

			var segments = new Segments(mockClient.Object);

			// Act
			segments.DeleteAsync(segmentId, deleteContacts, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void GetRecipients()
		{
			// Arrange
			var segmentId = 1;
			var recordsPerPage = 25;
			var page = 1;

			var apiResponse = @"{
				'recipients': [
					{
						'created_at': 1422313607,
						'email': 'jones@example.com',
						'first_name': null,
						'id': 'YUBh',
						'last_clicked': null,
						'last_emailed': null,
						'last_name': 'Jones',
						'last_opened': null,
						'updated_at': 1422313790,
						'custom_fields': [
							{
								'id': 23,
								'name': 'pet',
								'value': 'Indiana',
								'type': 'text'
							}
						]
					}
				]
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync(ENDPOINT + "/" + segmentId + "/recipients?page_size=" + recordsPerPage + "&page=" + page, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var segments = new Segments(mockClient.Object);

			// Act
			var result = segments.GetRecipientsAsync(segmentId, recordsPerPage, page, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("jones@example.com", result[0].Email);
		}
	}
}
