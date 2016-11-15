using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	public class Lists
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Initializes a new instance of the Lists class
		/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/contactdb.html
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint, do not prepend slash</param>
		public Lists(IClient client, string endpoint = "/contactdb/lists")
		{
			_endpoint = endpoint;
			_client = client;
		}

		public async Task<List> CreateAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				new JProperty("name", name)
			};
			var response = await _client.PostAsync(_endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var bulkUpsertResult = JObject.Parse(responseContent).ToObject<List>();
			return bulkUpsertResult;
		}

		public async Task<List[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(_endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// {
			//  "lists": [
			//    {
			//      "id": 1,
			//      "name": "the jones",
			//      "recipient_count": 1
			//    }
			//  ]
			// }
			// We use a dynamic object to get rid of the 'lists' property and simply return an array of lists
			dynamic dynamicObject = JObject.Parse(responseContent);
			dynamic dynamicArray = dynamicObject.lists;

			var lists = dynamicArray.ToObject<List[]>();
			return lists;
		}

		public async Task DeleteAsync(long listId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.DeleteAsync(string.Format("{0}/{1}", _endpoint, listId), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		public async Task DeleteAsync(IEnumerable<long> listIds, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = JArray.FromObject(listIds.ToArray());
			var response = await _client.DeleteAsync(_endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		public async Task<List> GetAsync(long listId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(string.Format("{0}/{1}", _endpoint, listId), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var list = JObject.Parse(responseContent).ToObject<List>();
			return list;
		}

		public async Task UpdateAsync(long listId, string name, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				new JProperty("name", name)
			};
			var response = await _client.PatchAsync(string.Format("{0}/{1}", _endpoint, listId), data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		public async Task<Contact[]> GetRecipientsAsync(long listId, int recordsPerPage = 100, int page = 1, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/{1}/recipients?page_size={2}&page={3}", _endpoint, listId, recordsPerPage, page);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// {
			//  "recipients": [
			//    {
			//      "created_at": 1422395108,
			//      "email": "e@example.com",
			//      "first_name": "Ed",
			//      "id": "YUBh",
			//      "last_clicked": null,
			//      "last_emailed": null,
			//      "last_name": null,
			//      "last_opened": null,
			//      "updated_at": 1422395108
			//    }
			//  ]
			// }
			// We use a dynamic object to get rid of the 'recipients' property and simply return an array of recipients
			dynamic dynamicObject = JObject.Parse(responseContent);
			dynamic dynamicArray = dynamicObject.recipients;

			var recipients = dynamicArray.ToObject<Contact[]>();
			return recipients;
		}

		public async Task AddRecipientAsync(long listId, string contactId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.PostAsync(string.Format("{0}/{1}/recipients/{2}", _endpoint, listId, contactId), (JObject)null, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		public async Task RemoveRecipientAsync(long listId, string contactId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.DeleteAsync(string.Format("{0}/{1}/recipients/{2}", _endpoint, listId, contactId), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		public async Task AddRecipientsAsync(long listId, IEnumerable<string> contactIds, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = JArray.FromObject(contactIds.ToArray());
			var response = await _client.PostAsync(string.Format("{0}/{1}/recipients", _endpoint, listId), data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}
	}
}
