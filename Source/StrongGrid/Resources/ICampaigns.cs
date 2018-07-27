using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage <see cref="Campaign">campaigns</see>.
	/// </summary>
	/// <remarks>
	/// See <a href="">SendGrid documentation</a> for more information.
	/// See also: https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/campaigns.html.
	/// </remarks>
	public interface ICampaigns
	{
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
		/// <returns>
		/// The <see cref="Campaign" />.
		/// </returns>
		Task<Campaign> CreateAsync(
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
			CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve all campaigns.
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Campaign" />.
		/// </returns>
		Task<Campaign[]> GetAllAsync(int limit = 10, int offset = 0, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve a campaign.
		/// </summary>
		/// <param name="campaignId">The id of the campaign.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Campaign" />.
		/// </returns>
		Task<Campaign> GetAsync(long campaignId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete a campaign.
		/// </summary>
		/// <param name="campaignId">The id of the campaign.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(long campaignId, CancellationToken cancellationToken = default(CancellationToken));

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
		Task<Campaign> UpdateAsync(
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
			CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Send a campaign immediately.
		/// </summary>
		/// <param name="campaignId">The id of the campaign.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task SendNowAsync(long campaignId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Schedule a campaign to be sewnt at a later time.
		/// </summary>
		/// <param name="campaignId">The id of the campaign.</param>
		/// <param name="sendOn">The send on.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task ScheduleAsync(long campaignId, DateTime sendOn, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Change the date a campaign is scheduled to be sent.
		/// </summary>
		/// <param name="campaignId">The id of the campaign.</param>
		/// <param name="sendOn">The send on.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task RescheduleAsync(long campaignId, DateTime sendOn, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve the date a campaign is scheduled to be sent.
		/// </summary>
		/// <param name="campaignId">The id of the campaign.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="DateTime" /> the campaign is cheduled to be sent or <c>null</c> if the campaign is
		/// not scheduled to be sent.
		/// </returns>
		Task<DateTime?> GetScheduledDateAsync(long campaignId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Unschedule a scheduled campaign.
		/// </summary>
		/// <param name="campaignId">The id of the campaign.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task UnscheduleAsync(long campaignId, CancellationToken cancellationToken = default(CancellationToken));

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
		Task SendTestAsync(long campaignId, IEnumerable<string> emailAddresses, CancellationToken cancellationToken = default(CancellationToken));
	}
}
