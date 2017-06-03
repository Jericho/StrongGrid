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
	/// Allows you to manage email addresses that have been declared invalid.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/invalid_emails.html
	/// </remarks>
	public class InvalidEmails : IInvalidEmails
	{
		private const string _endpoint = "suppression/invalid_emails";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidEmails" /> class.
		/// </summary>
		/// <param name="client">The HTTP client</param>
		internal InvalidEmails(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Retrieve all invalid emails.
		/// </summary>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="InvalidEmail" />.
		/// </returns>
		public Task<InvalidEmail[]> GetAllAsync(DateTime? startDate = null, DateTime? endDate = null, int limit = 25, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync(_endpoint)
				.WithArgument("start_time", startDate?.ToUnixTime())
				.WithArgument("end_time", endDate?.ToUnixTime())
				.WithArgument("limit", limit)
				.WithArgument("offset", offset)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<InvalidEmail[]>();
		}

		/// <summary>
		/// Retrieve invalid emails for a given email address.
		/// </summary>
		/// <param name="emailAddress">The email address.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="InvalidEmail" />.
		/// </returns>
		public Task<InvalidEmail[]> GetAsync(string emailAddress, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/{emailAddress}")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<InvalidEmail[]>();
		}

		/// <summary>
		/// Delete all invalid emails.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "delete_all", true }
			};
			return _client
				.DeleteAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Delete multiple invalid emails.
		/// </summary>
		/// <param name="emailAddresses">The email addresses.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteMultipleAsync(IEnumerable<string> emailAddresses, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "emails", JArray.FromObject(emailAddresses.ToArray()) }
			};
			return _client
				.DeleteAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Delete a specific invalid emails.
		/// </summary>
		/// <param name="emailAddress">The email address.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string emailAddress, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync($"{_endpoint}/{emailAddress}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}
	}
}
