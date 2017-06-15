using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.UnitTests;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class ContactsTests
	{
		#region FIELDS

		private const string ENDPOINT = "contactdb/recipients";

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

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<Contact>(SINGLE_RECIPIENT_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.CreatedOn.ShouldBe(new DateTime(2015, 1, 26, 23, 6, 47, DateTimeKind.Utc));
			result.CustomFields.ShouldNotBeNull();
			result.CustomFields.Length.ShouldBe(1);
			result.CustomFields[0].GetType().ShouldBe(typeof(Field<string>));
			var stringField = result.CustomFields[0] as Field<string>;
			stringField.Id.ShouldBe(23);
			stringField.Name.ShouldBe("pet");
			stringField.Value.ShouldBe("Indiana");
			result.Email.ShouldBe("jones@example.com");
			result.FirstName.ShouldBeNull();
			result.Id.ShouldBe("YUBh");
			result.LastClickedOn.ShouldBeNull();
			result.LastEmailedOn.ShouldBeNull();
			result.LastName.ShouldBe("Jones");
			result.LastOpenedOn.ShouldBeNull();
			result.ModifiedOn.ShouldBe(new DateTime(2015, 1, 26, 23, 9, 50, DateTimeKind.Utc));
		}

		[Fact]
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var contacts = new Contacts(client);

			// Act
			var result = contacts.CreateAsync(email, firstName, lastName, null, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var contacts = new Contacts(client);

			// Act
			Should.ThrowAsync<Exception>(() => contacts.CreateAsync(email, firstName, lastName, null, CancellationToken.None))
				.Result.Message.ShouldBe("Invalid email.");

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var contacts = new Contacts(client);

			// Act
			var result = contacts.ImportAsync(records, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.ErrorCount.ShouldBe(1);
			result.ErrorIndices.ShouldBe(new[] { 2 });
			result.NewCount.ShouldBe(2);
			result.PersistedRecipients.Length.ShouldBe(2);
			result.UpdatedCount.ShouldBe(0);
			result.Errors.Length.ShouldBe(1);
			result.Errors[0].Message.ShouldBe("Invalid email.");
			result.Errors[0].ErrorIndices.ShouldBe(new[] { 2 });
		}

		[Fact]
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var contacts = new Contacts(client);

			// Act
			contacts.UpdateAsync(email, null, lastName, null, CancellationToken.None).Wait();

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var contacts = new Contacts(client);

			// Act
			Should.ThrowAsync<Exception>(() => contacts.UpdateAsync(email, null, lastName, null, CancellationToken.None))
				.Result.Message.ShouldBe("Invalid email.");

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void Get_single()
		{
			// Arrange
			var contactId = "YUBh";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, contactId)).Respond("application/json", SINGLE_RECIPIENT_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var contacts = new Contacts(client);

			// Act
			var result = contacts.GetAsync(contactId, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.LastName.ShouldBe("Jones");
			result.Email.ShouldBe("jones@example.com");
			result.CustomFields.Length.ShouldBe(1);
			result.CustomFields[0].Name.ShouldBe("pet");
			((Field<string>)result.CustomFields[0]).Value.ShouldBe("Indiana");
		}

		[Fact]
		public void Get_multiple()
		{
			// Arrange
			var recordsPerPage = 25;
			var page = 1;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT) + $"?page_size={recordsPerPage}&page={page}").Respond("application/json", MULTIPLE_RECIPIENTS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var contacts = new Contacts(client);

			// Act
			var result = contacts.GetAsync(recordsPerPage, page, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].Email.ShouldBe("jones@example.com");
			result[0].CustomFields.Length.ShouldBe(2);
			result[0].CustomFields[0].Name.ShouldBe("pet");
			((Field<string>)result[0].CustomFields[0]).Value.ShouldBe("Indiana");
			result[0].CustomFields[1].Name.ShouldBe("age");
			((Field<long>)result[0].CustomFields[1]).Value.ShouldBe(43);
		}

		[Fact]
		public void Delete_single()
		{
			// Arrange
			var contactId = "recipient_id1";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var contacts = new Contacts(client);

			// Act
			contacts.DeleteAsync(contactId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void Delete_multiple()
		{
			// Arrange
			var contactIds = new[] { "recipient_id1", "recipient_id2" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var contacts = new Contacts(client);

			// Act
			contacts.DeleteAsync(contactIds, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void GetBillableCount()
		{
			// Arrange
			var apiResponse = @"{
				'recipient_count': 2
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "billable_count")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var contacts = new Contacts(client);

			// Act
			var result = contacts.GetBillableCountAsync(CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBe(2);
		}

		[Fact]
		public void GetTotalCount()
		{
			// Arrange
			var apiResponse = @"{
				'recipient_count': 3
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "count")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var contacts = new Contacts(client);

			// Act
			var result = contacts.GetTotalCountAsync(CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBe(3);
		}

		[Fact]
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "search")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var contacts = new Contacts(client);

			// Act
			var result = contacts.SearchAsync(conditions, listId, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].Email.ShouldBe("jones@example.com");
			result[0].CustomFields.Length.ShouldBe(2);
			result[0].CustomFields[0].Name.ShouldBe("pet");
			((Field<string>)result[0].CustomFields[0]).Value.ShouldBe("Indiana");
			result[0].CustomFields[1].Name.ShouldBe("age");
			((Field<long>)result[0].CustomFields[1]).Value.ShouldBe(43);
		}

		[Fact]
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "search")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var contacts = new Contacts(client);

			// Act
			var result = contacts.SearchAsync(conditions, listId, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].Email.ShouldBe("jones@example.com");
			result[0].CustomFields.Length.ShouldBe(2);
			result[0].CustomFields[0].Name.ShouldBe("pet");
			((Field<string>)result[0].CustomFields[0]).Value.ShouldBe("Indiana");
			result[0].CustomFields[1].Name.ShouldBe("age");
			((Field<long>)result[0].CustomFields[1]).Value.ShouldBe(43);
		}

		[Fact]
		public void GetLists()
		{
			// Arrange
			var contactId = "YUBh";
			var listsJson = @"{
				'lists': [
					{
						'id': 1,
						'name': 'prospects',
						'recipient_count': 1
					},
					{
						'id': 2,
						'name': 'customers',
						'recipient_count': 1
					}
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, contactId, "lists")).Respond("application/json", listsJson);

			var client = Utils.GetFluentClient(mockHttp);
			var contacts = new Contacts(client);

			// Act
			var result = contacts.GetListsAsync(contactId, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}
	}
}
