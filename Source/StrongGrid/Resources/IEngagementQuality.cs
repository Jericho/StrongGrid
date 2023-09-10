using StrongGrid.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to retrieve metrics that define the quality of your email program.
	/// </summary>
	/// <remarks>
	/// See <a href="https://docs.sendgrid.com/api-reference/sendgrid-engagement-quality-api/overview">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface IEngagementQuality
	{
		/// <summary>
		/// Retrieve the engagement scores.
		/// </summary>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		Task<EngagementQualityScore[]> GetScoresAsync(DateTime? startDate = null, DateTime? endDate = null, string onBehalfOf = null, CancellationToken cancellationToken = default);
	}
}
