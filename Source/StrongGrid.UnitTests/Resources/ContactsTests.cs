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
		#region FIELDS

		private const string ENDPOINT = "/contactdb/recipients";
		private MockRepository _mockRepository;
		private Mock<IClient> _mockClient;

		private const string SINGLE_RECIPIENT_JSON = @"{
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
		private const string MULTIPLE_RECIPIENTS_JSON = @"{
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

		#endregion

		private Contacts CreateContacts()
		{
			return new Contacts(_mockClient.Object, ENDPOINT);

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
			var result = JsonConvert.DeserializeObject<Contact>(SINGLE_RECIPIENTJSON);

			// Assert
			Assert.IsNotNull(result);
			CollectionAssert.AreEqual(new[] { "spring line" }, result.Categories);
			Assert.AreEqual("", result.CustomUnsubscribeUrl);
			Assert.AreEqual("<html><head><title></title></head><body><p>Check out our spring line!</p></body></html>", result.HtmlContent);
			Assert.AreEqual(986724, result.Id);
			Assert.AreEqual("marketing", result.IpPool);
			CollectionAssert.AreEqual(new[] { 110L, 124L }, result.Lists);
			CollectionAssert.AreEqual(new[] { 110L }, result.Segments);
			Assert.AreEqual(124451, result.SenderId);
			Assert.AreEqual(CampaignStatus.Draft, result.Status);
			Assert.AreEqual("New Products for Spring!", result.Subject);
			Assert.AreEqual(42, result.SuppressionGroupId);
			Assert.AreEqual("Check out our spring line!", result.TextContent);
			Assert.AreEqual("March Newsletter", result.Title);
		}

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

			_mockClient
                .Setup(c => c.PostAsync(ENDPOINT, It.Is<JArray>(o => o.Count == 1), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(apiResponse) })
                .Verifiable();

			var contacts = CreateContacts();

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
