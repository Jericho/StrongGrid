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
		private string _endpoint;
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Campaigns" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public Campaigns(Pathoschild.Http.Client.IClient client, string endpoint = "/campaigns")
		{
			_endpoint = endpoint;
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
		public Task<Campaign> CreateAsync(string title, long senderId, string subject = null, string htmlContent = null, string textContent = null, IEnumerable<long> listIds = null, IEnumerable<long> segmentIds = null, IEnumerable<string> categories = null, long? suppressionGroupId = null, string customUnsubscribeUrl = null, string ipPool = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			listIds = listIds ?? Enumerable.Empty<long>();
			segmentIds = segmentIds ?? Enumerable.Empty<long>();
			categories = categories ?? Enumerable.Empty<string>();

			var data = CreateJObjectForCampaign(title, senderId, subject, htmlContent, textContent, listIds, segmentIds, categories, suppressionGroupId, customUnsubscribeUrl, ipPool);
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
			var endpoint = $"{_endpoint}?limit={limit}&offset={offset}";
			return _client
				.GetAsync(endpoint)
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
			var endpoint = $"{_endpoint}/{campaignId}";
			return _client
				.GetAsync(endpoint)
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
			var endpoint = $"{_endpoint}/{campaignId}";
			return _client
				.DeleteAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
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
		public Task<Campaign> UpdateAsync(long campaignId, string title = null, long? senderId = null, string subject = null, string htmlContent = null, string textContent = null, IEnumerable<long> listIds = null, IEnumerable<long> segmentIds = null, IEnumerable<string> categories = null, long? suppressionGroupId = null, string customUnsubscribeUrl = null, string ipPool = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			listIds = listIds ?? Enumerable.Empty<long>();
			segmentIds = segmentIds ?? Enumerable.Empty<long>();
			categories = categories ?? Enumerable.Empty<string>();

			var endpoint = $"{_endpoint}/{campaignId}";
			var data = CreateJObjectForCampaign(title, senderId, subject, htmlContent, textContent, listIds, segmentIds, categories, suppressionGroupId, customUnsubscribeUrl, ipPool);
			return _client
				.PatchAsync(endpoint)
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
			var endpoint = $"{_endpoint}/{campaignId}/schedules/now";
			var data = (JObject)null;
			return _client
				.PostAsync(endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
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
			var endpoint = $"{_endpoint}/{campaignId}/schedules";
			var data = new JObject
			{
				{ "send_at", sendOn.ToUnixTime() }
			};
			return _client
				.PostAsync(endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
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
			var endpoint = $"{_endpoint}/{campaignId}/schedules";
			var data = new JObject
			{
				{ "send_at", sendOn.ToUnixTime() }
			};
			return _client
				.PatchAsync(endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
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
			var endpoint = $"{_endpoint}/{campaignId}/schedules";
			var unixTime = await _client
				.GetAsync(endpoint)
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
			var endpoint = $"{_endpoint}/{campaignId}/schedules";
			return _client
				.DeleteAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
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

			var endpoint = $"{_endpoint}/{campaignId}/schedules/test";
			return _client
				.PostAsync(endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		private static JObject CreateJObjectForCampaign(string title = null, long? senderId = null, string subject = null, string htmlContent = null, string textContent = null, IEnumerable<long> listIds = null, IEnumerable<long> segmentIds = null, IEnumerable<string> categories = null, long? suppressionGroupId = null, string customUnsubscribeUrl = null, string ipPool = null)
		{
			var result = new JObject();
			if (!string.IsNullOrEmpty(title)) result.Add("title", title);
			if (!string.IsNullOrEmpty(subject)) result.Add("subject", subject);
			if (senderId.HasValue) result.Add("sender_id", senderId.Value);
			if (!string.IsNullOrEmpty(htmlContent)) result.Add("html_content", htmlContent);
			if (!string.IsNullOrEmpty(textContent)) result.Add("plain_content", textContent);
			if (listIds.Any()) result.Add("list_ids", JArray.FromObject(listIds.ToArray()));
			if (segmentIds.Any()) result.Add("segment_ids", JArray.FromObject(segmentIds.ToArray()));
			if (categories.Any()) result.Add("categories", JArray.FromObject(categories.ToArray()));
			if (suppressionGroupId.HasValue) result.Add("suppression_group_id", suppressionGroupId.Value);
			if (!string.IsNullOrEmpty(customUnsubscribeUrl)) result.Add("custom_unsubscribe_url", customUnsubscribeUrl);
			if (!string.IsNullOrEmpty(ipPool)) result.Add("ip_pool", ipPool);
			return result;
		}
	}
}
