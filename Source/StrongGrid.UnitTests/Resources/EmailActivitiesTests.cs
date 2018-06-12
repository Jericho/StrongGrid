using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Shouldly;
using StrongGrid.Models;
using StrongGrid.Models.Search;
using StrongGrid.Resources;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StrongGrid.UnitTests.Resources
{
	public class EmailActivitiesTests
	{
		#region FIELDS

		private const string ENDPOINT = "messages";

		private const string SINGLE_MESSAGE = @"{
			'from_email': 'test@example.com',
			'msg_id': 'thtIPCIcR_iFZDws2JCrwA.filter0004p3las1-2776-5ACA5525-31.1',
			'subject': 'Dear customer',
			'to_email': 'bob@example.com',
			'status': 'delivered',
			'opens_count': 2,
			'clicks_count': 1,
			'last_event_time': '2018-04-08T17:47:18Z'
		}";

		private const string NO_MESSAGES_FOUND = "{'messages':[]}";
		private const string ONE_MESSAGE_FOUND = "{'messages':[" + SINGLE_MESSAGE + "]}";
		private const string MULTIPLE_MESSAGES_FOUND = "{'messages':[" +
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
			var result = JsonConvert.DeserializeObject<EmailMessageActivity>(SINGLE_MESSAGE);

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
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT) + $"?limit={limit}&query=").Respond("application/json", NO_MESSAGES_FOUND);

			var client = Utils.GetFluentClient(mockHttp);
			var emailActivities = (IEmailActivities)new EmailActivities(client);

			// Act
			var result = await emailActivities.SearchAsync(null, limit, CancellationToken.None).ConfigureAwait(false);

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

			var criteria = new SearchCriteriaEqual(FilterField.Subject, "thevalue");

			// Act
			var result = await emailActivities.SearchAsync(criteria, limit, CancellationToken.None).ConfigureAwait(false);

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
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT) + $"?limit={limit}&query=campaign_name%3D%22value1%22+AND+status%3D%processed%22").Respond("application/json", ONE_MESSAGE_FOUND);

			var client = Utils.GetFluentClient(mockHttp);
			var emailActivities = (IEmailActivities)new EmailActivities(client);

			var filterConditions = new[]
			{
				new SearchCriteriaEqual(FilterField.CampaignName, "value1"),
				new SearchCriteriaEqual(FilterField.ActivityType, EmailActivityStatus.Processed),
			};
			// Act
			var result = await emailActivities.SearchAsync(filterConditions, limit, CancellationToken.None).ConfigureAwait(false);

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
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT) + $"?limit={limit}&query=campaign_name%3D%22value1%22+OR+msg_id%3D%22value2%22+AND+subject%3D%22value3%22+AND+teammate%3D%22value4%22").Respond("application/json", ONE_MESSAGE_FOUND);

			var client = Utils.GetFluentClient(mockHttp);
			var emailActivities = new EmailActivities(client);

			var filterConditions = new[]
			{
				new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.Or, new[] { new SearchCriteriaEqual(FilterField.CampaignName, "value1"), new SearchCriteriaEqual(FilterField.MessageId, "value2") }),
				new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, new[] { new SearchCriteriaEqual(FilterField.Subject, "value3"), new SearchCriteriaEqual(FilterField.Teammate, "value4") }),
			};

			// Act
			var result = await emailActivities.SearchAsync(filterConditions, limit, CancellationToken.None).ConfigureAwait(false);

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
			mockHttp.Expect(HttpMethod.Get, Utils.GetSendGridApiUri(ENDPOINT) + $"?limit={limit}&query=(unique_args['name']%3D%22Joe%22)").Respond("application/json", ONE_MESSAGE_FOUND);

			var client = Utils.GetFluentClient(mockHttp);
			var emailActivities = (IEmailActivities)new EmailActivities(client);

			var criteria = new SearchCriteriaUniqueArgEqual("name", "Joe");

			// Act
			var result = await emailActivities.SearchAsync(criteria, limit, CancellationToken.None).ConfigureAwait(false);

			// Assert
			mockHttp.VerifyNoOutstandingExpectation();
			mockHttp.VerifyNoOutstandingRequest();
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
		}
	}
}
