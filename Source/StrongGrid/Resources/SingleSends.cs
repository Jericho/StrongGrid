using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage <see cref="SingleSend">single sends (AKA campaigns).</see>.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.ISingleSends" />
	/// <remarks>
	/// See <a href="https://sendgrid.api-docs.io/v3.0/single-sends">SendGrid documentation</a> for more information.
	/// </remarks>
	public class SingleSends : ISingleSends
	{
		private const string _endpoint = "marketing/campaigns";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="SingleSends" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal SingleSends(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Create a single send (AKA campaign).
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customUnsubscribeUrl">The custom unsubscribe URL.</param>
		/// <param name="suppressionGroupId">The suppression group identifier.</param>
		/// <param name="listIds">The list ids.</param>
		/// <param name="segmentIds">The segment ids.</param>
		/// <param name="templateId">The template id.</param>
		/// <param name="ipPool">The ip pool.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="SingleSend" />.
		/// </returns>
		public Task<SingleSend> CreateAsync(
			string name,
			long senderId,
			Parameter<IEnumerable<string>> categories = default,
			Parameter<string> customUnsubscribeUrl = default,
			Parameter<long?> suppressionGroupId = default,
			Parameter<IEnumerable<string>> listIds = default,
			Parameter<IEnumerable<string>> segmentIds = default,
			Parameter<string> templateId = default,
			Parameter<string> ipPool = default,
			CancellationToken cancellationToken = default)
		{
			var data = CreateJObject(name, senderId, SingleSendStatus.Draft, categories, customUnsubscribeUrl, suppressionGroupId, listIds, segmentIds, default, templateId, ipPool);
			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SingleSend>();
		}

		/// <summary>
		/// Update a single send (AKA campaign).
		/// </summary>
		/// <param name="singleSendId">The id of the single send.</param>
		/// <param name="name">The name.</param>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customUnsubscribeUrl">The custom unsubscribe URL.</param>
		/// <param name="suppressionGroupId">The suppression group identifier.</param>
		/// <param name="listIds">The list ids.</param>
		/// <param name="segmentIds">The segment ids.</param>
		/// <param name="templateId">The template id.</param>
		/// <param name="ipPool">The ip pool.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="SingleSend" />.
		/// </returns>
		public Task<SingleSend> UpdateAsync(
			string singleSendId,
			Parameter<string> name = default,
			Parameter<long> senderId = default,
			Parameter<IEnumerable<string>> categories = default,
			Parameter<string> customUnsubscribeUrl = default,
			Parameter<long?> suppressionGroupId = default,
			Parameter<IEnumerable<string>> listIds = default,
			Parameter<IEnumerable<string>> segmentIds = default,
			Parameter<string> templateId = default,
			Parameter<string> ipPool = default,
			CancellationToken cancellationToken = default)
		{
			var data = CreateJObject(name, senderId, default, categories, customUnsubscribeUrl, suppressionGroupId, listIds, segmentIds, default, templateId, ipPool);
			return _client
				.PatchAsync($"{_endpoint}/{singleSendId}")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SingleSend>();
		}

		/// <summary>
		/// Delete multiple single sends (AKA campaigns).
		/// </summary>
		/// <param name="singleSendIds">The id of the single sends.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(IEnumerable<string> singleSendIds, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync(_endpoint)
				.WithArgument("ids", string.Join(",", singleSendIds))
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Delete a single send (AKA campaign).
		/// </summary>
		/// <param name="singleSendId">The id of the single send.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string singleSendId, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/{singleSendId}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Retrieve a single send (AKA campaign).
		/// </summary>
		/// <param name="singleSendId">The id of the single send.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="SingleSend" />.
		/// </returns>
		public Task<SingleSend> GetAsync(string singleSendId, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/{singleSendId}")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SingleSend>();
		}

		/// <summary>
		/// Retrieve all single sends.
		/// </summary>
		/// <param name="recordsPerPage">The number of records per page.</param>
		/// <param name="pageToken">The token corresponding to a specific page of results, as provided by metadata.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="SingleSend" />.
		/// </returns>
		public Task<PaginatedResponse<SingleSend>> GetAllAsync(int recordsPerPage = 10, string pageToken = null, CancellationToken cancellationToken = default)
		{
			var request = _client
				.GetAsync(_endpoint)
				.WithArgument("page_size", recordsPerPage)
				.WithCancellationToken(cancellationToken);

			if (!string.IsNullOrEmpty(pageToken)) request.WithArgument("page_token", pageToken);

			return request.AsPaginatedResponse<SingleSend>("result");
		}

		/// <summary>
		/// Send a single send immediately.
		/// </summary>
		/// <param name="singleSendId">The id of the single send.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task SendNowAsync(string singleSendId, CancellationToken cancellationToken = default)
		{
			var data = new JObject()
			{
				{ "send_at", "now" }
			};
			return _client
				.PutAsync($"{_endpoint}/{singleSendId}/schedule")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Schedule a single send to be sent at a later time.
		/// </summary>
		/// <param name="singleSendId">The id of the single send.</param>
		/// <param name="sendOn">The send on.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task ScheduleAsync(string singleSendId, DateTime sendOn, CancellationToken cancellationToken = default)
		{
			var data = new JObject
			{
				{ "send_at", sendOn.ToUniversalTime().ToString("o") }
			};
			return _client
				.PutAsync($"{_endpoint}/{singleSendId}/schedule")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Unschedule a scheduled single send.
		/// </summary>
		/// <param name="singleSendId">The id of the single send.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task UnscheduleAsync(string singleSendId, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/{singleSendId}/schedule")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		private static JObject CreateJObject(string name, long senderId, Parameter<SingleSendStatus> status, Parameter<IEnumerable<string>> categories, Parameter<string> customUnsubscribeUrl, Parameter<long?> suppressionGroupId, Parameter<IEnumerable<string>> listIds, Parameter<IEnumerable<string>> segmentIds, Parameter<DateTime?> sendOn, Parameter<string> templateId, Parameter<string> ipPool)
		{
			var result = new JObject();
			result.AddPropertyIfValue("name", name);
			result.AddPropertyIfValue("categories", categories);
			result.AddPropertyIfValue("sender_id", senderId);
			result.AddPropertyIfEnumValue("status", status);
			result.AddPropertyIfValue("custom_unsubscribe_url", customUnsubscribeUrl);
			result.AddPropertyIfValue("suppression_group_id", suppressionGroupId);
			result.AddPropertyIfValue("send_at", sendOn);
			result.AddPropertyIfValue("template_id", templateId);
			result.AddPropertyIfValue("ip_pool", ipPool);

			if (listIds.HasValue && listIds.Value != null && listIds.Value.Any())
			{
				result.AddPropertyIfValue("filter/list_ids", listIds);
			}

			if (segmentIds.HasValue && segmentIds.Value != null && segmentIds.Value.Any())
			{
				result.AddPropertyIfValue("filter/segment_ids", segmentIds);
			}

			return result;
		}
	}
}
