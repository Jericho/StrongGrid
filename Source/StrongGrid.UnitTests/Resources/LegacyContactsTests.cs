using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Models.Legacy;
using StrongGrid.Resources.Legacy;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class LegacyContactsTests
	{
		internal const string ENDPOINT = "contactdb/recipients";

		internal const string SINGLE_RECIPIENT_JSON = @"{
			""created_at"": 1422313607,
			""email"": ""jones@example.com"",
			""first_name"": null,
			""id"": ""YUBh"",
			""last_clicked"": null,
			""last_emailed"": null,
			""last_name"": ""Jones"",
			""last_opened"": null,
			""updated_at"": 1422313790,
			""custom_fields"": [
				{
					""id"": 23,
					""name"": ""pet"",
					""value"": ""Indiana"",
					""type"": ""text""
				}
			]
		}";
		internal const string MULTIPLE_RECIPIENTS_JSON = @"{
			""recipients"": [
				{
					""created_at"": 1422313607,
					""email"": ""jones@example.com"",
					""first_name"": null,
					""id"": ""YUBh"",
					""last_clicked"": null,
					""last_emailed"": null,
					""last_name"": ""Jones"",
					""last_opened"": null,
					""updated_at"": 1422313790,
					""custom_fields"": [
						{
							""id"": 23,
							""name"": ""pet"",
							""value"": ""Indiana"",
							""type"": ""text""
						},
						{
							""id"": 24,
							""name"": ""age"",
							""value"": ""43"",
							""type"": ""number""
						}
					]
				}
			]
		}";

		private readonly ITestOutputHelper _outputHelper;

		public LegacyContactsTests(ITestOutputHelper outputHelper)
		{
			_outputHelper = outputHelper;
		}

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<StrongGrid.Models.Legacy.Contact>(SINGLE_RECIPIENT_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.CreatedOn.ShouldBe(new DateTime(2015, 1, 26, 23, 6, 47, DateTimeKind.Utc));
			result.CustomFields.ShouldNotBeNull();
			result.CustomFields.Length.ShouldBe(1);
			result.CustomFields[0].GetType().ShouldBe(typeof(StrongGrid.Models.Legacy.Field<string>));
			var stringField = result.CustomFields[0] as StrongGrid.Models.Legacy.Field<string>;
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
		public async Task CreateAsync_success()
		{
			// Arrange
			var email = "Jane@example.com";
			var firstName = "Jane";
			var lastName = "Doe";

			var apiResponse = @"{
				""error_count"": 0,
				""error_indices"": [
				],
				""unmodified_indices"": [
				],
				""new_count"": 1,
				""persisted_recipients"": [
					""am9uZXNAZXhhbXBsZS5jb20=""
				],
				""updated_count"": 0
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var contacts = new Contacts(client);

			// Act
			var result = await contacts.CreateAsync(email, firstName, lastName, null, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task CreateAsync_failure()
		{
			// Arrange
			var email = "invalid_email";
			var firstName = "Jane";
			var lastName = "Doe";

			var apiResponse = @"{
				""error_count"": 1,
				""error_indices"": [0],
				""unmodified_indices"": [],
				""new_count"": 0,
				""persisted_recipients"": [],
				""updated_count"": 0,
				""errors"": [
					{
						""message"": ""Invalid email."",
						""error_indices"": [0]
					}
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var contacts = new Contacts(client);

			// Act
			var result = await Should.ThrowAsync<Exception>(contacts.CreateAsync(email, firstName, lastName, null, null, TestContext.Current.CancellationToken));

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();

			result.Message.ShouldBe("Invalid email.");
		}

		[Fact]
		public async Task ImportAsync()
		{
			// Arrange
			var records = new[]
			{
				new StrongGrid.Models.Legacy.Contact("jones@example.com", null, "Jones", new StrongGrid.Models.Legacy.Field[] { new StrongGrid.Models.Legacy.Field<string>("pet", "Fluffy"), new StrongGrid.Models.Legacy.Field<long>("age", 25) }),
				new StrongGrid.Models.Legacy.Contact("miller@example.com", null, "Miller", new StrongGrid.Models.Legacy.Field[] { new StrongGrid.Models.Legacy.Field<string>("pet", "FrouFrou"), new StrongGrid.Models.Legacy.Field<long>("age", 32) }),
				new StrongGrid.Models.Legacy.Contact("invalid email", null, "Smith", new StrongGrid.Models.Legacy.Field[] { new StrongGrid.Models.Legacy.Field<string>("pet", "Spot"), new StrongGrid.Models.Legacy.Field<long>("age", 17) })
			};

			var apiResponse = @"{
				""error_count"": 1,
				""error_indices"": [
					2
				],
				""unmodified_indices"": [
					3
				],
				""new_count"": 2,
				""persisted_recipients"": [
					""YUBh"",
					""bWlsbGVyQG1pbGxlci50ZXN0""
				],
				""updated_count"": 0,
				""errors"": [
					{
						""message"": ""Invalid email."",
						""error_indices"": [
							2
						]
					}
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var contacts = new Contacts(client);

			// Act
			var result = await contacts.ImportAsync(records, null, TestContext.Current.CancellationToken);

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
		public async Task UpdateAsync_success()
		{
			// Arrange
			var email = "jones@example.com";
			var lastName = "Jones";

			var apiResponse = @"{
				""error_count"": 0,
				""error_indices"": [
				],
				""unmodified_indices"": [
					1
				],
				""new_count"": 0,
				""persisted_recipients"": [
					""am9uZXNAZXhhbXBsZS5jb20=""
				],
				""updated_count"": 1
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var contacts = new Contacts(client);

			var customFields = new StrongGrid.Models.Legacy.Field[]
			{
				new StrongGrid.Models.Legacy.Field<string>("nickname", "Bob"),
				new StrongGrid.Models.Legacy.Field<long>("age", 55),
				new StrongGrid.Models.Legacy.Field<long?>("net_worth", 1000000),
				new StrongGrid.Models.Legacy.Field<DateTime>("anniversary", new DateTime(2000, 1, 1)),
				new StrongGrid.Models.Legacy.Field<DateTime?>("customer_since", new DateTime(2000, 1, 1))
			};

			// Act
			await contacts.UpdateAsync(email, null, lastName, customFields, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task UpdateAsync_failure()
		{
			// Arrange
			var email = "invalid_email";
			var lastName = "Jones";

			var apiResponse = @"{
				""error_count"": 1,
				""error_indices"": [0],
				""unmodified_indices"": [],
				""new_count"": 0,
				""persisted_recipients"": [],
				""updated_count"": 0,
				""errors"": [
					{
						""message"": ""Invalid email."",
						""error_indices"": [0]
					}
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var contacts = new Contacts(client);

			// Act
			var result = await Should.ThrowAsync<Exception>(contacts.UpdateAsync(email, null, lastName, null, null, TestContext.Current.CancellationToken));

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();

			result.Message.ShouldBe("Invalid email.");
		}

		[Fact]
		public async Task GetAsync_single()
		{
			// Arrange
			var contactId = "YUBh";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, contactId)).Respond("application/json", SINGLE_RECIPIENT_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var contacts = new Contacts(client);

			// Act
			var result = await contacts.GetAsync(contactId, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.LastName.ShouldBe("Jones");
			result.Email.ShouldBe("jones@example.com");
			result.CustomFields.Length.ShouldBe(1);
			result.CustomFields[0].Name.ShouldBe("pet");
			((StrongGrid.Models.Legacy.Field<string>)result.CustomFields[0]).Value.ShouldBe("Indiana");
		}

		[Fact]
		public async Task GetAsync_multiple()
		{
			// Arrange
			var recordsPerPage = 25;
			var page = 1;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT) + $"?page_size={recordsPerPage}&page={page}").Respond("application/json", MULTIPLE_RECIPIENTS_JSON);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var contacts = new Contacts(client);

			// Act
			var result = await contacts.GetAsync(recordsPerPage, page, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].Email.ShouldBe("jones@example.com");
			result[0].CustomFields.Length.ShouldBe(2);
			result[0].CustomFields[0].Name.ShouldBe("pet");
			((StrongGrid.Models.Legacy.Field<string>)result[0].CustomFields[0]).Value.ShouldBe("Indiana");
			result[0].CustomFields[1].Name.ShouldBe("age");
			((StrongGrid.Models.Legacy.Field<long>)result[0].CustomFields[1]).Value.ShouldBe(43);
		}

		[Fact]
		public async Task DeleteAsync_single()
		{
			// Arrange
			var contactId = "recipient_id1";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT)).Respond(HttpStatusCode.OK);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var contacts = new Contacts(client);

			// Act
			await contacts.DeleteAsync(contactId, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task DeleteAsync_multiple()
		{
			// Arrange
			var contactIds = new[] { "recipient_id1", "recipient_id2" };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT)).Respond(HttpStatusCode.OK);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var contacts = new Contacts(client);

			// Act
			await contacts.DeleteAsync(contactIds, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task GetBillableCountAsync()
		{
			// Arrange
			var apiResponse = @"{
				""recipient_count"": 2
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "billable_count")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var contacts = new Contacts(client);

			// Act
			var result = await contacts.GetBillableCountAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBe(2);
		}

		[Fact]
		public async Task GetTotalCountAsync()
		{
			// Arrange
			var apiResponse = @"{
				""recipient_count"": 3
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, "count")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var contacts = new Contacts(client);

			// Act
			var result = await contacts.GetTotalCountAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldBe(3);
		}

		[Fact]
		public async Task SearchAsync()
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
				""recipients"": [
					{
						""created_at"": 1422313607,
						""email"": ""jones@example.com"",
						""first_name"": null,
						""id"": ""YUBh"",
						""last_clicked"": 12345,
						""last_emailed"": null,
						""last_name"": ""Miller"",
						""last_opened"": null,
						""updated_at"": 1422313790,
						""custom_fields"": [
							{
								""id"": 23,
								""name"": ""pet"",
								""value"": ""Indiana"",
								""type"": ""text""
							},
							{
								""id"": 24,
								""name"": ""age"",
								""value"": ""43"",
								""type"": ""number""
							}
						]
					}
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "search")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var contacts = new Contacts(client);

			// Act
			var result = await contacts.SearchAsync(conditions, listId, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].Email.ShouldBe("jones@example.com");
			result[0].CustomFields.Length.ShouldBe(2);
			result[0].CustomFields[0].Name.ShouldBe("pet");
			((StrongGrid.Models.Legacy.Field<string>)result[0].CustomFields[0]).Value.ShouldBe("Indiana");
			result[0].CustomFields[1].Name.ShouldBe("age");
			((StrongGrid.Models.Legacy.Field<long>)result[0].CustomFields[1]).Value.ShouldBe(43);
		}

		[Fact]
		public async Task SearchAsync_without_conditions()
		{
			// Arrange
			var listId = (int?)null;
			var conditions = (SearchCondition[])null;
			var apiResponse = @"{
				""recipients"": [
					{
						""created_at"": 1422313607,
						""email"": ""jones@example.com"",
						""first_name"": null,
						""id"": ""YUBh"",
						""last_clicked"": 12345,
						""last_emailed"": null,
						""last_name"": ""Miller"",
						""last_opened"": null,
						""updated_at"": 1422313790,
						""custom_fields"": [
							{
								""id"": 23,
								""name"": ""pet"",
								""value"": ""Indiana"",
								""type"": ""text""
							},
							{
								""id"": 24,
								""name"": ""age"",
								""value"": ""43"",
								""type"": ""number""
							}
						]
					}
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT, "search")).Respond("application/json", apiResponse);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var contacts = new Contacts(client);

			// Act
			var result = await contacts.SearchAsync(conditions, listId, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].Email.ShouldBe("jones@example.com");
			result[0].CustomFields.Length.ShouldBe(2);
			result[0].CustomFields[0].Name.ShouldBe("pet");
			((StrongGrid.Models.Legacy.Field<string>)result[0].CustomFields[0]).Value.ShouldBe("Indiana");
			result[0].CustomFields[1].Name.ShouldBe("age");
			((StrongGrid.Models.Legacy.Field<long>)result[0].CustomFields[1]).Value.ShouldBe(43);
		}

		[Fact]
		public async Task GetListsAsync()
		{
			// Arrange
			var contactId = "YUBh";
			var listsJson = @"{
				""lists"": [
					{
						""id"": 1,
						""name"": ""prospects"",
						""recipient_count"": 1
					},
					{
						""id"": 2,
						""name"": ""customers"",
						""recipient_count"": 1
					}
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, contactId, "lists")).Respond("application/json", listsJson);

			var logger = _outputHelper.ToLogger<IClient>();
			var client = Utils.GetFluentClient(mockHttp, logger);
			var contacts = new Contacts(client);

			// Act
			var result = await contacts.GetListsAsync(contactId, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}
	}
}
