using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage Designs.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.api-docs.io/v3.0/designs-api">SendGrid documentation</a> for more information.
	/// </remarks>
	public class Designs : IDesigns
	{
		private const string _endpoint = "designs";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Designs" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal Designs(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Retrieve all designs, except the pre-built Twilio SendGrid designs.
		/// </summary>
		/// <param name="recordsPerPage">The number of records per page.</param>
		/// <param name="pageToken">The token corresponding to a specific page of results, as provided by metadata.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Design" />.
		/// </returns>
		public Task<Design[]> GetAllAsync(int recordsPerPage = 100, string pageToken = null, CancellationToken cancellationToken = default)
		{
			var request = _client
				.GetAsync(_endpoint)
				.WithArgument("page_size", recordsPerPage)
				.WithArgument("summary", "false")
				.WithCancellationToken(cancellationToken);

			if (!string.IsNullOrEmpty(pageToken)) request.WithArgument("page_token", pageToken);

			return request.AsSendGridObject<Design[]>("result");
		}

		/// <summary>
		/// Retrieve all pre-built designs.
		/// </summary>
		/// <param name="recordsPerPage">The number of records per page.</param>
		/// <param name="pageToken">The token corresponding to a specific page of results, as provided by metadata.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Alert" />.
		/// </returns>
		public Task<Design[]> GetAllPrebuiltAsync(int recordsPerPage = 100, string pageToken = null, CancellationToken cancellationToken = default)
		{
			var request = _client
				.GetAsync($"{_endpoint}/pre-builts")
				.WithArgument("page_size", recordsPerPage)
				.WithArgument("summary", "false")
				.WithCancellationToken(cancellationToken);

			if (!string.IsNullOrEmpty(pageToken)) request.WithArgument("page_token", pageToken);

			return request.AsSendGridObject<Design[]>("result");
		}
	}
}
