using Pathoschild.Http.Client;
using StrongGrid.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to retrieve metrics that define the quality of your email program.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.IEngagementQuality" />
	/// <remarks>
	/// See <a href="https://docs.sendgrid.com/api-reference/sendgrid-engagement-quality-api/overview">SendGrid documentation</a> for more information.
	/// </remarks>
	public class EngagementQuality : IEngagementQuality
	{
		private const string _endpoint = "engagementquality/scores";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="EngagementQuality" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal EngagementQuality(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <inheritdoc/>
		public Task<EngagementQualityScore[]> GetScoresAsync(DateTime? startDate = null, DateTime? endDate = null, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync(_endpoint)
				.WithArgument("from", startDate?.ToString("yyyy-MM-dd"))
				.WithArgument("to", endDate?.ToString("yyyy-MM-dd"))
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<EngagementQualityScore[]>("result");
		}
	}
}
