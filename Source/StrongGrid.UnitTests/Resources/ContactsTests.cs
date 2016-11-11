using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Linq;

namespace StrongGrid.Resources.UnitTests
{
	[TestClass]
	public class ContactsTests
	{
		private const string ENDPOINT = "/contactdb/recipients";

		[TestMethod]
		public void Create_success()
		{
			// Arrange
			var email = "Jane@example.com";
			var firstName = "Jane";
			var lastName = "Doe";

			var apiResponse = @"{
				'error_count': 0,
				'error_indices': [
				],
				'unmodified_indices': [
				],
				'new_count': 1,
				'persisted_recipients': [
					'am9uZXNAZXhhbXBsZS5jb20='
				],
				'updated_count': 0
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync(ENDPOINT, It.Is<JArray>(o => o.Count == 1), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var contacts = new Contacts(mockClient.Object);

			// Act
			var result = contacts.CreateAsync(email, firstName, lastName, null, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		[ExpectedException(typeof(AggregateException))]
		public void Create_failure()
		{
			// Arrange
			var email = "invalid_email";
			var firstName = "Jane";
			var lastName = "Doe";

			var apiResponse = @"{
				'error_count': 1,
				'error_indices': [
					0
				],
				'unmodified_indices': [
				],
				'new_count': 0,
				'persisted_recipients': [
				],
				'updated_count': 0,
				'errors': [
					{
						'message': 'Invalid email.',
						'error_indices': [
							0
						]
					}
				]
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync(ENDPOINT, It.Is<JArray>(o => o.Count == 1), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var contacts = new Contacts(mockClient.Object);

			// Act
			var result = contacts.CreateAsync(email, firstName, lastName, null, CancellationToken.None).Result;
		}

		[TestMethod]
		public void Import()
		{
			// Arrange
			var records = new[]
			{
				new Contact("jones@example.com", null, "Jones", new Field[] { new Field<string>("pet", "Fluffy"), new Field<long>("age", 25) }),
				new Contact("miller@example.com", null, "Miller", new Field[] { new Field<string>("pet", "FrouFrou"), new Field<long>("age", 32) }),
				new Contact("invalid email", null, "Smith", new Field[] { new Field<string>("pet", "Spot"), new Field<long>("age", 17) })
			};

			var apiResponse = @"{
				'error_count': 1,
				'error_indices': [
					2
				],
				'unmodified_indices': [
					3
				],
				'new_count': 2,
				'persisted_recipients': [
					'YUBh',
					'bWlsbGVyQG1pbGxlci50ZXN0'
				],
				'updated_count': 0,
				'errors': [
					{
						'message': 'Invalid email.',
						'error_indices': [
							2
						]
					}
				]
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync(ENDPOINT, It.Is<JArray>(o => o.Count == 3), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var contacts = new Contacts(mockClient.Object);

			// Act
			var result = contacts.ImportAsync(records, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.ErrorCount);
			CollectionAssert.AreEqual(new[] { 2 }, result.ErrorIndices);
			Assert.AreEqual(2, result.NewCount);
			Assert.AreEqual(2, result.PersistedRecipients.Length);
			Assert.AreEqual(0, result.UpdatedCount);
			Assert.AreEqual(1, result.Errors.Length);
			Assert.AreEqual("Invalid email.", result.Errors[0].Message);
			CollectionAssert.AreEqual(new[] { 2 }, result.Errors[0].ErrorIndices);
		}

		[TestMethod]
		public void Update_success()
		{
			// Arrange
			var email = "jones@example.com";
			var lastName = "Jones";

			var apiResponse = @"{
				'error_count': 0,
				'error_indices': [
				],
				'unmodified_indices': [
					1
				],
				'new_count': 0,
				'persisted_recipients': [
					'am9uZXNAZXhhbXBsZS5jb20='
				],
				'updated_count': 1
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PatchAsync(ENDPOINT, It.Is<JArray>(o => o.Count == 1), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var contacts = new Contacts(mockClient.Object);

			// Act
			contacts.UpdateAsync(email, null, lastName, null, CancellationToken.None).Wait();

			// Assert
		}

		[TestMethod]
		[ExpectedException(typeof(AggregateException))]
		public void Update_failure()
		{
			// Arrange
			var email = "invalid_email";
			var lastName = "Jones";

			var apiResponse = @"{
				'error_count': 1,
				'error_indices': [
					0
				],
				'unmodified_indices': [
				],
				'new_count': 0,
				'persisted_recipients': [
				],
				'updated_count': 0,
				'errors': [
					{
						'message': 'Invalid email.',
						'error_indices': [
							0
						]
					}
				]
			}";
			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PatchAsync(ENDPOINT, It.Is<JArray>(o => o.Count == 1), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var contacts = new Contacts(mockClient.Object);

			// Act
			contacts.UpdateAsync(email, null, lastName, null, CancellationToken.None).Wait();
		}

		[TestMethod]
		public void Get_single()
		{
			// Arrange
			var contactId = "YUBh";
			var apiResponse = @"{
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
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync($"{ENDPOINT}/{contactId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var contacts = new Contacts(mockClient.Object);

			// Act
			var result = contacts.GetAsync(contactId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual("Jones", result.LastName);
			Assert.AreEqual("jones@example.com", result.Email);
			Assert.AreEqual(1, result.CustomFields.Length);
			Assert.AreEqual("pet", result.CustomFields[0].Name);
			Assert.AreEqual("Indiana", ((Field<string>)result.CustomFields[0]).Value);
		}

		[TestMethod]
		public void Get_multiple()
		{
			// Arrange
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
							},
							{
								'id': 24,
								'name': 'age',
								'value': '43',
								'type': 'number'
							}
						]
					}
				]
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync($"{ENDPOINT}?page_size={recordsPerPage}&page={page}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var contacts = new Contacts(mockClient.Object);

			// Act
			var result = contacts.GetAsync(recordsPerPage, page, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("jones@example.com", result[0].Email);
			Assert.AreEqual(2, result[0].CustomFields.Length);
			Assert.AreEqual("pet", result[0].CustomFields[0].Name);
			Assert.AreEqual("Indiana", ((Field<string>)result[0].CustomFields[0]).Value);
			Assert.AreEqual("age", result[0].CustomFields[1].Name);
			Assert.AreEqual(43, ((Field<long?>)result[0].CustomFields[1]).Value);
		}

		[TestMethod]
		public void Delete_single()
		{
			// Arrange
			var contactId = "recipient_id1";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync(ENDPOINT, It.Is<JArray>(o => o.Count == 1), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var contacts = new Contacts(mockClient.Object);

			// Act
			contacts.DeleteAsync(contactId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void Delete_multiple()
		{
			// Arrange
			var contactIds = new[] { "recipient_id1", "recipient_id2" };

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.DeleteAsync(ENDPOINT, It.Is<JArray>(o => o.Count == 2), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			var contacts = new Contacts(mockClient.Object);

			// Act
			contacts.DeleteAsync(contactIds, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[TestMethod]
		public void GetBillableCount()
		{
			// Arrange
			var apiResponse = @"{
				'recipient_count': 2
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync($"{ENDPOINT}/billable_count", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var contacts = new Contacts(mockClient.Object);

			// Act
			var result = contacts.GetBillableCountAsync(CancellationToken.None).Result;

			// Assert
			Assert.AreEqual(2, result);
		}

		[TestMethod]
		public void GetTotalCount()
		{
			// Arrange
			var apiResponse = @"{
				'recipient_count': 3
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.GetAsync($"{ENDPOINT}/count", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var contacts = new Contacts(mockClient.Object);

			// Act
			var result = contacts.GetTotalCountAsync(CancellationToken.None).Result;

			// Assert
			Assert.AreEqual(3, result);
		}

		[TestMethod]
		public void Search()
		{
			// Arrange
			var listId = 4;
			var conditions = new[]
			{
				new SearchCondition
				{
					Field = "last_name",
					Value = "Miller",
					Operator = ConditionOperator.Equal,
					LogicalOperator = LogicalOperator.None
				},
				new SearchCondition
				{
					Field = "last_click",
					Value = "01/02/2015",
					Operator = ConditionOperator.GreatherThan,
					LogicalOperator = LogicalOperator.And
				},
				new SearchCondition
				{
					Field = "clicks.campaign_identifier",
					Value = "513",
					Operator = ConditionOperator.GreatherThan,
					LogicalOperator = LogicalOperator.Or
				}
			};
			var apiResponse = @"{
				'recipients': [
					{
						'created_at': 1422313607,
						'email': 'jones@example.com',
						'first_name': null,
						'id': 'YUBh',
						'last_clicked': 12345,
						'last_emailed': null,
						'last_name': 'Miller',
						'last_opened': null,
						'updated_at': 1422313790,
						'custom_fields': [
							{
								'id': 23,
								'name': 'pet',
								'value': 'Indiana',
								'type': 'text'
							},
							{
								'id': 24,
								'name': 'age',
								'value': '43',
								'type': 'number'
							}
						]
					}
				]
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync($"{ENDPOINT}/search", It.Is<JObject>(o => o.Properties().Count() == 2), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var contacts = new Contacts(mockClient.Object);

			// Act
			var result = contacts.SearchAsync(conditions, listId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("jones@example.com", result[0].Email);
			Assert.AreEqual(2, result[0].CustomFields.Length);
			Assert.AreEqual("pet", result[0].CustomFields[0].Name);
			Assert.AreEqual("Indiana", ((Field<string>)result[0].CustomFields[0]).Value);
			Assert.AreEqual("age", result[0].CustomFields[1].Name);
			Assert.AreEqual(43, ((Field<long?>)result[0].CustomFields[1]).Value);
		}

		[TestMethod]
		public void Search_without_conditions()
		{
			// Arrange
			var listId = (int?)null;
			var conditions = (SearchCondition[])null;
			var apiResponse = @"{
				'recipients': [
					{
						'created_at': 1422313607,
						'email': 'jones@example.com',
						'first_name': null,
						'id': 'YUBh',
						'last_clicked': 12345,
						'last_emailed': null,
						'last_name': 'Miller',
						'last_opened': null,
						'updated_at': 1422313790,
						'custom_fields': [
							{
								'id': 23,
								'name': 'pet',
								'value': 'Indiana',
								'type': 'text'
							},
							{
								'id': 24,
								'name': 'age',
								'value': '43',
								'type': 'number'
							}
						]
					}
				]
			}";

			var mockClient = new Mock<IClient>(MockBehavior.Strict);
			mockClient.Setup(c => c.PostAsync($"{ENDPOINT}/search", It.Is<JObject>(o => o.Properties().Count() == 0), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) });

			var contacts = new Contacts(mockClient.Object);

			// Act
			var result = contacts.SearchAsync(conditions, listId, CancellationToken.None).Result;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("jones@example.com", result[0].Email);
			Assert.AreEqual(2, result[0].CustomFields.Length);
			Assert.AreEqual("pet", result[0].CustomFields[0].Name);
			Assert.AreEqual("Indiana", ((Field<string>)result[0].CustomFields[0]).Value);
			Assert.AreEqual("age", result[0].CustomFields[1].Name);
			Assert.AreEqual(43, ((Field<long?>)result[0].CustomFields[1]).Value);
		}
	}
}
