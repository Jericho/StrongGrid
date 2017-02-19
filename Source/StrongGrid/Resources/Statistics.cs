using Newtonsoft.Json;
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
	/// The stats APIs provide a read-only access to your SendGrid email statistics.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Stats/index.html
	/// </remarks>
	public class Statistics
	{
		private const string _endpoint = "stats";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Statistics" /> class.
		/// </summary>
		/// <param name="client">The HTTP client</param>
		public Statistics(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Get all global email statistics for a given date range.
		/// See: https://sendgrid.com/docs/API_Reference/Web_API_v3/Stats/global.html
		/// </summary>
		/// <param name="startDate">The starting date of the statistics to retrieve.</param>
		/// <param name="endDate">The end date of the statistics to retrieve. Defaults to today.</param>
		/// <param name="aggregatedBy">How to group the statistics, must be day|week|month</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Statistic" />.
		/// </returns>
		public Task<Statistic[]> GetGlobalStatisticsAsync(DateTime startDate, DateTime? endDate = null, AggregateBy aggregatedBy = AggregateBy.None, CancellationToken cancellationToken = default(CancellationToken))
		{
			var request = _client
				.GetAsync(_endpoint)
				.WithArgument("start_date", startDate.ToString("yyyy-MM-dd"))
				.WithCancellationToken(cancellationToken);

			if (endDate.HasValue) request.WithArgument("end_date", endDate.Value.ToString("yyyy-MM-dd"));
			if (aggregatedBy != AggregateBy.None) request.WithArgument("aggregated_by", JToken.Parse(JsonConvert.SerializeObject(aggregatedBy)).ToString());

			return request.AsSendGridObject<Statistic[]>();
		}

		/// <summary>
		/// Get email statistics for the given categories. If you don’t pass any parameters, the endpoint will return a sum for each category 10 at a time.
		/// See: https://sendgrid.com/docs/API_Reference/Web_API_v3/Stats/categories.html
		/// </summary>
		/// <param name="categories">The categories to get statistics for, up to 10</param>
		/// <param name="startDate">The starting date of the statistics to retrieve.</param>
		/// <param name="endDate">The end date of the statistics to retrieve. Defaults to today.</param>
		/// <param name="aggregatedBy">How to group the statistics, must be day|week|month</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Statistic" />.
		/// </returns>
		public Task<Statistic[]> GetCategoriesStatisticsAsync(IEnumerable<string> categories, DateTime startDate, DateTime? endDate = null, AggregateBy aggregatedBy = AggregateBy.None, CancellationToken cancellationToken = default(CancellationToken))
		{
			var request = _client.GetAsync($"categories/{_endpoint}")
				.WithArgument("start_date", startDate.ToString("yyyy-MM-dd"))
				.WithCancellationToken(cancellationToken);

			if (endDate.HasValue) request.WithArgument("end_date", endDate.Value.ToString("yyyy-MM-dd"));
			if (aggregatedBy != AggregateBy.None) request.WithArgument("aggregated_by", JToken.Parse(JsonConvert.SerializeObject(aggregatedBy)).ToString());
			if (categories != null && categories.Any())
			{
				foreach (var category in categories)
				{
					request.WithArgument("categories", category);
				}
			}

			return request.AsSendGridObject<Statistic[]>();
		}

		/// <summary>
		/// Get email statistics for the given subusers. You can add up to 10 subusers parameters, one for each subuser you want stats for.
		/// See: https://sendgrid.com/docs/API_Reference/Web_API_v3/Stats/subusers.html
		/// </summary>
		/// <param name="subusers">The subusers to get statistics for, up to 10</param>
		/// <param name="startDate">The starting date of the statistics to retrieve.</param>
		/// <param name="endDate">The end date of the statistics to retrieve. Defaults to today.</param>
		/// <param name="aggregatedBy">How to group the statistics, must be day|week|month</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Statistic" />.
		/// </returns>
		public Task<Statistic[]> GetSubusersStatisticsAsync(IEnumerable<string> subusers, DateTime startDate, DateTime? endDate = null, AggregateBy aggregatedBy = AggregateBy.None, CancellationToken cancellationToken = default(CancellationToken))
		{
			var request = _client
				.GetAsync($"subusers/{_endpoint}")
				.WithArgument("start_date", startDate.ToString("yyyy-MM-dd"))
				.WithCancellationToken(cancellationToken);

			if (endDate.HasValue) request.WithArgument("end_date", endDate.Value.ToString("yyyy-MM-dd"));
			if (aggregatedBy != AggregateBy.None) request.WithArgument("aggregated_by", JToken.Parse(JsonConvert.SerializeObject(aggregatedBy)).ToString());
			if (subusers != null && subusers.Any())
			{
				foreach (var subuser in subusers)
				{
					request.WithArgument("subusers", subuser);
				}
			}

			return request.AsSendGridObject<Statistic[]>();
		}

		/// <summary>
		/// Gets email statistics by country and state/province. Only supported for US and CA.
		/// See: https://sendgrid.com/docs/API_Reference/Web_API_v3/Stats/advanced.html
		/// </summary>
		/// <param name="country">US|CA</param>
		/// <param name="startDate">The starting date of the statistics to retrieve.</param>
		/// <param name="endDate">The end date of the statistics to retrieve. Defaults to today.</param>
		/// <param name="aggregatedBy">How to group the statistics, must be day|week|month</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Statistic" />.
		/// </returns>
		public Task<Statistic[]> GetCountryStatisticsAsync(string country, DateTime startDate, DateTime? endDate = null, AggregateBy aggregatedBy = AggregateBy.None, CancellationToken cancellationToken = default(CancellationToken))
		{
			var request = _client
				.GetAsync($"geo/{_endpoint}")
				.WithArgument("start_date", startDate.ToString("yyyy-MM-dd"))
				.WithCancellationToken(cancellationToken);

			if (endDate.HasValue) request.WithArgument("end_date", endDate.Value.ToString("yyyy-MM-dd"));
			if (aggregatedBy != AggregateBy.None) request.WithArgument("aggregated_by", JToken.Parse(JsonConvert.SerializeObject(aggregatedBy)).ToString());
			if (!string.IsNullOrEmpty(country)) request.WithArgument("country", country);

			return request.AsSendGridObject<Statistic[]>();
		}

		/// <summary>
		/// Gets email statistics by device type
		/// See: https://sendgrid.com/docs/API_Reference/Web_API_v3/Stats/advanced.html
		/// </summary>
		/// <param name="startDate">The starting date of the statistics to retrieve.</param>
		/// <param name="endDate">The end date of the statistics to retrieve. Defaults to today.</param>
		/// <param name="aggregatedBy">How to group the statistics, must be day|week|month</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Statistic" />.
		/// </returns>
		public Task<Statistic[]> GetDeviceTypesStatisticsAsync(DateTime startDate, DateTime? endDate = null, AggregateBy aggregatedBy = AggregateBy.None, CancellationToken cancellationToken = default(CancellationToken))
		{
			var request = _client
				.GetAsync($"devices/{_endpoint}")
				.WithArgument("start_date", startDate.ToString("yyyy-MM-dd"))
				.WithCancellationToken(cancellationToken);

			if (endDate.HasValue) request.WithArgument("end_date", endDate.Value.ToString("yyyy-MM-dd"));
			if (aggregatedBy != AggregateBy.None) request.WithArgument("aggregated_by", JToken.Parse(JsonConvert.SerializeObject(aggregatedBy)).ToString());

			return request.AsSendGridObject<Statistic[]>();
		}

		/// <summary>
		/// Get email statistics by client type
		/// See: https://sendgrid.com/docs/API_Reference/Web_API_v3/Stats/advanced.html
		/// </summary>
		/// <param name="startDate">The starting date of the statistics to retrieve.</param>
		/// <param name="endDate">The end date of the statistics to retrieve. Defaults to today.</param>
		/// <param name="aggregatedBy">How to group the statistics, must be day|week|month</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Statistic" />.
		/// </returns>
		public Task<Statistic[]> GetClientTypesStatisticsAsync(DateTime startDate, DateTime? endDate = null, AggregateBy aggregatedBy = AggregateBy.None, CancellationToken cancellationToken = default(CancellationToken))
		{
			var request = _client
				.GetAsync($"clients/{_endpoint}")
				.WithArgument("start_date", startDate.ToString("yyyy-MM-dd"))
				.WithCancellationToken(cancellationToken);

			if (endDate.HasValue) request.WithArgument("end_date", endDate.Value.ToString("yyyy-MM-dd"));
			if (aggregatedBy != AggregateBy.None) request.WithArgument("aggregated_by", JToken.Parse(JsonConvert.SerializeObject(aggregatedBy)).ToString());

			return request.AsSendGridObject<Statistic[]>();
		}

		/// <summary>
		/// Gets email statistics by mailbox provider
		/// See: https://sendgrid.com/docs/API_Reference/Web_API_v3/Stats/advanced.html
		/// </summary>
		/// <param name="providers">The mailbox providers to get statistics for, up to 10</param>
		/// <param name="startDate">The starting date of the statistics to retrieve.</param>
		/// <param name="endDate">The end date of the statistics to retrieve. Defaults to today.</param>
		/// <param name="aggregatedBy">How to group the statistics, must be day|week|month</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Statistic" />.
		/// </returns>
		public Task<Statistic[]> GetInboxProvidersStatisticsAsync(IEnumerable<string> providers, DateTime startDate, DateTime? endDate = null, AggregateBy aggregatedBy = AggregateBy.None, CancellationToken cancellationToken = default(CancellationToken))
		{
			var request = _client
				.GetAsync($"mailbox_providers/{_endpoint}")
				.WithArgument("start_date", startDate.ToString("yyyy-MM-dd"))
				.WithCancellationToken(cancellationToken);

			if (endDate.HasValue) request.WithArgument("end_date", endDate.Value.ToString("yyyy-MM-dd"));
			if (aggregatedBy != AggregateBy.None) request.WithArgument("aggregated_by", JToken.Parse(JsonConvert.SerializeObject(aggregatedBy)).ToString());

			if (providers != null && providers.Any())
			{
				foreach (var provider in providers)
				{
					request.WithArgument("mailbox_providers", provider);
				}
			}

			return request.AsSendGridObject<Statistic[]>();
		}

		/// <summary>
		/// Gets email statistics by browser
		/// See: https://sendgrid.com/docs/API_Reference/Web_API_v3/Stats/advanced.html
		/// </summary>
		/// <param name="browsers">The browsers to get statistics for, up to 10</param>
		/// <param name="startDate">The starting date of the statistics to retrieve.</param>
		/// <param name="endDate">The end date of the statistics to retrieve. Defaults to today.</param>
		/// <param name="aggregatedBy">How to group the statistics, must be day|week|month</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Statistic" />.
		/// </returns>
		public Task<Statistic[]> GetBrowsersStatisticsAsync(IEnumerable<string> browsers, DateTime startDate, DateTime? endDate = null, AggregateBy aggregatedBy = AggregateBy.None, CancellationToken cancellationToken = default(CancellationToken))
		{
			var request = _client
				.GetAsync($"browsers/{_endpoint}")
				.WithArgument("start_date", startDate.ToString("yyyy-MM-dd"))
				.WithCancellationToken(cancellationToken);

			if (endDate.HasValue) request.WithArgument("end_date", endDate.Value.ToString("yyyy-MM-dd"));
			if (aggregatedBy != AggregateBy.None) request.WithArgument("aggregated_by", JToken.Parse(JsonConvert.SerializeObject(aggregatedBy)).ToString());

			if (browsers != null && browsers.Any())
			{
				foreach (var browser in browsers)
				{
					request.WithArgument("browsers", browser);
				}
			}

			return request.AsSendGridObject<Statistic[]>();
		}
	}
}
