using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Models.Search;
using StrongGrid.Resources;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class EmailActivitiesTests
	{
		#region FIELDS

		internal const string ENDPOINT = "messages";

		internal const string SINGLE_MESSAGE = @"{
			""from_email"": ""test@example.com"",
			""msg_id"": ""thtIPCIcR_iFZDws2JCrwA.filter0004p3las1-2776-5ACA5525-31.1"",
			""subject"": ""Dear customer"",
			""to_email"": ""bob@example.com"",
			""status"": ""delivered"",
			""opens_count"": 2,
			""clicks_count"": 1,
			""last_event_time"": ""2018-04-08T17:47:18Z""
		}";

		internal const string NO_MESSAGES_FOUND = "{\"messages\":[]}";
		internal const string ONE_MESSAGE_FOUND = "{\"messages\":[" + SINGLE_MESSAGE + "]}";
		internal const string MULTIPLE_MESSAGES_FOUND = "{\"messages\":[" +
			SINGLE_MESSAGE + "," +
			SINGLE_MESSAGE + "," +
			SINGLE_MESSAGE +
		"]}";

		#endregion

		[Fact]
		public void Parse_json()
		{
			// Arrange

			// Act
			var result = JsonSerializer.Deserialize<EmailMessageActivity>(SINGLE_MESSAGE, JsonFormatter.DeserializerOptions);

			// Assert
			result.ShouldNotBeNull();
			result.From.ShouldBe("test@example.com");
			result.MessageId.ShouldBe("thtIPCIcR_iFZDws2JCrwA.filter0004p3las1-2776-5ACA5525-31.1");
			result.Subject.ShouldBe("Dear customer");
			result.To.ShouldBe("bob@example.com");
			result.Status.ShouldBe(EmailActivityStatus.Delivered);
			result.OpensCount.ShouldBe(2);
			result.ClicksCount.ShouldBe(1);
			result.LastEventOn.ShouldBe(new DateTime(2018, 04, 08, 17, 47, 18, DateTimeKind.Utc));
		}

		[Fact]
		public async Task SearchMessages_without_criteria()
		{
			// Arrange
			var limit = 25;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT) + $"?limit={limit}").Respond("application/json", NO_MESSAGES_FOUND);

			var client = Utils.GetFluentClient(mockHttp);
			var emailActivities = (IEmailActivities)new EmailActivities(client);

			// Act
			var result = await emailActivities.SearchAsync(null, limit, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(0);
		}

		[Fact]
		public async Task SearchMessages_single_criteria()
		{
			// Arrange
			var limit = 25;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT) + $"?limit={limit}&query=subject%3D%22thevalue%22").Respond("application/json", ONE_MESSAGE_FOUND);

			var client = Utils.GetFluentClient(mockHttp);
			var emailActivities = (IEmailActivities)new EmailActivities(client);

			var criteria = new SearchCriteriaEqual(EmailActivitiesFilterField.Subject, "thevalue");

			// Act
			var result = await emailActivities.SearchAsync(criteria, limit, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
		}

		[Fact]
		public async Task SearchMessages_multiple_filter_conditions()
		{
			// Arrange
			var limit = 25;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT) + $"?limit={limit}&query=(marketing_campaign_name%3D%22value1%22+AND+status%3D%22processed%22)").Respond("application/json", ONE_MESSAGE_FOUND);

			var client = Utils.GetFluentClient(mockHttp);
			var emailActivities = (IEmailActivities)new EmailActivities(client);

			var filterConditions = new[]
			{
				new SearchCriteriaEqual(EmailActivitiesFilterField.CampaignName, "value1"),
				new SearchCriteriaEqual(EmailActivitiesFilterField.ActivityType, EmailActivityStatus.Processed),
			};
			// Act
			var result = await emailActivities.SearchAsync(filterConditions, limit, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
		}

		[Fact]
		public async Task SearchMessages_complex_filter_conditions()
		{
			// Arrange
			var limit = 25;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT) + $"?limit={limit}&query=(marketing_campaign_name%3D%22value1%22+OR+msg_id%3D%22value2%22+AND+subject%3D%22value3%22+AND+teammate%3D%22value4%22)").Respond("application/json", ONE_MESSAGE_FOUND);

			var client = Utils.GetFluentClient(mockHttp);
			var emailActivities = new EmailActivities(client);

			var filterConditions = new[]
			{
				new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.Or, new[] { new SearchCriteriaEqual(EmailActivitiesFilterField.CampaignName, "value1"), new SearchCriteriaEqual(EmailActivitiesFilterField.MessageId, "value2") }),
				new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, new[] { new SearchCriteriaEqual(EmailActivitiesFilterField.Subject, "value3"), new SearchCriteriaEqual(EmailActivitiesFilterField.Teammate, "value4") }),
			};

			// Act
			var result = await emailActivities.SearchAsync(filterConditions, limit, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
		}

		[Fact]
		public async Task SearchMessages_single_custom_argument_criteria()
		{
			// Arrange
			var limit = 25;

			var mockHttp = new MockHttpMessageHandler();
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT) + $"?limit={limit}&query=(unique_args%5B%27name%27%5D%3D%22Joe%22)").Respond("application/json", ONE_MESSAGE_FOUND);

			var client = Utils.GetFluentClient(mockHttp);
			var emailActivities = (IEmailActivities)new EmailActivities(client);

			var criteria = new SearchCriteriaUniqueArgEqual("name", "Joe");

			// Act
			var result = await emailActivities.SearchAsync(criteria, limit, TestContext.Current.CancellationToken);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
		}
	}
}
