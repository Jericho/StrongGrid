using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Model;
using StrongGrid.UnitTests;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class UnsubscribeGroupsTests
	{
		#region FIELDS

		private const string ENDPOINT = "asm/groups";

		private const string SINGLE_SUPPRESSION_GROUP_JSON = @"{
			'id': 1,
			'name': 'Product Suggestions',
			'description': 'Suggestions for products our users might like.',
			'is_default': true
		}";
		private const string MULTIPLE_SUPPRESSION_GROUPS_JSON = @"[
			{
				'id': 100,
				'name': 'Newsletters',
				'description': 'Our monthly newsletter.',
				'last_email_sent_at': null,
				'is_default': true,
				'unsubscribes': 400
			},
			{
				'id': 101,
				'name': 'Alerts',
				'description 2': 'Emails triggered by user-defined rules.',
				'last_email_sent_at': null,
				'is_default': false,
				'unsubscribes': 1
			}
		]";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<SuppressionGroup>(SINGLE_SUPPRESSION_GROUP_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.Description.ShouldBe("Suggestions for products our users might like.");
			result.Id.ShouldBe(1);
			result.IsDefault.ShouldBe(true);
			result.Name.ShouldBe("Product Suggestions");
		}

		[Fact]
		public void Create()
		{
			// Arrange
			var name = "Product Suggestions";
			var description = "Suggestions for products our users might like.";
			var isDefault = true;

			var apiResponse = @"{
				'name': 'Product Suggestions',
				'description': 'Suggestions for products our users might like.',
				'is_default': true
			}";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Post, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", apiResponse);

			var client = Utils.GetFluentClient(mockHttp);
			var goups = new UnsubscribeGroups(client);

			// Act
			var result = goups.CreateAsync(name, description, isDefault, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Name.ShouldBe(name);
			result.Description.ShouldBe(description);
			result.IsDefault.ShouldBe(isDefault);
		}

		[Fact]
		public void Get()
		{
			// Arrange
			var groupId = 100;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT, groupId)).Respond("application/json", SINGLE_SUPPRESSION_GROUP_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var groups = new UnsubscribeGroups(client);

			// Act
			var result = groups.GetAsync(groupId, CancellationToken.None).Result;

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
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", MULTIPLE_SUPPRESSION_GROUPS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var groups = new UnsubscribeGroups(client);

			// Act
			var result = groups.GetAllAsync(CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
			result[0].Id.ShouldBe(100);
			result[1].Id.ShouldBe(101);
		}

		[Fact]
		public void GetMultiple()
		{
			// Arrange
			var groupIds = new[] { 100, 101 };

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT)).Respond("application/json", MULTIPLE_SUPPRESSION_GROUPS_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var groups = new UnsubscribeGroups(client);

			// Act
			var result = groups.GetMultipleAsync(groupIds, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(2);
		}

		[Fact]
		public void Delete()
		{
			// Arrange
			var groupId = 100;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Delete, Utils.GetSendGridApiUri(ENDPOINT, groupId)).Respond(HttpStatusCode.OK);

			var client = Utils.GetFluentClient(mockHttp);
			var groups = new UnsubscribeGroups(client);

			// Act
			groups.DeleteAsync(groupId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
		}

		[Fact]
		public void Update()
		{
			// Arrange
			var groupId = 103;
			var name = "Item Suggestions";
			var description = "Suggestions for items our users might like.";

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(new HttpMethod("PATCH"), Utils.GetSendGridApiUri(ENDPOINT, groupId)).Respond("application/json", SINGLE_SUPPRESSION_GROUP_JSON);

			var client = Utils.GetFluentClient(mockHttp);
			var groups = new UnsubscribeGroups(client);

			// Act
			var result = groups.UpdateAsync(groupId, name, description, CancellationToken.None).Result;

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
		}
	}
}
