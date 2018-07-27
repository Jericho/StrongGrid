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
	/// Allows you to manage <see cref="Campaign">campaigns</see>.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.ICampaigns" />
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/campaigns.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public class Campaigns : ICampaigns
	{
		private const string _endpoint = "campaigns";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Campaigns" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal Campaigns(Pathoschild.Http.Client.IClient client)
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
		/// <param name="editor">The editor used in the UI. Allowed values: code, design.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <remarks>
		/// Note: In order to send or schedule the campaign, you will be required to provide a subject, sender ID, content (we suggest both html and plain text), and at least one list or segment ID. This information is not required when you create a campaign.
		/// </remarks>
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
			Parameter<EditorType?> editor = default(Parameter<EditorType?>),
			CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = CreateJObject(title, senderId, subject, htmlContent, textContent, listIds, segmentIds, categories, suppressionGroupId, customUnsubscribeUrl, ipPool, editor);
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
		/// <param name="cancellationToken">Cancellation token.</param>
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
		/// <param name="campaignId">The id of the campaign.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
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
		/// <param name="campaignId">The id of the campaign.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
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
		/// Update a campaign.
		/// </summary>
		/// <param name="campaignId">The id of the campaign.</param>
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
		/// <param name="editor">The editor used in the UI. Allowed values: code, design.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
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
			Parameter<EditorType?> editor = default(Parameter<EditorType?>),
			CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = CreateJObject(title, senderId, subject, htmlContent, textContent, listIds, segmentIds, categories, suppressionGroupId, customUnsubscribeUrl, ipPool, editor);
			return _client
				.PatchAsync($"{_endpoint}/{campaignId}")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Campaign>();
		}

		/// <summary>
		/// Send a campaign immediately.
		/// </summary>
		/// <param name="campaignId">The id of the campaign.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
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
		/// <param name="campaignId">The id of the campaign.</param>
		/// <param name="sendOn">The send on.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
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
		/// Change the date a campaign is scheduled to be sent.
		/// </summary>
		/// <param name="campaignId">The id of the campaign.</param>
		/// <param name="sendOn">The send on.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
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
		/// Retrieve the date a campaign is scheduled to be sent.
		/// </summary>
		/// <param name="campaignId">The id of the campaign.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
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
		/// <param name="campaignId">The id of the campaign.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
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
		/// Send a test campaign.
		/// </summary>
		/// <param name="campaignId">The id of the campaign.</param>
		/// <param name="emailAddresses">The email addresses.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		/// <exception cref="System.ArgumentException">You must specify at least one email address.</exception>
		public Task SendTestAsync(long campaignId, IEnumerable<string> emailAddresses, CancellationToken cancellationToken = default(CancellationToken))
		{
			emailAddresses = emailAddresses ?? Enumerable.Empty<string>();
			if (!emailAddresses.Any()) throw new ArgumentException("You must specify at least one email address");

			var data = new JObject();
			if (emailAddresses.Count() == 1) data.AddPropertyIfValue("to", emailAddresses.First());
			else data.AddPropertyIfValue("to", emailAddresses);

			return _client
				.PostAsync($"{_endpoint}/{campaignId}/schedules/test")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		private static JObject CreateJObject(Parameter<string> title, Parameter<long?> senderId, Parameter<string> subject, Parameter<string> htmlContent, Parameter<string> textContent, Parameter<IEnumerable<long>> listIds, Parameter<IEnumerable<long>> segmentIds, Parameter<IEnumerable<string>> categories, Parameter<long?> suppressionGroupId, Parameter<string> customUnsubscribeUrl, Parameter<string> ipPool, Parameter<EditorType?> editor)
		{
			var result = new JObject();
			result.AddPropertyIfValue("title", title);
			result.AddPropertyIfValue("subject", subject);
			result.AddPropertyIfValue("sender_id", senderId);
			result.AddPropertyIfValue("html_content", htmlContent);
			result.AddPropertyIfValue("plain_content", textContent);
			result.AddPropertyIfValue("list_ids", listIds);
			result.AddPropertyIfValue("segment_ids", segmentIds);
			result.AddPropertyIfValue("categories", categories);
			result.AddPropertyIfValue("suppression_group_id", suppressionGroupId);
			result.AddPropertyIfValue("custom_unsubscribe_url", customUnsubscribeUrl);
			result.AddPropertyIfValue("ip_pool", ipPool);
			result.AddPropertyIfEnumValue("editor", editor);
			return result;
		}
	}
}
