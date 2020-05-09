using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage <see cref="SingleSend">single sends (AKA campaigns).</see>.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.api-docs.io/v3.0/single-sends">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface ISingleSends
	{
		/// <summary>
		/// Create a single send (AKA campaign).
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">The HTML content.</param>
		/// <param name="textContent">The plain text content.</param>
		/// <param name="designId">The design identifier.</param>
		/// <param name="editor">The type of editor.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customUnsubscribeUrl">The custom unsubscribe URL.</param>
		/// <param name="suppressionGroupId">The suppression group identifier.</param>
		/// <param name="listIds">The list ids.</param>
		/// <param name="segmentIds">The segment ids.</param>
		/// <param name="ipPool">The ip pool.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="SingleSend" />.
		/// </returns>
		Task<SingleSend> CreateAsync(
			string name,
			long senderId,
			Parameter<string> subject = default,
			Parameter<string> htmlContent = default,
			Parameter<string> textContent = default,
			Parameter<string> designId = default,
			Parameter<EditorType> editor = default,
			Parameter<IEnumerable<string>> categories = default,
			Parameter<string> customUnsubscribeUrl = default,
			Parameter<long?> suppressionGroupId = default,
			Parameter<IEnumerable<string>> listIds = default,
			Parameter<IEnumerable<string>> segmentIds = default,
			Parameter<string> ipPool = default,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Update a single send (AKA campaign).
		/// </summary>
		/// <param name="singleSendId">The id of the single send.</param>
		/// <param name="name">The name.</param>
		/// <param name="senderId">The sender identifier.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlContent">The HTML content.</param>
		/// <param name="textContent">The plain text content.</param>
		/// <param name="designId">The design identifier.</param>
		/// <param name="editor">The type of editor.</param>
		/// <param name="categories">The categories.</param>
		/// <param name="customUnsubscribeUrl">The custom unsubscribe URL.</param>
		/// <param name="suppressionGroupId">The suppression group identifier.</param>
		/// <param name="listIds">The list ids.</param>
		/// <param name="segmentIds">The segment ids.</param>
		/// <param name="ipPool">The ip pool.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="SingleSend" />.
		/// </returns>
		Task<SingleSend> UpdateAsync(
			string singleSendId,
			Parameter<string> name = default,
			Parameter<long> senderId = default,
			Parameter<string> subject = default,
			Parameter<string> htmlContent = default,
			Parameter<string> textContent = default,
			Parameter<string> designId = default,
			Parameter<EditorType> editor = default,
			Parameter<IEnumerable<string>> categories = default,
			Parameter<string> customUnsubscribeUrl = default,
			Parameter<long?> suppressionGroupId = default,
			Parameter<IEnumerable<string>> listIds = default,
			Parameter<IEnumerable<string>> segmentIds = default,
			Parameter<string> ipPool = default,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete multiple single sends (AKA campaigns).
		/// </summary>
		/// <param name="singleSendIds">The id of the single sends.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(IEnumerable<string> singleSendIds, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete a single send (AKA campaign).
		/// </summary>
		/// <param name="singleSendId">The id of the single send.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(string singleSendId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve a single send (AKA campaign).
		/// </summary>
		/// <param name="singleSendId">The id of the single send.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Models.Legacy.Campaign" />.
		/// </returns>
		Task<SingleSend> GetAsync(string singleSendId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve all single sends.
		/// </summary>
		/// <param name="recordsPerPage">The number of records per page.</param>
		/// <param name="pageToken">The token corresponding to a specific page of results, as provided by metadata.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="SingleSend" />.
		/// </returns>
		Task<PaginatedResponse<SingleSend>> GetAllAsync(int recordsPerPage = 10, string pageToken = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Send a single send immediately.
		/// </summary>
		/// <param name="singleSendId">The id of the single send.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task SendNowAsync(string singleSendId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Schedule a single send to be sent at a later time.
		/// </summary>
		/// <param name="singleSendId">The id of the single send.</param>
		/// <param name="sendOn">The send on.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task ScheduleAsync(string singleSendId, DateTime sendOn, CancellationToken cancellationToken = default);

		/// <summary>
		/// Unschedule a scheduled single send.
		/// </summary>
		/// <param name="singleSendId">The id of the single send.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task UnscheduleAsync(string singleSendId, CancellationToken cancellationToken = default);
	}
}
