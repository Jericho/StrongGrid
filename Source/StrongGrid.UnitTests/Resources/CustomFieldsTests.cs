using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Model;
using StrongGrid.UnitTests;
using StrongGrid.Utilities;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class CustomFieldsTests
	{
		#region FIELDS

		private const string ENDPOINT = "contactdb/custom_fields";

		private const string SINGLE_CUSTOM_FIELD_JSON = @"{
			'id': 1,
			'name': 'customfield1',
			'type': 'text'
		}";
		private const string MULTIPLE_CUSTOM_FIELDS_JSON = @"{
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

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<CustomFieldMetadata>(SINGLE_CUSTOM_FIELD_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.Id.ShouldBe(1);
			result.Name.ShouldBe("customfield1");
			result.Type.ShouldBe(FieldType.Text);
		}

		[Fact]
		public void Create()
		{
			// Arrange
			var name = "customfield1";
			var type = FieldType.Text;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", SINGLE_CUSTOM_FIELD_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var customFields = new CustomFields(client);

			// Act
			var result = customFields.CreateAsync(name, type, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void Get()
		{
			// Arrange
			var fieldId = 1;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, fieldId)).Respond("application/json", SINGLE_CUSTOM_FIELD_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var customFields = new CustomFields(client);

			// Act
			var result = customFields.GetAsync(fieldId, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetAll()
		{
			// Arrange
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", MULTIPLE_CUSTOM_FIELDS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var customFields = new CustomFields(client);

			// Act
			var result = customFields.GetAllAsync(CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(3);
		}

		[Fact]
		public void Delete()
		{
			// Arrange
			var fieldId = 1;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, fieldId)).Respond(HttpStatusCode.Accepted);

			var client = Utils.GetFluentClient(mockHttp);
			var customFields = new CustomFields(client);

			// Act
			customFields.DeleteAsync(fieldId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
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

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri("contactdb/reserved_fields")).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var customFields = new CustomFields(client);

			// Act
			var result = customFields.GetReservedFieldsAsync(CancellationToken.None).Result;

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
