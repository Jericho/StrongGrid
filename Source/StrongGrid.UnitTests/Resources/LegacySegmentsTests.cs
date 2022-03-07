using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.Models.Legacy;
using StrongGrid.Resources.Legacy;
using StrongGrid.Utilities;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class LegacySegmentsTests
	{
		#region FIELDS

		internal const string ENDPOINT = "contactdb/segments";

		internal const string SINGLE_SEGMENT_JSON = @"{
			""id"": 1,
			""name"": ""Last Name Miller"",
			""list_id"": 4,
			""conditions"": [
				{
					""field"": ""last_name"",
					""value"": ""Miller"",
					""operator"": ""eq"",
					""and_or"": """"
				},
				{
					""field"": ""last_clicked"",
					""value"": ""01/02/2015"",
					""operator"": ""gt"",
					""and_or"": ""and""
				},
				{
					""field"": ""clicks.campaign_identifier"",
					""value"": ""513"",
					""operator"": ""eq"",
					""and_or"": ""or""
				}
			]
		}";
		internal const string MULTIPLE_SEGMENTS_JSON = @"{
			""segments"": [
				{
					""id"": 1,
					""name"": ""Last Name Miller"",
					""list_id"": 4,
					""conditions"": [
						{
							""field"": ""last_name"",
							""value"": ""Miller"",
							""operator"": ""eq"",
							""and_or"": """"
						}
					],
					""recipient_count"": 1
				}
			]
		}";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<StrongGrid.Models.Legacy.Segment>(SINGLE_SEGMENT_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.Conditions.ShouldNotBeNull();
			result.Conditions.Length.ShouldBe(3);

			result.Conditions[0].Field.ShouldBe("last_name");
			result.Conditions[0].LogicalOperator.ShouldBe(LogicalOperator.None);
			result.Conditions[0].Operator.ShouldBe(ConditionOperator.Equal);
			result.Conditions[0].Value.ShouldBe("Miller");

			result.Conditions[1].Field.ShouldBe("last_clicked");
			result.Conditions[1].LogicalOperator.ShouldBe(LogicalOperator.And);
			result.Conditions[1].Operator.ShouldBe(ConditionOperator.GreatherThan);
			result.Conditions[1].Value.ShouldBe("01/02/2015");

			result.Conditions[2].Field.ShouldBe("clicks.campaign_identifier");
			result.Conditions[2].LogicalOperator.ShouldBe(LogicalOperator.Or);
			result.Conditions[2].Operator.ShouldBe(ConditionOperator.Equal);
			result.Conditions[2].Value.ShouldBe("513");

			result.Id.ShouldBe(1);
			result.ListId.ShouldBe(4);
			result.Name.ShouldBe("Last Name Miller");
		}

		[Fact]
		public async Task CreateAsync()
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_SEGMENT_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var segments = new Segments(client);

			// Act
			var result = await segments.CreateAsync(name, conditions, listId, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetAllAsync()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", MULTIPLE_SEGMENTS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var segments = new Segments(client);

			// Act
			var result = await segments.GetAllAsync(null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
		}

		[Fact]
		public async Task GetAsync()
		{
			// Arrange
			var segmentId = 1;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, segmentId)).Respond("application/json", SINGLE_SEGMENT_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var segments = new Segments(client);

			// Act
			var result = await segments.GetAsync(segmentId, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task UpdateAsync()
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT, segmentId)).Respond("application/json", SINGLE_SEGMENT_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var segments = new Segments(client);

			// Act
			var result = await segments.UpdateAsync(segmentId, name, listId, conditions, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task DeleteAsync_and_preserve_contacts()
		{
			// Arrange
			var segmentId = 1;
			var deleteContacts = false;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, segmentId) + "?delete_contacts=false").Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var segments = new Segments(client);

			// Act
			await segments.DeleteAsync(segmentId, deleteContacts, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task DeleteAsync_and_delete_contacts()
		{
			// Arrange
			var segmentId = 1;
			var deleteContacts = true;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, segmentId) + "?delete_contacts=true").Respond(HttpStatusCode.NoContent);

			var client = Utils.GetFluentClient(mockHttp);
			var segments = new Segments(client);

			// Act
			await segments.DeleteAsync(segmentId, deleteContacts, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task GetRecipientsAsync()
		{
			// Arrange
			var segmentId = 1;
			var recordsPerPage = 25;
			var page = 1;

			var apiResponse = @"{
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
							}
						]
					}
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, $"{segmentId}/recipients?page_size={recordsPerPage}&page={page}")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var segments = new Segments(client);

			// Act
			var result = await segments.GetRecipientsAsync(segmentId, recordsPerPage, page, null, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
			result[0].Email.ShouldBe("jones@example.com");
		}
	}
}
