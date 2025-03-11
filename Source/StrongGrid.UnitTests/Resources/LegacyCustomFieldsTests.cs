using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Resources.Legacy;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class LegacyCustomFieldsTests
	{
		#region FIELDS

		internal const string ENDPOINT = "contactdb/custom_fields";

		internal const string SINGLE_CUSTOM_FIELD_JSON = @"{
			""id"": 1,
			""name"": ""customfield1"",
			""type"": ""text""
		}";
		internal const string MULTIPLE_CUSTOM_FIELDS_JSON = @"{
			""custom_fields"": [
				{
					""id"": 1,
					""name"": ""birthday"",
					""type"": ""date""
				},
				{
					""id"": 2,
					""name"": ""middle_name"",
					""type"": ""text""
				},
				{
					""id"": 3,
					""name"": ""favorite_number"",
					""type"": ""number""
				}
			]
		}";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<StrongGrid.Models.Legacy.CustomFieldMetadata>(SINGLE_CUSTOM_FIELD_JSON, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.Id.ShouldBe(1);
			result.Name.ShouldBe("customfield1");
			result.Type.ShouldBe(FieldType.Text);
		}

		[Fact]
		public async Task CreateAsync()
		{
			// Arrange
			var name = "customfield1";
			var type = FieldType.Text;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_CUSTOM_FIELD_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var customFields = new CustomFields(client);

			// Act
			var result = await customFields.CreateAsync(name, type, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public async Task GetAsync()
		{
			// Arrange
			var fieldId = 1;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, fieldId)).Respond("application/json", SINGLE_CUSTOM_FIELD_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var customFields = new CustomFields(client);

			// Act
			var result = await customFields.GetAsync(fieldId, null, TestContext.Current.CancellationToken);

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
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", MULTIPLE_CUSTOM_FIELDS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var customFields = new CustomFields(client);

			// Act
			var result = await customFields.GetAllAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(3);
		}

		[Fact]
		public async Task DeleteAsync()
		{
			// Arrange
			var fieldId = 1;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, fieldId)).Respond(HttpStatusCode.Accepted);

			var client = Utils.GetFluentClient(mockHttp);
			var customFields = new CustomFields(client);

			// Act
			await customFields.DeleteAsync(fieldId, null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public async Task GetReservedFieldsAsync()
		{
			// Arrange
			var apiResponse = @"{
				""reserved_fields"": [
					{
						""name"": ""first_name"",
						""type"": ""text""
					},
					{
						""name"": ""last_name"",
						""type"": ""text""
					},
					{
						""name"": ""email"",
						""type"": ""text""
					},
					{
						""name"": ""created_at"",
						""type"": ""date""
					},
					{
						""name"": ""updated_at"",
						""type"": ""date""
					},
					{
						""name"": ""last_emailed"",
						""type"": ""date""
					},
					{
						""name"": ""last_clicked"",
						""type"": ""date""
					},
					{
						""name"": ""last_opened"",
						""type"": ""date""
					},
					{
						""name"": ""my_custom_field"",
						""type"": ""text""
					}
				]
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("contactdb/reserved_fields")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var customFields = new CustomFields(client);

			// Act
			var result = await customFields.GetReservedFieldsAsync(null, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(9);
			result[0].Name.ShouldBe("first_name");
			result[1].Name.ShouldBe("last_name");
			result[2].Name.ShouldBe("email");
		}
	}
}
