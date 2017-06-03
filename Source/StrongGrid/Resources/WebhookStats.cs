using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Provides statistics on your Webhook usage.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Stats/parse.html
	/// </remarks>
	public class WebhookStats : IWebhookStats
	{
		private const string _endpoint = "user/webhooks/parse/stats";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="WebhookStats" /> class.
		/// </summary>
		/// <param name="client">The HTTP client</param>
		internal WebhookStats(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Get statistics for Inbound Parse Webhook usage.
		/// </summary>
		/// <param name="startDate">The starting date of the statistics to retrieve.</param>
		/// <param name="endDate">The end date of the statistics to retrieve. Defaults to today.</param>
		/// <param name="aggregatedBy">How to group the statistics, must be day|week|month</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Statistic" />.
		/// </returns>
		public Task<Statistic[]> GetInboundParseUsageAsync(DateTime startDate, DateTime? endDate = null, AggregateBy aggregatedBy = AggregateBy.None, CancellationToken cancellationToken = default(CancellationToken))
		{
			var request = _client
				.GetAsync(_endpoint)
				.WithArgument("start_date", startDate.ToString("yyyy-MM-dd"))
				.WithCancellationToken(cancellationToken);

			if (endDate.HasValue) request.WithArgument("end_date", endDate.Value.ToString("yyyy-MM-dd"));
			if (aggregatedBy != AggregateBy.None) request.WithArgument("aggregated_by", JToken.Parse(JsonConvert.SerializeObject(aggregatedBy)).ToString());

			return request.AsSendGridObject<Statistic[]>();
		}
	}
}
