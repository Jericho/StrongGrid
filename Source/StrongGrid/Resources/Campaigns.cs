using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage <see cref="Campaign">campaigns</see>.
	/// </summary>
	/// <remarks>
	/// See also: https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/campaigns.html
	/// </remarks>
	public class Campaigns
	{
		private const string _endpoint = "campaigns";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Campaigns" /> class.
		/// </summary>
		/// <param name="client">The HTTP client</param>
		public Campaigns(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Create a campaign.
		/// </summary>
		/// <param name="title">The title.</param>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="listIds">The list ids.</param>
		/// <param name="segmentIds">The segment ids.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="suppressionGroupId">The suppression group identifier.</param>
		/// <param name="customUnsubscribeUrl">The custom unsubscribe URL.</param>
		/// <param name="ipPool">The ip pool.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Campaign" />.
		/// </returns>
		public Task<Campaign> CreateAsync(
			string title,
			long senderId,
			Parameter<string> subject = default(Parameter<string>),
			Parameter<string> htmlContent = default(Parameter<string>),
			Parameter<string> textContent = default(Parameter<string>),
			Parameter<IEnumerable<long>> listIds = default(Parameter<IEnumerable<long>>),
			Parameter<IEnumerable<long>> segmentIds = default(Parameter<IEnumerable<long>>),
			Parameter<IEnumerable<string>> categories = default(Parameter<IEnumerable<string>>),
			Parameter<long?> suppressionGroupId = default(Parameter<long?>),
			Parameter<string> customUnsubscribeUrl = default(Parameter<string>),
			Parameter<string> ipPool = default(Parameter<string>),
			CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = CreateJObject(title, senderId, subject, htmlContent, textContent, listIds, segmentIds, categories, suppressionGroupId, customUnsubscribeUrl, ipPool);
			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Campaign>();
		}

		/// <summary>
		/// Retrieve all campaigns.
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="Campaign" />.
		/// </returns>
		public Task<Campaign[]> GetAllAsync(int limit = 10, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync(_endpoint)
				.WithArgument("limit", limit)
				.WithArgument("offset", offset)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Campaign[]>("result");
		}

		/// <summary>
		/// Retrieve a campaign.
		/// </summary>
		/// <param name="campaignId">The id of the campaign</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Campaign" />.
		/// </returns>
		public Task<Campaign> GetAsync(long campaignId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/{campaignId}")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Campaign>();
		}

		/// <summary>
		/// Delete a campaign.
		/// </summary>
		/// <param name="campaignId">The id of the campaign</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(long campaignId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync($"{_endpoint}/{campaignId}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Update a campaign
		/// </summary>
		/// <param name="campaignId">The id of the campaign</param>
		/// <param name="title">The title.</param>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="listIds">The list ids.</param>
		/// <param name="segmentIds">The segment ids.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="suppressionGroupId">The suppression group identifier.</param>
		/// <param name="customUnsubscribeUrl">The custom unsubscribe URL.</param>
		/// <param name="ipPool">The ip pool.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Campaign" />.
		/// </returns>
		public Task<Campaign> UpdateAsync(
			long campaignId,
			Parameter<string> title = default(Parameter<string>),
			Parameter<long?> senderId = default(Parameter<long?>),
			Parameter<string> subject = default(Parameter<string>),
			Parameter<string> htmlContent = default(Parameter<string>),
			Parameter<string> textContent = default(Parameter<string>),
			Parameter<IEnumerable<long>> listIds = default(Parameter<IEnumerable<long>>),
			Parameter<IEnumerable<long>> segmentIds = default(Parameter<IEnumerable<long>>),
			Parameter<IEnumerable<string>> categories = default(Parameter<IEnumerable<string>>),
			Parameter<long?> suppressionGroupId = default(Parameter<long?>),
			Parameter<string> customUnsubscribeUrl = default(Parameter<string>),
			Parameter<string> ipPool = default(Parameter<string>),
			CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = CreateJObject(title, senderId, subject, htmlContent, textContent, listIds, segmentIds, categories, suppressionGroupId, customUnsubscribeUrl, ipPool);
			return _client
				.PatchAsync($"{_endpoint}/{campaignId}")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Campaign>();
		}

		/// <summary>
		/// Send a campaign immediately.
		/// </summary>
		/// <param name="campaignId">The id of the campaign</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task SendNowAsync(long campaignId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = (JObject)null;
			return _client
				.PostAsync($"{_endpoint}/{campaignId}/schedules/now")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Schedule a campaign to be sewnt at a later time.
		/// </summary>
		/// <param name="campaignId">The id of the campaign</param>
		/// <param name="sendOn">The send on.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task ScheduleAsync(long campaignId, DateTime sendOn, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "send_at", sendOn.ToUnixTime() }
			};
			return _client
				.PostAsync($"{_endpoint}/{campaignId}/schedules")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Change the date a campaign is scheduled to be sent
		/// </summary>
		/// <param name="campaignId">The id of the campaign</param>
		/// <param name="sendOn">The send on.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task RescheduleAsync(long campaignId, DateTime sendOn, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "send_at", sendOn.ToUnixTime() }
			};
			return _client
				.PatchAsync($"{_endpoint}/{campaignId}/schedules")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Retrieve the date a campaign is scheduled to be sent
		/// </summary>
		/// <param name="campaignId">The id of the campaign</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="DateTime" /> the campaign is cheduled to be sent or <c>null</c> if the campaign is
		/// not scheduled to be sent.
		/// </returns>
		public async Task<DateTime?> GetScheduledDateAsync(long campaignId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var unixTime = await _client
				.GetAsync($"{_endpoint}/{campaignId}/schedules")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<long>("send_at")
				.ConfigureAwait(false);

			if (unixTime == 0) return null;
			else return unixTime.FromUnixTime();
		}

		/// <summary>
		/// Unschedule a scheduled campaign.
		/// </summary>
		/// <param name="campaignId">The id of the campaign</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task UnscheduleAsync(long campaignId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync($"{_endpoint}/{campaignId}/schedules")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Send a test campaign
		/// </summary>
		/// <param name="campaignId">The id of the campaign</param>
		/// <param name="emailAddresses">The email addresses.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		/// <exception cref="System.ArgumentException">You must specify at least one email address</exception>
		public Task SendTestAsync(long campaignId, IEnumerable<string> emailAddresses, CancellationToken cancellationToken = default(CancellationToken))
		{
			emailAddresses = emailAddresses ?? Enumerable.Empty<string>();
			if (!emailAddresses.Any()) throw new ArgumentException("You must specify at least one email address");

			var data = new JObject();
			if (emailAddresses.Count() == 1) data.Add("to", emailAddresses.First());
			else data.Add("to", JArray.FromObject(emailAddresses.ToArray()));

			return _client
				.PostAsync($"{_endpoint}/{campaignId}/schedules/test")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		private static JObject CreateJObject(Parameter<string> title, Parameter<long?> senderId, Parameter<string> subject, Parameter<string> htmlContent, Parameter<string> textContent, Parameter<IEnumerable<long>> listIds, Parameter<IEnumerable<long>> segmentIds, Parameter<IEnumerable<string>> categories, Parameter<long?> suppressionGroupId, Parameter<string> customUnsubscribeUrl, Parameter<string> ipPool)
		{
			var result = new JObject();
			if (title.HasValue) result.Add("title", title.Value);
			if (subject.HasValue) result.Add("subject", subject.Value);
			if (senderId.HasValue) result.Add("sender_id", senderId.Value);
			if (htmlContent.HasValue) result.Add("html_content", htmlContent.Value);
			if (textContent.HasValue) result.Add("plain_content", textContent.Value);
			if (listIds.HasValue) result.Add("list_ids", listIds.Value == null ? null : JArray.FromObject(listIds.Value.ToArray()));
			if (segmentIds.HasValue) result.Add("segment_ids", segmentIds.Value == null ? null : JArray.FromObject(segmentIds.Value.ToArray()));
			if (categories.HasValue) result.Add("categories", categories.Value == null ? null : JArray.FromObject(categories.Value.ToArray()));
			if (suppressionGroupId.HasValue) result.Add("suppression_group_id", suppressionGroupId.Value);
			if (customUnsubscribeUrl.HasValue) result.Add("custom_unsubscribe_url", customUnsubscribeUrl.Value);
			if (ipPool.HasValue) result.Add("ip_pool", ipPool.Value);
			return result;
		}
	}
}
