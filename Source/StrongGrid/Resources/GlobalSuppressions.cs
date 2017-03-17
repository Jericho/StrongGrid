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
	/// Allows you to manage email addresses that will not receive any emails.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Suppression_Management/global_suppressions.html
	/// </remarks>
	public class GlobalSuppressions
	{
		private const string _endpoint = "asm/suppressions/global";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="GlobalSuppressions" /> class.
		/// </summary>
		/// <param name="client">The HTTP client</param>
		public GlobalSuppressions(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Get all globally unsubscribed email addresses.
		/// </summary>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="GlobalSuppression"/>.
		/// </returns>
		public Task<GlobalSuppression[]> GetAllAsync(DateTime? startDate = null, DateTime? endDate = null, int limit = 50, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync("suppression/unsubscribes")
				.WithArgument("start_time", startDate?.ToUnixTime())
				.WithArgument("end_time", endDate?.ToUnixTime())
				.WithArgument("limit", limit)
				.WithArgument("offset", offset)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<GlobalSuppression[]>();
		}

		/// <summary>
		/// Check if a recipient address is in the global suppressions group.
		/// </summary>
		/// <param name="email">email address to check</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the email address is in the global suppression group; otherwise, <c>false</c>.
		/// </returns>
		public async Task<bool> IsUnsubscribedAsync(string email, CancellationToken cancellationToken = default(CancellationToken))
		{
			var responseContent = await _client
				.GetAsync($"{_endpoint}/{email}")
				.WithCancellationToken(cancellationToken)
				.AsString(null)
				.ConfigureAwait(false);

			// If the email address is on the global suppression list, the response will look like this:
			//  {
			//      "recipient_email": "{email}"
			//  }
			// If the email address is not on the global suppression list, the response will be empty
			//
			// Therefore, we check for the presence of the 'recipient_email' to indicate if the email
			// address is on the global suppression list or not.
			var dynamicObject = JObject.Parse(responseContent);
			var propertyDictionary = (IDictionary<string, JToken>)dynamicObject;
			return propertyDictionary.ContainsKey("recipient_email");
		}

		/// <summary>
		/// Add recipient addresses to the global suppression group.
		/// </summary>
		/// <param name="emails">Array of email addresses to add to the suppression group</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task AddAsync(IEnumerable<string> emails, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject(new JProperty("recipient_emails", JArray.FromObject(emails.ToArray())));
			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Delete a recipient email from the global suppressions group.
		/// </summary>
		/// <param name="email">email address to be removed from the global suppressions group</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task RemoveAsync(string email, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync($"{_endpoint}/{email}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}
	}
}
