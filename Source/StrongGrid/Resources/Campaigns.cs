using Newtonsoft.Json.Linq;
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
		private IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Campaigns" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public Campaigns(IClient client, string endpoint = "/campaigns")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Create a campaign.
		/// </summary>
		/// <param name="title">The title.</param>
		/// <param name="suppressionGroupId">The suppression group identifier.</param>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="listIds">The list ids.</param>
		/// <param name="segmentIds">The segment ids.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customUnsubscribeUrl">The custom unsubscribe URL.</param>
		/// <param name="ipPool">The ip pool.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Campaign" />.
		/// </returns>
		public async Task<Campaign> CreateAsync(string title, long suppressionGroupId, long senderId, string subject = null, string htmlContent = null, string textContent = null, IEnumerable<long> listIds = null, IEnumerable<long> segmentIds = null, IEnumerable<string> categories = null, string customUnsubscribeUrl = null, string ipPool = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			listIds = listIds ?? Enumerable.Empty<long>();
			segmentIds = segmentIds ?? Enumerable.Empty<long>();
			categories = categories ?? Enumerable.Empty<string>();

			var data = CreateJObjectForCampaign(title, suppressionGroupId, senderId, subject, htmlContent, textContent, listIds, segmentIds, categories, customUnsubscribeUrl, ipPool);
			var response = await _client.PostAsync(_endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var campaign = JObject.Parse(responseContent).ToObject<Campaign>();
			return campaign;
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
		public async Task<Campaign[]> GetAllAsync(int limit = 10, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}?limit={1}&offset={2}", _endpoint, limit, offset);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "result": [
			//     {
			//       "id": 986724,
			//       "title": "March Newsletter",
			//       "subject": "New Products for Spring!",
			//       "sender_id": 124451,
			//       "list_ids": [
			//         110,
			//         124
			//       ],
			//       "segment_ids": [
			//         110
			//       ],
			//       "categories": [
			//         "spring line"
			//       ],
			//       "suppression_group_id": 42,
			//       "custom_unsubscribe_url": "",
			//       "ip_pool": "marketing",
			//       "html_content": "<html><head><title></title></head><body><p>Check out our spring line!</p></body></html>",
			//       "plain_content": "Check out our spring line!",
			//       "status": "Draft"
			//     }
			//  ]
			// }
			// We use a dynamic object to get rid of the 'result' property and simply return an array of campaigns
			dynamic dynamicObject = JObject.Parse(responseContent);
			dynamic dynamicArray = dynamicObject.result;

			var campaigns = dynamicArray.ToObject<Campaign[]>();
			return campaigns;
		}

		/// <summary>
		/// Retrieve a campaign.
		/// </summary>
		/// <param name="campaignId">The id of the campaign</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Campaign" />.
		/// </returns>
		public async Task<Campaign> GetAsync(long campaignId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(string.Format("{0}/{1}", _endpoint, campaignId), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var campaign = JObject.Parse(responseContent).ToObject<Campaign>();
			return campaign;
		}

		/// <summary>
		/// Delete a campaign.
		/// </summary>
		/// <param name="campaignId">The id of the campaign</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public async Task DeleteAsync(long campaignId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.DeleteAsync(string.Format("{0}/{1}", _endpoint, campaignId), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Update a campaign
		/// </summary>
		/// <param name="campaignId">The id of the campaign</param>
		/// <param name="title">The title.</param>
		/// <param name="suppressionGroupId">The suppression group identifier.</param>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">Content of the HTML.</param>
		/// <param name="textContent">Content of the text.</param>
		/// <param name="listIds">The list ids.</param>
		/// <param name="segmentIds">The segment ids.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customUnsubscribeUrl">The custom unsubscribe URL.</param>
		/// <param name="ipPool">The ip pool.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Campaign" />.
		/// </returns>
		public async Task<Campaign> UpdateAsync(long campaignId, string title = null, long? suppressionGroupId = null, long? senderId = null, string subject = null, string htmlContent = null, string textContent = null, IEnumerable<long> listIds = null, IEnumerable<long> segmentIds = null, IEnumerable<string> categories = null, string customUnsubscribeUrl = null, string ipPool = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			listIds = listIds ?? Enumerable.Empty<long>();
			segmentIds = segmentIds ?? Enumerable.Empty<long>();
			categories = categories ?? Enumerable.Empty<string>();

			var data = CreateJObjectForCampaign(title, suppressionGroupId, senderId, subject, htmlContent, textContent, listIds, segmentIds, categories, customUnsubscribeUrl, ipPool);
			var response = await _client.PatchAsync(string.Format("{0}/{1}", _endpoint, campaignId), data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var campaign = JObject.Parse(responseContent).ToObject<Campaign>();
			return campaign;
		}

		/// <summary>
		/// Send a campaign immediately.
		/// </summary>
		/// <param name="campaignId">The id of the campaign</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public async Task SendNowAsync(long campaignId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = (JObject)null;
			var response = await _client.PostAsync(string.Format("{0}/{1}/schedules/now", _endpoint, campaignId), data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
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
		public async Task ScheduleAsync(long campaignId, DateTime sendOn, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "send_at", sendOn.ToUnixTime() }
			};
			var response = await _client.PostAsync(string.Format("{0}/{1}/schedules", _endpoint, campaignId), data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
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
		public async Task RescheduleAsync(long campaignId, DateTime sendOn, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "send_at", sendOn.ToUnixTime() }
			};
			var response = await _client.PatchAsync(string.Format("{0}/{1}/schedules", _endpoint, campaignId), data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
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
			var response = await _client.GetAsync(string.Format("{0}/{1}/schedules", _endpoint, campaignId), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// {
			//    "send_at": 1489771528
			// }
			// We use a dynamic object to get rid of the 'send_at' property and simply return the DateTime value
			dynamic dynamicObject = JObject.Parse(responseContent);
			dynamic dynamicValue = dynamicObject.send_at;

			var unixTime = (long)dynamicValue;
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
		public async Task UnscheduleAsync(long campaignId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.DeleteAsync(string.Format("{0}/{1}/schedules", _endpoint, campaignId), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
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
		public async Task SendTestAsync(long campaignId, IEnumerable<string> emailAddresses, CancellationToken cancellationToken = default(CancellationToken))
		{
			emailAddresses = emailAddresses ?? Enumerable.Empty<string>();
			if (!emailAddresses.Any()) throw new ArgumentException("You must specify at least one email address");

			var data = new JObject();
			if (emailAddresses.Count() == 1) data.Add("to", emailAddresses.First());
			else data.Add("to", JArray.FromObject(emailAddresses.ToArray()));

			var response = await _client.PostAsync(string.Format("{0}/{1}/schedules/test", _endpoint, campaignId), data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		private static JObject CreateJObjectForCampaign(string title = null, long? suppressionGroupId = null, long? senderId = null, string subject = null, string htmlContent = null, string textContent = null, IEnumerable<long> listIds = null, IEnumerable<long> segmentIds = null, IEnumerable<string> categories = null, string customUnsubscribeUrl = null, string ipPool = null)
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
